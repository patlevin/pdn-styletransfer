// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Color
{
    /// <summary>
    /// Image data for colour transfer
    /// </summary>
    public struct ImageData
    {
        /// <summary>
        /// Pixel data in interlaced ARGB format
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// Image width in pixels
        /// </summary>
        public int Width;

        /// <summary>
        /// Image height in pixels
        /// </summary>
        public int Height;

        /// <summary>
        /// Return whether the image data is valid
        /// </summary>
        public bool IsValid
            => Width > 0 && Height > 0 && Data.Length >= Width * Height * 4;

        /// <summary>
        /// Return whether this data is compatible with other image data
        /// </summary>
        /// <param name="other">Image data to test for compatibility</param>
        /// <returns><c>true</c>, iff both are valid and of the same size</returns>
        public bool IsCompatibleWith(ImageData other)
        {
            return IsValid && other.IsValid &&
                   Width == other.Width &&
                   Height == other.Height;
        }
    }

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
        /// <param name="source">Source image data</param>
        /// <param name="target">Target image data</param>
        /// <param name="output">Output image data, must be compatible with target</param>
        /// <returns><c>true</c>, iff the colour transfer was successfull</returns>
        bool TransferColor(ImageData source, ImageData target, ImageData output);
    }
}
