// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Color
{
    /// <summary>
    /// Nearest-neighbour image sampler without boundary checks.
    /// Pixels are NOT centered, e.g. coordinates are 0 <= x|y < 1.
    /// </summary>
    sealed class Sampler
    {
        /// <summary>
        /// Initialise from image source.
        /// </summary>
        /// <param name="source">Image data in 8-bit ARGB format</param>
        public Sampler(ImageData source)
        {
            data = source.Data;
            fX = source.Width;
            fY = source.Height;
        }

        /// <summary>
        /// Sample a pixel from the source image.
        /// </summary>
        /// <param name="x">Normalised x-coordinate [0..1)</param>
        /// <param name="y">Normalised y-coordinate [0..1)</param>
        /// <returns>Tuple of 8-bit (red, green, blue) components</returns>
        public (byte, byte, byte) this[float x, float y]
        {
            get
            {
                var sx = (int)(x * fX);
                var sy = (int)(y * fY);
                var offset = (sy * fX + sx) * 4;
                return (data[offset + 2], data[offset + 1], data[offset + 0]);
            }
        }

        private readonly int fX;
        private readonly int fY;
        private readonly byte[] data;
    }
}
