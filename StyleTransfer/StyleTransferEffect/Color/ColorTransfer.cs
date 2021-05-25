// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin
using System;

namespace PaintDotNet.Effects.ML.StyleTransfer.Color
{
    /// <summary>
    /// Base class for color transfer methods.
    /// </summary>
    public abstract class ColorTransfer : IColorTransfer
    {
        /// <inheritdoc/>
        public bool TransferColor(ImageData source, ImageData target, ImageData output)
        {
            if (!source.IsValid)
                throw new ArgumentException("image data must be valid", "source");
            if (!target.IsValid)
                throw new ArgumentException("image data must be valid", "target");
            if (!output.IsValid)
                throw new ArgumentException("image data must be valid", "output");
            if (!output.IsCompatibleWith(target))
                throw new ArgumentException("image data incompatible with target", "output");

            if (!DoTransferColor(source, target, output))
            {
                Array.Copy(target.Data, output.Data, target.Data.Length);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Perform colour transfer
        /// </summary>
        /// <param name="source">Source image data</param>
        /// <param name="target">Target image data</param>
        /// <param name="output">Output image data</param>
        /// <returns><c>true</c>, iff the transfer was successful</returns>
        /// <remarks>
        /// All arguments are guaranteed to be valid; in case of failure,
        /// <paramref name="output"/> doesn't have to be modified.
        /// </remarks>
        protected abstract bool DoTransferColor(ImageData source, ImageData target, ImageData output);
    }
}
