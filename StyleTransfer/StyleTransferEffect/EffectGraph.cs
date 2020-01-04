// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    using Microsoft.ML.OnnxRuntime;
    using Microsoft.ML.OnnxRuntime.Tensors;

    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.Linq;

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
    public class EffectGraph : IDisposable
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
        /// Get the style extraction model
        /// </summary>
        public IEffectModel Style => style;

        /// <summary>
        /// Get the content transformer model
        /// </summary>
        public IEffectModel Transformer => transformer;

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
        public bool CanRun => style.Session != null && transformer.Session != null;

        /// <summary>
        /// Run the stylising using the current set of parameters stored in <see cref="Params"/>
        /// </summary>
        /// <param name="updateStyle">if true, calculates the style vector</param>
        /// <returns>Stylised <see cref="Params.Content"/></returns>
        public Tensor<float> Run(bool updateStyle = true)
        {
            Contract.Requires(style.Session != null, StringResources.MessageStyleModelNotLoaded);
            Contract.Requires(transformer.Session != null, StringResources.MessageTransformerModelNotLoaded);

            var sw = Stopwatch.StartNew();

            if (updateStyle)
            {
                UpdateStyleVector();
                GC.Collect();
            }

            UpdateIdentityTransform();
            GC.Collect();
            var styleInput = MergeVectors();
            var result = TransformContent(styleInput);
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
            Tensor<float> stylised;
            Update?.Invoke(this, new EffectGraphEventArgs(GraphEvent.TransformContent));
            var sw = Stopwatch.StartNew();

            using (var results = transformer.Session.Run(new NamedOnnxValue[]
            {
                NamedOnnxValue.CreateFromTensor(transformer.ContentImage, effectParams.Content),
                NamedOnnxValue.CreateFromTensor(transformer.StyleVector, styleInput)
            }))
            {
                stylised = results.Single().AsTensor<float>().Clone();
            }

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

        private void UpdateIdentityTransform()
        {
            Update?.Invoke(this, new EffectGraphEventArgs(GraphEvent.CalculateIdentity));

            var sw = Stopwatch.StartNew();
            if (effectParams.IsIdentityRequired)
            {
                using (var results = style.Session.Run(new NamedOnnxValue[]
                {
                    NamedOnnxValue.CreateFromTensor(style.InputImage, effectParams.ScaledContent)
                }))
                {
                    effectParams.IdentityVector = results.Single().AsTensor<float>().Clone();
                }
            }
            effectParams.IdentityTime = sw.Elapsed;
        }

        private void UpdateStyleVector()
        {
            Update?.Invoke(this, new EffectGraphEventArgs(GraphEvent.CalculateStyle));

            var sw = Stopwatch.StartNew();
            if (!effectParams.IsStyleVectorValid)
            {
                using (var results = style.Session.Run(new NamedOnnxValue[]
                {
                NamedOnnxValue.CreateFromTensor(style.InputImage, effectParams.Style)
                }))
                {
                    effectParams.StyleVector = results.Single().AsTensor<float>().Clone();
                }
            }
            effectParams.StyleTime = sw.Elapsed;
        }

        private readonly StyleModel style = new StyleModel();

        private readonly TransformerModel transformer = new TransformerModel();

        private readonly EffectParams effectParams = new EffectParams();

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    style.Dispose();
                    transformer.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
