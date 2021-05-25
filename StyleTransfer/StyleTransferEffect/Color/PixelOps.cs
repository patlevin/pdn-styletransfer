// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin
using System;
using PaintDotNet.Effects.ML.StyleTransfer.Maths;

namespace PaintDotNet.Effects.ML.StyleTransfer.Color
{
    /// <summary>
    /// Various pixel operations.
    /// </summary>
    internal static class PixelOps
    {
        /// <summary>
        /// Return the mean colour vector of an image
        /// </summary>
        /// <param name="pixelDataArgb">ARGB image data</param>
        /// <returns>Mean colour vector</returns>
        public static IVector3 Mean(byte[] pixelDataArgb)
        {
            var r = 0;
            var g = 0;
            var b = 0;

            for (int i = 0; i < pixelDataArgb.Length; i += 4)
            {
                b += pixelDataArgb[i + 0];
                g += pixelDataArgb[i + 1];
                r += pixelDataArgb[i + 2];
            }

            var oneOverN = 1f / (pixelDataArgb.Length / 4);
            return new Vector3(b * oneOverN, g * oneOverN, r * oneOverN);
        }

        /// <summary>
        /// Return the standard deviation of an image.
        /// </summary>
        /// <param name="pixelDataArgb">Image data in ARGB format</param>
        /// <param name="mean">Mean image colour</param>
        /// <returns>Vector containing the standard deviation of the image</returns>
        public static IVector3 StdDev(byte[] pixelDataArgb, IVector3 mean)
        {
            var v1 = 0f;
            var v2 = 0f;
            var v3 = 0f;
            var (m1, m2, m3) = mean;

            for (int i = 0; i < pixelDataArgb.Length; i += 4)
            {
                v1 += (pixelDataArgb[i + 0] - m1).Sqr();
                v2 += (pixelDataArgb[i + 1] - m2).Sqr();
                v3 += (pixelDataArgb[i + 2] - m3).Sqr();
            }

            var oneOverN = 1f / (pixelDataArgb.Length / 4);
            return new Vector3(
                (float)Math.Sqrt(v1 * oneOverN),
                (float)Math.Sqrt(v2 * oneOverN),
                (float)Math.Sqrt(v3 * oneOverN));
        }

        /// <summary>
        /// Return the covariance matrix of an image
        /// </summary>
        /// <param name="pixelDataArgb">Image data as ARGB</param>
        /// <param name="mean">Mean image colour</param>
        /// <returns>Covariance matrix of the image</returns>
        public static Matrix3 Covariance(byte[] pixelDataArgb, IVector3 mean)
        {
            Matrix3 cov = Matrix3.Zero;
            Matrix3 tmp = Matrix3.Zero;
            var v = Vector3.Zero;
            var t = Vector3.Zero;

            for (int i = 0; i < pixelDataArgb.Length; i += 4)
            {
                v.CopyFrom(pixelDataArgb, i);
                cov.Add(tmp.FromVector(v.Sub(mean, ref t)), ref cov);
            }

            return cov._ / (pixelDataArgb.Length / 4);
        }

        /// <summary>
        /// Perform a linear colour transfer operation.
        /// </summary>
        /// <param name="A">Colour transfer matrix</param>
        /// <param name="b">Colour offset</param>
        /// <param name="input">Input image data in ARGB format</param>
        /// <param name="output">Output image data in ARGB format</param>
        public static void LinearTransfer(Matrix3 A, IVector3 b, byte[] input, byte[] output)
        {
            var x = Vector3.Zero;
            var ax = Vector3.Zero;

            for (int i = 0; i < input.Length; i += 4)
            {
                x.CopyFrom(input, i);
                _ = A.Mul(x, ref ax);
                _ = ax.Add(b, ref x);
                x.CopyTo(output, i);
            }
        }

