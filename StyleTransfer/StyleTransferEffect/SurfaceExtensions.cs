// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    using Microsoft.ML.OnnxRuntime.Tensors;
    using PaintDotNet.Rendering;

    using System;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.Runtime.InteropServices;

    /// <summary>
    /// ISurface extension methods
    /// </summary>
    public static class SurfaceExtensions
    {
        /// <summary>
        /// Return a normalised, 4D float tensor from a PiantDotNet ISurface
        /// </summary>
        /// <param name="surface">Surface to convert to tensor</param>
        /// <returns>Normalised, 4D float tensor</returns>
        public static Tensor<float> ToTensor(this ISurface<ColorBgra> surface)
        {
            Contract.Requires(surface != null);
            var (width, height, stride) = (surface.Width, surface.Height, surface.Stride);
            var buffer = new byte[stride];
            var memory = new float[width * height * 3];
            var offset = 0;

            for (var line = 0; line < height; ++line)
            {
                var ptr = surface.GetRowPointer<ColorBgra>(line);
                Marshal.Copy(ptr, buffer, 0, stride);
                for (var i = 0; i < stride; i += 4)
                {
                    memory[offset++] = buffer[i + 2] / 255.0f;
                    memory[offset++] = buffer[i + 1] / 255.0f;
                    memory[offset++] = buffer[i + 0] / 255.0f;
                }
            }

            ReadOnlySpan<int> dims = new int[] { 1, height, width, 3 };
            return new DenseTensor<float>(memory, dims);
        }

        /// <summary>
        /// Copy ISurface tile to a nomralised, 4D float tensor
        /// </summary>
        /// <param name="surface">Surface to copy pixels from</param>
        /// <param name="tensor">Tensor to copy pixels to</param>
        /// <param name="area">Size and location of the pixels</param>
        /// <returns>Normalised, 4D float tensor</returns>
        public static Tensor<float> CopyToTensor(this ISurface<ColorBgra> surface, Tensor<float> tensor, Rectangle area)
        {
            Contract.Requires(surface != null && tensor != null);

            var (x, y) = (area.X, area.Y);
            var (width, height) = (area.Width, area.Height);
            var buffer = new byte[width * 4];

            for (var row = 0; row < height; ++row)
            {
                _ = surface.GetRowArgb(buffer, x, row + y);
                tensor.SetRowArgb(buffer, 0, row);
            }

            return tensor;
        }

        /// <summary>
        /// Copy a row of pixels from an ISurface to a byte array
        /// </summary>
        /// <param name="surface">Surface that contains the pixels</param>
        /// <param name="buffer">Buffer that receives the data</param>
        /// <param name="x">Column to start copying</param>
        /// <param name="y">Row to copy</param>
        /// <returns>Filled buffer</returns>
        public static byte[] GetRowArgb(this ISurface<ColorBgra> surface, byte[] buffer, int x, int y)
        {
            Contract.Requires(buffer != null);
            var ptr = surface.GetPointPointer(x, y);
            Marshal.Copy(ptr, buffer, 0, buffer.Length);
            return buffer;
        }

        /// <summary>
        /// Copy a row of pixels from an ISurface to a byte array
        /// </summary>
        /// <param name="surface">Surface that contains the pixels</param>
        /// <param name="buffer">Buffer that receives the data</param>
        /// <param name="x">Column to start copying</param>
        /// <param name="y">Row to copy</param>
        /// <returns>Filled buffer</returns>
        public static ArraySegment<byte> GetRowArgb(this ISurface<ColorBgra> surface, ArraySegment<byte> buffer, int x, int y)
        {
            Contract.Requires(buffer != null);
            var ptr = surface.GetPointPointer(x, y);
            Marshal.Copy(ptr, buffer.Array, buffer.Offset, buffer.Count);
            return buffer;
        }

        /// <summary>
        /// Copy a row of pixels from a byte array to an ISurface
        /// </summary>
        /// <param name="surface">Surface to copy the pixels to</param>
        /// <param name="buffer">Buffer that contains the data</param>
        /// <param name="x">Column to start copying</param>
        /// <param name="y">Row to copy</param>
        public static void SetRowArgb(this ISurface<ColorBgra> surface, byte[] buffer, int x, int y)
        {
            Contract.Requires(buffer != null);
            var ptr = surface.GetPointPointer(x, y);
            Marshal.Copy(buffer, 0, ptr, buffer.Length);
        }

        /// <summary>
        /// Copy a row of pixels from a byte array to an ISurface
        /// </summary>
        /// <param name="surface">Surface to copy the pixels to</param>
        /// <param name="buffer">Buffer that contains the data</param>
        /// <param name="x">Column to start copying</param>
        /// <param name="y">Row to copy</param>
        public static void SetRowArgb(this ISurface<ColorBgra> surface, ArraySegment<byte> buffer, int x, int y)
        {
            Contract.Requires(buffer != null);
            var ptr = surface.GetPointPointer(x, y);
            Marshal.Copy(buffer.Array, buffer.Offset, ptr, buffer.Count);
        }
    }
}
