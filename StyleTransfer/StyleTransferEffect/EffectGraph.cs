// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

using Microsoft.ML.OnnxRuntime.Tensors;

using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Runtime.Versioning;

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    /// <summary>
    /// Current graph state
    /// </summary>
    public enum GraphEvent
    {
        /// <summary>
        /// Style calculation
        /// </summary>
        CalculateStyle,
        /// <summary>
        /// Identity transformation
        /// </summary>
        CalculateIdentity,
        /// <summary>
        /// Merging identity and style
        /// </summary>
        MergeStyles,
        /// <summary>
        /// Transform content image
        /// </summary>
        TransformContent,
        /// <summary>
        /// Perform post-processing
        /// </summary>
        PostProcess,
        /// <summary>
        /// Effect processing finished
        /// </summary>
        Finished
    }

    /// <summary>
    /// Event args published on graph updates
    /// </summary>
    public class EffectGraphEventArgs : EventArgs
    {
        public EffectGraphEventArgs(GraphEvent what)
        {
            What = what;
        }

        /// <summary>
        /// Get or set what the update is about
        /// </summary>
        public GraphEvent What { get; set; }
    }

    /// <summary>
    /// Wraps the effect calculation
    /// </summary>
    public class EffectGraph
    {
        /// <summary>
        /// Minimum style image dimension (width or height) in pixels
        /// </summary>
        public static int MinimumStyleSize => 64;

        /// <summary>
        /// Optimal lowest style image dimension (width or height) in pixels
        /// </summary>
        public static int OptimalStyleSizeLo => 96;

        /// <summary>
        /// Optimal highest style image dimension (width or height) in pixels
        /// </summary>
        public static int OptimalStyleSizeHi => 352;

        /// <summary>
        /// Fires when the graph is performing an update
        /// </summary>
        public event EventHandler<EffectGraphEventArgs> Update;

        /// <summary>
        /// Get the effect parameters (e.g. inputs and settings)
        /// </summary>
        public IEffectParams Params => effectParams;

        /// <summary>
        /// Get the current style vector
        /// </summary>
        public Tensor<float> StyleData => effectParams.StyleVector;

        /// <summary>
        /// Get whether the graph can be run (i.e. inference sessions are available)
        /// </summary>
        public bool CanRun
            => modelProvider.Style.Ready && modelProvider.Transform.Ready;

        /// <summary>
        /// Initialise graph from model provider
        /// </summary>
        /// <param name="provider">Model provider instance</param>
        public EffectGraph(IModelProvider provider)
        {
            modelProvider = provider;
        }

        /// <summary>
        /// Run the stylising using the current set of parameters stored in <see cref="Params"/>
        /// </summary>
        /// <param name="updateStyle">if true, calculates the style vector</param>
        /// <returns>Stylised <see cref="Params.Content"/></returns>
        public Tensor<float> Run(bool updateStyle = true)
        {
            Contract.Requires(modelProvider.Style.Ready, StringResources.MessageStyleModelNotLoaded);
            Contract.Requires(modelProvider.Transform.Ready, StringResources.MessageTransformerModelNotLoaded);

            var sw = Stopwatch.StartNew();

            if (updateStyle)
            {
                UpdateStyleVector();
                GC.Collect();
            }

            UpdateIdentityTransform();
            GC.Collect();
            var styleInput = MergeVectors();
            var stylised = TransformContent(styleInput);
            var result = PostProcess(stylised);
            GC.Collect();
            effectParams.TotalTime = sw.Elapsed;

            Update?.Invoke(this, new EffectGraphEventArgs(GraphEvent.Finished));

            return result;
        }

        /// <summary>
        /// Return the estimated required memory for running the graph
        /// </summary>
        /// <param name="size">Size of the input in pixels</param>
        /// <returns>Estimated number of bytes the graph requires given the input size</returns>
        public static long GetEstimatedRequiredMemory(SizeF size)
        {
            // parameters estimated by curve fitting of empirical data
            const double A = 0.001683427;
            const double B = 1.120338;

            double x = (double)size.Width * size.Height;
            return Math.Max(128L << 20, (long)Math.Ceiling(A * Math.Pow(x, B)) << 20);
        }

        /// <summary>
        /// Return the maximum number of pixels that can be processed using a given memory limit
        /// </summary>
        /// <param name="memoryInBytes">Memory limit in bytes</param>
        /// <returns>Maximum number of pixels that can be processed</returns>
        public static long GetMaximumPixelCount(long memoryInBytes)
        {
            // inverse of power approximation ax^b 
            const double A = 299.134;
            const double B = 0.8925877726186204;

            return (long)Math.Ceiling(A * Math.Pow(memoryInBytes >> 20, B));
        }

        /// <summary>
        /// Return the recommended tile size based on the available RAM
        /// </summary>
        /// <param name="size">Size of the input in pixels</param>
        /// <param name="margin">Tile margin in percent [0, 1.0]</param>
        /// <returns>Maximum tile size that can be processed safely</returns>
        [SupportedOSPlatform("windows")]
        public static Size GetRecommendedTileSize(Size size, float margin)
        {
            // the .NET heap *will* allocate much more than 33% of the available RAM
            // though the algorithm performs better with less (i.e. bigger) tiles in
            // terms of output quality, this will come at the expense of much longer
            // calculation time
            const int THRESHOLD_PERCENT = 33;
            var threshold = (AvailableMemory * THRESHOLD_PERCENT) / 100;
            var required = GetEstimatedRequiredMemory(size);
            if (required > threshold)
            {
                var factor = new RangedValue<float>(margin, 0.0f, 1.0f).Value;
                var pixels = GetMaximumPixelCount(threshold);
                var tileSize = (int)Math.Floor(Math.Sqrt(pixels));
                var n = (int)(tileSize / (1.0f + factor));
                if (TiledRenderer.GetTiles(size, n).Count > 1)
                {
                    return new Size(n, n);
                }
            }

            return size;
        }

        /// <summary>
        /// Get the available system memory in bytes
        /// </summary>
        [SupportedOSPlatform("windows")]
        public static long AvailableMemory
        {
            get
            {
                using (var pc = new PerformanceCounter("Memory", "Available Bytes"))
                {
                    return Convert.ToInt64(pc.NextValue());
                }
            }
        }

        private Tensor<float> TransformContent(Tensor<float> styleInput)
        {
            Update?.Invoke(this, new EffectGraphEventArgs(GraphEvent.TransformContent));
            var sw = Stopwatch.StartNew();
            var stylised = modelProvider.Transform.Run(effectParams.Content, styleInput);
            effectParams.TransformTime = sw.Elapsed;
            return stylised;
        }

        private Tensor<float> MergeVectors()
        {
            Update?.Invoke(this, new EffectGraphEventArgs(GraphEvent.MergeStyles));
            if (effectParams.StyleRatio < 1.0f)
            {
                var p = effectParams;
                return p.IdentityVector.Mix(p.StyleVector, p.StyleRatio);
            }
            else
            {
                return effectParams.StyleVector;
            }
        }

        private Tensor<float> PostProcess(Tensor<float> stylised)
        {
            if (effectParams.PostProcess != null)
            {
                var output = stylised.CloneEmpty();
                Update?.Invoke(this, new EffectGraphEventArgs(GraphEvent.PostProcess));
                _ = effectParams.PostProcess.TransferColor(effectParams.Content, stylised, output);
                return output;
            }

            return stylised;
        }

        private void UpdateIdentityTransform()
        {
            Update?.Invoke(this, new EffectGraphEventArgs(GraphEvent.CalculateIdentity));

            var sw = Stopwatch.StartNew();

            if (effectParams.IsIdentityRequired)
            {
                effectParams.IdentityVector = modelProvider.Style.Run(effectParams.ScaledContent);
            }

            effectParams.IdentityTime = sw.Elapsed;
        }

        private void UpdateStyleVector()
        {
            Update?.Invoke(this, new EffectGraphEventArgs(GraphEvent.CalculateStyle));

            var sw = Stopwatch.StartNew();

            if (!effectParams.IsStyleVectorValid)
            {
                effectParams.StyleVector = modelProvider.Style.Run(effectParams.Style);
            }

            effectParams.StyleTime = sw.Elapsed;
        }

        private readonly IModelProvider modelProvider;

        private readonly EffectParams effectParams = new EffectParams();
    }
}
