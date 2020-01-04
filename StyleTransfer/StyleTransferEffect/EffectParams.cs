// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    using Microsoft.ML.OnnxRuntime.Tensors;
    using System;
    
    /// <summary>
    /// State tracking effect parameters.
    /// </summary>
    /// <remarks>
    /// In an effort to reduce the amount of calculations, this class
    /// tracks the state of effect parameters and contains information
    /// about which results need to be updated.
    /// </remarks>
    internal class EffectParams : IEffectParams
    {
        private const float DEFAULT_STYLE_RATIO = 1.0f;

        /// <summary>
        /// Initialise defaults
        /// </summary>
        public EffectParams()
        {
            StyleRatio = DEFAULT_STYLE_RATIO;
        }

        /// <summary>
        /// Get or set the targeted image style
        /// </summary>
        public Tensor<float> Style
        {
            get => style;
            set { style = value; IsStyleVectorValid = false; }
        }

        /// <summary>
        /// Get or set the content image for identity transforms
        /// (defaults to <see cref="Content"/> if not set)
        /// </summary>
        public Tensor<float> ScaledContent
        {
            get => scaledContent ?? content;
            set { scaledContent = value; IsIdentityVectorValid = false; }
        }

        /// <summary>
        /// Get or set the content image
        /// </summary>
        public Tensor<float> Content
        {
            get => content;
            set { content = value; IsIdentityVectorValid = false; }
        }

        /// <summary>
        /// Get or set the style ratio (i.e. effect strength) [0..1]
        /// </summary>
        public float StyleRatio { get; set; }

        /// <summary>
        /// Get or set style compression time
        /// </summary>
        public TimeSpan StyleTime { get; internal set; }

        /// <summary>
        /// Get or set identity compression time
        /// </summary>
        public TimeSpan IdentityTime { get; internal set; }

        /// <summary>
        /// Get or set content transform time
        /// </summary>
        public TimeSpan TransformTime { get; internal set; }

        /// <summary>
        /// Get or set total processing time
        /// </summary>
        public TimeSpan TotalTime { get; internal set; }

        /// <summary>
        /// Get or set cached style vector intermediate result
        /// </summary>
        internal Tensor<float> StyleVector { get; set; }

        /// <summary>
        /// Get or set cached identity transformation intermediate result
        /// </summary>
        internal Tensor<float> IdentityVector { get; set; }

        /// <summary>
        /// Get whether style is current
        /// </summary>
        internal bool IsStyleVectorValid { get; private set; }

        /// <summary>
        /// Get whether identity is current
        /// </summary>
        internal bool IsIdentityVectorValid { get; private set; }

        /// <summary>
        /// Get whether identity transform needs update
        /// </summary>
        internal bool IsIdentityRequired => StyleRatio < 1.0f && !IsIdentityVectorValid;

        /// <summary>
        /// Set the style vector directly
        /// </summary>
        /// <param name="tensor">Style vector (maybe <c>null</c>)</param>
        public void SetStyleVector(Tensor<float> tensor)
        {
            StyleVector = tensor;
            IsStyleVectorValid = tensor != null;
        }

        // style image parameter
        private Tensor<float> style;

        // content image parameter
        private Tensor<float> content;

        // [optional] content image parameter
        private Tensor<float> scaledContent;
    }
}
