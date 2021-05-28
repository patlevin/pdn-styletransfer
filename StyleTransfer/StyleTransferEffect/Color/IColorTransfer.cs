// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

using Microsoft.ML.OnnxRuntime.Tensors;

namespace PaintDotNet.Effects.ML.StyleTransfer.Color
{
    /// <summary>
    /// Interface for colour transfer methods.
    /// </summary>
    /// <remarks>
    /// Colour transfer attempts to transfer the colour palette of a source
    /// image to a target image of a potentially different colour distribution.
    /// </remarks>
    public interface IColorTransfer
    {
        /// <summary>
        /// Transfer the colours of a source image to a target image.
        /// </summary>
        /// <remarks>
        /// A colour transfer might not be possible if either the source or
        /// target image are monochromatic. In such cases the function will
        /// return <c>false</c> and copy the pixels of targetArb to outputArgb
        /// unchanged.
        /// 
        /// Alpha channels are unaffected by the colour transfer and only
        /// colour channels are processed. The input images will only be
        /// read and stay unmodified.
        /// </remarks>
        /// <param name="source">Source tensor (normalised RGB, NHWC-format)</param>
        /// <param name="target">Target tensor (normalised RGB, NHWC-format)</param>
        /// <param name="output">Output tensor, must be compatible with target</param>
        /// <returns><c>true</c>, iff the colour transfer was successfull</returns>
        bool TransferColor(Tensor<float> source, Tensor<float> target, Tensor<float> output);
    }
}