        /// <summary>
        /// Return the luminosity (as per Y'UV standard) of a colour.
        /// </summary>
        /// <param name="r">Red component</param>
        /// <param name="g">Green component</param>
        /// <param name="b">Blue component</param>
        /// <returns>Unscaled luminosity of the colour</returns>
        public static float Luma(byte r, byte g, byte b)
        {
            return r * 0.299f + g * 0.587f + b * 0.114f;
        }

        /// <summary>
        /// Return the luminosity (as per Y'UV standard) of a colour.
        /// </summary>
        /// <param name="data">Source pixels in interlaced ARGB format</param>
        /// <param name="offset">Offset of the blue-component (i.e. pixel offset)</param>
        /// <returns>Unscaled luminosity of the colour</returns>
        public static float Luma(byte[] data, int offset)
        {
            return data[offset + 2] * 0.299f +
                   data[offset + 1] * 0.587f +
                   data[offset + 0] * 0.114f;
        }

        /// <summary>
        /// Convert RGB to unscaled Y'UV
        /// </summary>
        /// <param name="r">Red component [0..255]</param>
        /// <param name="g">Green component [0..255]</param>
        /// <param name="b">Blue component [0..255]</param>
        /// <param name="y">Luminosity [0..255]</param>
        /// <param name="u">U (actually: Cb) component</param>
        /// <param name="v">V (actually: Cr) component</param>
        public static void RgbToYuv(byte r, byte g, byte b, out float y, out float u, out float v)
        {
            y = r * 0.299f + g * 0.587f + b * 0.114f;
            u = 0.493f * (b - y);
            v = 0.877f * (r - y);
        }

        /// <summary>
        /// Convert unscaled Y'UV to 8-bit RGB.
        /// </summary>
        /// <param name="y">Luminosity [0..255]</param>
        /// <param name="u">U (actually: Cb) component</param>
        /// <param name="v">V (actually: Cr) component</param>
        /// <param name="r">Red component [0..255]</param>
        /// <param name="g">Green component [0..255]</param>
        /// <param name="b">Blue component [0..255]</param>
        public static void YuvToRgb(float y, float u, float v, out byte r, out byte g, out byte b)
        {
            var ri = (int)Math.Round(y + 1 / 0.877f * v);
            var gi = (int)Math.Round(y - 0.144f / (0.587f * 0.493f) * u - 0.299f / (0.587f * 0.877f) * v);
            var bi = (int)Math.Round(y + 1 / 0.493f * u);

            r = (byte)(ri < 0 ? 0 : ri > 255 ? 255 : ri);
            g = (byte)(gi < 0 ? 0 : gi > 255 ? 255 : gi);
            b = (byte)(bi < 0 ? 0 : bi > 255 ? 255 : bi);
        }

        /// <summary>
        /// Convert 8-bit RGB to unscaled Y'UV 
        /// </summary>
        /// <param name="data">Source pixels in interlaced ARGB format</param>
        /// <param name="offset">Offset of the blue-component (i.e. pixel offset)</param>
        /// <param name="y">Luminosity [0..255]</param>
        /// <param name="u">U (actually: Cb) component</param>
        /// <param name="v">V (actually: Cr) component</param>
        public static void RgbToYuv(byte[] data, int offset, out float y, out float u, out float v)
        {
            RgbToYuv(data[offset + 2], data[offset + 1], data[offset + 0], out y, out u, out v);
        }

        /// <summary>
        /// Convert unscaled Y'UV to 8-bit RGB
        /// </summary>
        /// <param name="y">Luminosity [0..255]</param>
        /// <param name="u">U (actually: Cb) component</param>
        /// <param name="v">V (actually: Cr) component</param>
        /// <param name="data">Source pixels in interlaced ARGB format</param>
        /// <param name="offset">Offset of the blue-component (i.e. pixel offset)</param>
        public static void YuvToRgb(float y, float u, float v, byte[] data, int offset)
        {
            YuvToRgb(y, u, v, out data[offset + 2], out data[offset + 1], out data[offset + 0]);
        }
    }
}
