// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    using Microsoft.ML.OnnxRuntime.Tensors;
    using PaintDotNet.Rendering;

    using System;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Tensor extension methods
    /// </summary>
    public static class TensorExtensions
    {
        /// <summary>
        /// Return Tensor width if NHWC-formatted
        /// </summary>
        public static int Width<T>(this Tensor<T> tensor)
        {
            Contract.Requires(tensor != null);
            return tensor.Dimensions[2];
        }

        /// <summary>
        /// Return Tensor height if NHWC-formatted
        /// </summary>
        public static int Height<T>(this Tensor<T> tensor)
        {
            Contract.Requires(tensor != null);
            return tensor.Dimensions[1];
        }

        /// <summary>
        /// Return dimensions for an NHWC-formatted tensor from a rectangle
        /// </summary>
        public static int[] ToNHWC(this Rectangle rect)
        {
            return new int[] { 1, rect.Height, rect.Width, 3 };
        }

        /// <summary>
        /// Returns the linear blend of x and y, i.e.
        /// the product of x and (1 - a) plus the product of y and a.
        /// </summary>
        /// <param name="x">First tensor to mix</param>
        /// <param name="y">Second tensor to mix</param>
        /// <param name="a">Blend factor [0..1]</param>
        /// <returns>Linear blending between two tensors</returns>
        public static Tensor<float> Mix(this Tensor<float> x, Tensor<float> y, float a)
        {
            Contract.Requires(x != null && y != null);
            // limit "a" to [0..1]
            a = a < 0.0f ? 0.0f : a > 1.0f ? 1.0f : a;

            var xa = ((DenseTensor<float>)x).Buffer.Span;
            var ya = ((DenseTensor<float>)y).Buffer.Span;
            var za = new float[xa.Length];
            var b = 1.0f - a;

            for (var i = 0; i < x.Length; ++i)
            {
                za[i] = xa[i] * b + ya[i] * a;
            }

            return new DenseTensor<float>(za, x.Dimensions);
        }

        /// <summary>
        /// Return a normalised [0..1], 4D (NHWC) float tensor from a bitmap
        /// </summary>
        /// <param name="bitmap">Bitmap to be converted</param>
        /// <param name="scaling">Image scaling factor (clamped to [0..1])</param>
        /// <returns>Normalised, 4D Tensor representation of the bitmap</returns>
        public static Tensor<float> ToTensor(this Bitmap bitmap, float scaling = 1.0f)
        {
            Contract.Requires(bitmap != null);
            using (var input = ScaleImage(bitmap, scaling.Clamp(0, 1)))
            {
                return ToTensor(input);
            }
        }

        /// <summary>
        /// Copy normalised, 4D, NHWC-formatted tensor to an BGRA surface
        /// </summary>
        /// <param name="tensor">NHWC-formatted float tensor</param>
        /// <param name="surface">Target BGRA surface</param>
        /// <returns>Updated surface</returns>
        public static ISurface<ColorBgra> ToSurface(this Tensor<float> tensor, ISurface<ColorBgra> surface)
        {
            Contract.Requires(tensor != null && surface != null);

            // assume NHWC formatted tensor
            var cols = Math.Min(surface.Width, tensor.Dimensions[2]);
            var rows = Math.Min(surface.Height, tensor.Dimensions[1]);
            var data = ((DenseTensor<float>)tensor).Buffer.Span;
            var line = new byte[surface.Stride];
            var stride = tensor.Strides[1];

            for (int row = 0; row < rows; ++row)
            {
                var index = row * stride;
                for (int col = 0; col < cols; ++col, index += 3)
                {
                    // convert from RGB [0..1] to BGRA [0..255] 
                    line[col * 4 + 0] = Clamp(data[index + 2] * 255);
                    line[col * 4 + 1] = Clamp(data[index + 1] * 255);
                    line[col * 4 + 2] = Clamp(data[index + 0] * 255);
                    line[col * 4 + 3] = 255;
                }

                var ptr = surface.GetRowPointer<ColorBgra>(row);
                Marshal.Copy(line, 0, ptr, line.Length);
            }

            return surface;
        }

        /// <summary>
        /// Fill a buffer with ARGB values from an NHWC-formatted, normalised BGR float tensor 
        /// </summary>
        /// <param name="tensor">NHWC-formatted, normalised BGR float Tensor</param>
        /// <param name="row">Row to be extracted from the tensor</param>
        /// <param name="x">Column to start extraction</param>
        /// <param name="y">Index of the row to be extracted</param>
        /// <returns>Number of bytes copied to row buffer</returns>
        public static int GetRowArgb(this Tensor<float> tensor, byte[] row, int x, int y)
        {
            Contract.Requires(tensor != null && row != null);

            var stride = tensor.Dimensions[2] * tensor.Dimensions[3];
            var offset = y * stride + x * 3;
            var columns = Math.Min(tensor.Dimensions[2] - x, row.Length / 4);
            var data = ((DenseTensor<float>)tensor).Buffer.Span;

            // rescale and remap BGR to ARGB
            for (int i = 0; i < columns; ++i)
            {
                row[i * 4 + 2] = Clamp(data[offset + 3 * i + 0] * 255);
                row[i * 4 + 1] = Clamp(data[offset + 3 * i + 1] * 255);
                row[i * 4 + 0] = Clamp(data[offset + 3 * i + 2] * 255);
                row[i * 4 + 3] = 255;
            }

            return columns * 4;
        }

        /// <summary>
        /// Fill a buffer with ARGB values from an NHWC-formatted, normalised BGR float tensor 
        /// </summary>
        /// <param name="tensor">NHWC-formatted, normalised BGR float Tensor</param>
        /// <param name="row">Row to be extracted from the tensor</param>
        /// <param name="x">Column to start extraction</param>
        /// <param name="y">Index of the row to be extracted</param>
        /// <returns>Number of bytes copied to row buffer</returns>
        public static int GetRowArgb(this Tensor<float> tensor, ArraySegment<byte> row, int x, int y)
        {
            Contract.Requires(tensor != null);

            var stride = tensor.Dimensions[2] * tensor.Dimensions[3];
            var offset = y * stride + x * 3;
            var columns = Math.Min(tensor.Dimensions[2] - x, row.Count / 4);
            var data = ((DenseTensor<float>)tensor).Buffer.Span;
            var dest = row.Array;

            // rescale and remap BGR to ARGB
            for (int i = 0, j = row.Offset; i < columns; ++i, j += 4)
            {
                dest[j + 2] = Clamp(data[offset + 3 * i + 0] * 255);
                dest[j + 1] = Clamp(data[offset + 3 * i + 1] * 255);
                dest[j + 0] = Clamp(data[offset + 3 * i + 2] * 255);
                dest[j + 3] = 255;
            }

            return columns * 4;
        }

        /// <summary>
        /// Fill a buffer with ARGB values from a GDI+ bitmap
        /// </summary>
        /// <param name="data">Readable bitmap data from locked bitmap</param>
        /// <param name="row">Row to be extracted from the bitmap</param>
        /// <param name="x">Column to start extraction</param>
        /// <param name="y">Index of the row to be extracted</param>
        /// <returns>Number of bytes copied to row buffer</returns>
        public static int GetRowArgb(this BitmapData data, byte[] row, int x, int y)
        {
            Contract.Requires(data != null && row != null);

            var offset = y * data.Stride + x * 4;
            var bytes = Math.Min((data.Width - x) * 4, row.Length);
            Marshal.Copy(IntPtr.Add(data.Scan0, offset), row, 0, bytes);
            return bytes;
        }

        /// <summary>
        /// Fill a buffer with ARGB values from a GDI+ bitmap
        /// </summary>
        /// <param name="data">Readable bitmap data from locked bitmap</param>
        /// <param name="row">Row to be extracted from the bitmap</param>
        /// <param name="x">Column to start extraction</param>
        /// <param name="y">Index of the row to be extracted</param>
        /// <returns>Number of bytes copied to row buffer</returns>
        public static int GetRowArgb(this BitmapData data, ArraySegment<byte> row, int x, int y)
        {
            Contract.Requires(data != null);

            var offset = y * data.Stride + x * 4;
            var bytes = Math.Min((data.Width - x) * 4, row.Count);
            Marshal.Copy(IntPtr.Add(data.Scan0, offset), row.Array, row.Offset, bytes);
            return bytes;
        }

        /// <summary>
        /// Copy a row of ARGB pixel values to an NHWC-formatted BGR tensor
        /// </summary>
        /// <param name="tensor">NHWC-formatted BGR-valued tensor</param>
        /// <param name="data">Data row containing ARGB pixel values</param>
        /// <param name="x">X-offset to copy to</param>
        /// <param name="y">Y-offset to copy to</param>
        /// <returns>Number of copied elements</returns>
        public static int SetRowArgb(this Tensor<float> tensor, byte[] data, int x, int y)
        {
            Contract.Requires(tensor != null && data != null);
            var memory = ((DenseTensor<float>)tensor).Buffer.Span;
            var width = tensor.Width();
            var offset = y * width * 3 + x * 3;
            var cols = Math.Min(data.Length / 4, width);

            for (int i = 0, j = offset; i < cols; ++i)
            {
                memory[j++] = data[i * 4 + 2] / 255f;
                memory[j++] = data[i * 4 + 1] / 255f;
                memory[j++] = data[i * 4 + 0] / 255f;
            }

            return cols * 3;
        }

        /// <summary>
        /// Copy a row of ARGB pixel values to an NHWC-formatted BGR tensor
        /// </summary>
        /// <param name="tensor">NHWC-formatted BGR-valued tensor</param>
        /// <param name="data">Data row containing ARGB pixel values</param>
        /// <param name="x">X-offset to copy to</param>
        /// <param name="y">Y-offset to copy to</param>
        /// <returns>Number of copied elements</returns>
        public static int SetRowArgb(this Tensor<float> tensor, ArraySegment<byte> data, int x, int y)
        {
            Contract.Requires(tensor != null);
            var memory = ((DenseTensor<float>)tensor).Buffer.Span;
            var source = data.Array;
            var width = tensor.Width();
            var offset = y * width * 3 + x * 3;
            var cols = Math.Min(data.Count / 4, width);

            for (int i = 0, j = offset, k = data.Offset; i < cols; ++i, k += 4)
            {
                memory[j++] = source[k + 2] / 255f;
                memory[j++] = source[k + 1] / 255f;
                memory[j++] = source[k + 0] / 255f;
            }

            return cols * 3;
        }

        /// <summary>
        /// Write a buffer with ARGB values to a GDI+ bitmap
        /// </summary>
        /// <param name="data">Writable bitmap data from locked bitmap</param>
        /// <param name="row">Row to be written to the bitmap</param>
        /// <param name="x">Column to start writing</param>
        /// <param name="y">Index of the row to be written</param>
        public static void SetRow(this BitmapData data, byte[] row, int x, int y)
        {
            Contract.Requires(data != null && row != null);

            var offset = y * data.Stride + x * 4;
            var length = Math.Min((data.Width - x) * 4, row.Length);

            Marshal.Copy(row, 0, IntPtr.Add(data.Scan0, offset), length);
        }

        /// <summary>
        /// Write a buffer with ARGB values to a GDI+ bitmap
        /// </summary>
        /// <param name="data">Writable bitmap data from locked bitmap</param>
        /// <param name="row">Row to be written to the bitmap</param>
        /// <param name="x">Column to start writing</param>
        /// <param name="y">Index of the row to be written</param>
        public static void SetRow(this BitmapData data, ArraySegment<byte> row, int x, int y)
        {
            Contract.Requires(data != null);

            var offset = y * data.Stride + x * 4;
            var length = Math.Min((data.Width - x) * 4, row.Count);

            Marshal.Copy(row.Array, row.Offset, IntPtr.Add(data.Scan0, offset), length);
        }

        /// <summary>
        /// Mix ARGB vectors
        /// </summary>
        /// <param name="x">Vector to mix</param>
        /// <param name="y">Vector to mix</param>
        /// <param name="z">Result vector</param>
        /// <param name="c">Mixing coefficient</param>
        /// <returns>Mixed vector z = (1-c)x + cy</returns>
        public static byte[] MixArgb(this byte[] x, byte[] y, byte[] z, float c)
        {
            Contract.Requires(x != null && y != null && z != null);

            int len = x.Length / 4;
            var b = 1.0f - c;
            for (int i = 0, j = 0; i < len; ++i, ++j)
            {
                z[j] = Clamp(x[j] * b + y[j] * c); ++j;
                z[j] = Clamp(x[j] * b + y[j] * c); ++j;
                z[j] = Clamp(x[j] * b + y[j] * c); ++j;
                z[j] = 255;
            }

            return z;
        }

        /// <summary>
        /// Mix ARGB vectors
        /// </summary>
        /// <param name="x">Vector to mix</param>
        /// <param name="y">Vector to mix</param>
        /// <param name="z">Result vector</param>
        /// <param name="c">Mixing coefficient</param>
        /// <returns>Mixed vector z = (1-c)x + cy</returns>
        public static void MixArgb(this ArraySegment<byte> x, ArraySegment<byte> y, ArraySegment<byte> z, float c)
        {
            int len = x.Count / 4;
            var b = 1.0f - c;
            var (s, t, r) = (x.Offset, y.Offset, z.Offset);
            var (xa, ya, za) = (x.Array, y.Array, z.Array);

            for (int i = 0; i < len; ++i, ++s, ++t)
            {
                za[r++] = Clamp(xa[s++] * b + ya[t++] * c);
                za[r++] = Clamp(xa[s++] * b + ya[t++] * c);
                za[r++] = Clamp(xa[s++] * b + ya[t++] * c);
                za[r++] = 255;
            }
        }

        /// <summary>
        /// Mix ARGB vectors
        /// </summary>
        /// <param name="x">Vector to mix</param>
        /// <param name="y">Vector to mix</param>
        /// <param name="z">Result vector</param>
        /// <param name="c">Mixing coefficient</param>
        /// <returns>Mixed vector z = (1-c)x + cy</returns>
        public static ArraySegment<byte> LinearBlend(this ArraySegment<byte> x, ArraySegment<byte> y, ArraySegment<byte> z)
        {
            var k = Math.Min(x.Count, Math.Min(y.Count, z.Count)) / 4;
            if (k == 0)
            {
                return z;
            }

            var di = 1.0f / k;
            var c = di;
            var (xa, ya, za) = (x.Array, y.Array, z.Array);
            var (xi, yi, zi) = (x.Offset, y.Offset, z.Offset);

            for (int i = 0; i < k; ++i, c += di)
            {
                var b = 1.0f - c;
                za[zi++] = Clamp(b * xa[xi++] + ya[yi++] * c);
                za[zi++] = Clamp(b * xa[xi++] + ya[yi++] * c);
                za[zi++] = Clamp(b * xa[xi++] + ya[yi++] * c);
                za[zi++] = 255; xi++; yi++;
            }

            return z;
        }

        /// <summary>
        /// Return the size scaled uniformly by a factor
        /// </summary>
        /// <param name="size">Size to be scaled</param>
        /// <param name="s">Scaling factor</param>
        /// <returns>Uniformly scaled size</returns>
        public static SizeF ScaleBy(this Size size, float s)
        {
            return new SizeF(size.Width * s, size.Height * s);
        }

        // Return scaled image
        private static Bitmap ScaleImage(Bitmap image, float scaling)
        {
            if (Math.Abs(1.0f - scaling) < float.Epsilon * 10)
            {
                return image;
            }

            var (width, height) = (image.Width * scaling, image.Height * scaling);
            var scaled = new Bitmap((int)Math.Round(width), (int)Math.Round(height));

            using (var g = Graphics.FromImage(scaled))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(image, new Rectangle(0, 0, scaled.Width, scaled.Height));
            }

            return scaled;
        }

        // Return tensor from bitmap
        private static Tensor<float> ToTensor(Bitmap image)
        {
            var rect = new Rectangle(Point.Empty, image.Size);
            var tensor = new DenseTensor<float>(rect.ToNHWC());
            var data = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var row = new byte[data.Stride];

            for (var i = 0; i < rect.Height; ++i)
            {
                _ = data.GetRowArgb(row, 0, i);
                tensor.SetRowArgb(row, 0, i);
            }

            image.UnlockBits(data);
            return tensor;
        }

        private static byte Clamp(float v)
        {
            return (byte)(v < 0 ? 0 : v > 255 ? 255 : v);
        }
    }
}
