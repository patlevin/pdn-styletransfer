// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    using Microsoft.ML.OnnxRuntime.Tensors;
    using PaintDotNet.Effects.ML.StyleTransfer.Color;
    using System;

    /// <summary>
    /// Parameters for calculating the effect
    /// </summary>
    public interface IEffectParams
    {
        /// <summary>
        /// Get or set the style image - normalised 4D tensor.
        /// </summary>
        Tensor<float> Style { get; set; }

        /// <summary>
        /// Get or set a scaled content image for mixing - normalised 4D tensor.
        /// Only applicable if <see cref="StyleRatio"/> is less than 1.0. 
        /// </summary>
        Tensor<float> ScaledContent { get; set; }

        /// <summary>
        /// Get or set the content image - normalised 4D tensor.
        /// </summary>
        Tensor<float> Content { get; set; }

        /// <summary>
        /// Get or set the style ratio [0..1] (i.e. the effect strength)
        /// </summary>
        float StyleRatio { get; set; }

        /// <summary>
        /// Get or set optional post-processing step
        /// </summary>
        IColorTransfer PostProcess { get; set; }

        #region Performance Information
        /// <summary>
        /// Style compression time in ms
        /// </summary>
        TimeSpan StyleTime { get; }

        /// <summary>
        /// Identity compression time in ms
        /// </summary>
        TimeSpan IdentityTime { get; }

        /// <summary>
        /// Content transformation time in ms
        /// </summary>
        TimeSpan TransformTime{ get; }

        /// <summary>
        /// Total effect processing time in ms
        /// </summary>
        TimeSpan TotalTime { get; }
        #endregion

        /// <summary>
        /// Set the style vector directly
        /// </summary>
        /// <param name="tensor">Style vector (maybe <c>null</c>)</param>
        void SetStyleVector(Tensor<float> tensor);
    }
}
