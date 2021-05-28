// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin
using Microsoft.ML.OnnxRuntime.Tensors;
using System;

namespace PaintDotNet.Effects.ML.StyleTransfer.Color
{
    /// <summary>
    /// Base class for color transfer methods.
    /// </summary>
    public abstract class ColorTransfer : IColorTransfer
    {
        /// <inheritdoc/>
        public bool TransferColor(Tensor<float> source, Tensor<float> target, Tensor<float> output)
        {
            if (!output.Dimensions.SequenceEqual(target.Dimensions))
                throw new ArgumentException("image data incompatible with target", "output");

            if (!DoTransferColor(source, target, output))
            {
                ((DenseTensor<float>)target).Buffer.Span.CopyTo(
                    ((DenseTensor<float>)output).Buffer.Span);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Perform colour transfer
        /// </summary>
        /// <param name="source">Source image tensor</param>
        /// <param name="target">Target image tensor</param>
        /// <param name="output">Output image tensor</param>
        /// <returns><c>true</c>, iff the transfer was successful</returns>
        /// <remarks>
        /// All arguments are guaranteed to be valid; in case of failure,
        /// <paramref name="output"/> doesn't have to be modified.
        /// </remarks>
        protected abstract bool DoTransferColor(Tensor<float> source, Tensor<float> target, Tensor<float> output);
    }
}
