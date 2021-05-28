// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin
using System;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime.Tensors;
using PaintDotNet.Effects.ML.StyleTransfer.Maths;
using Vec3 = System.ValueTuple<float, float, float>;

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
        /// <param name="tensor">Normalised RGB image tensor</param>
        /// <returns>Mean colour vector</returns>
        public static IVector3 Mean(Tensor<float> tensor)
        {
            var granularity = tensor.Dimensions[3];
            var total = (int)tensor.Length;
            var (m1, m2, m3) = Aggregate(total, granularity, (0f, 0f, 0f), Sum, Merge);
            var s = 1f / (tensor.Width() * tensor.Height());
            return new Vector3(m1 * s, m2 * s, m3 * s);

            Vec3 Sum(int start, int end)
            {
                var (s1, s2, s3) = (0f, 0f, 0f);
                var data = ((DenseTensor<float>)tensor).Buffer.Span;
                for (int i = start; i < end; i += 3)
                {
                    s1 += data[i + 0];
                    s2 += data[i + 1];
                    s3 += data[i + 2];
                }
                return (s1, s2, s3);
            }

            Vec3 Merge(Vec3 accumulate, Vec3 current)
            {
                return (accumulate.Item1 + current.Item1,
                        accumulate.Item2 + current.Item2,
                        accumulate.Item3 + current.Item3);
            }
        }

        /// <summary>
        /// Return the standard deviation of an image.
        /// </summary>
        /// <param name="tensor">RGB image tensor in NHWC-format</param>
        /// <param name="mean">Mean image colour</param>
        /// <returns>Vector containing the standard deviation of the image</returns>
        public static IVector3 StdDev(Tensor<float> tensor, IVector3 mean)
        {
            var total = (int)tensor.Length;
            var granularity = tensor.Dimensions[3];
            var (s1, s2, s3) = Aggregate(total, granularity, (0f, 0f, 0f), Sum, Merge);

            var s = 1f / (tensor.Width() * tensor.Height());
            return new Vector3(
                (float)Math.Sqrt(s1 * s),
                (float)Math.Sqrt(s2 * s),
                (float)Math.Sqrt(s3 * s));

            Vec3 Sum(int start, int end)
            {
                var (m1, m2, m3) = mean;
                var (v1, v2, v3) = (0f, 0f, 0f);

                for (int i = start; i < end; i += 3)
                {
                    v1 += (tensor[i + 0] - m1).Sqr();
                    v2 += (tensor[i + 1] - m2).Sqr();
                    v3 += (tensor[i + 2] - m3).Sqr();
                }

                return (v1, v2, v3);
            }

            Vec3 Merge(Vec3 agg, Vec3 cur)
            {
                return (agg.Item1 + cur.Item1,
                        agg.Item2 + cur.Item2,
                        agg.Item3 + cur.Item3);
            }
        }

        /// <summary>
        /// Return the covariance matrix of an image
        /// </summary>
        /// <param name="tensor">Image tensor (normalised RGB, NHWC format)</param>
        /// <param name="mean">Mean image colour</param>
        /// <returns>Covariance matrix of the image</returns>
        public static Matrix3 Covariance(Tensor<float> tensor, IVector3 mean)
        {
            var granularity = tensor.Dimensions[3];
            var total = (int)tensor.Length;
            var cov = Aggregate(total, granularity, Matrix3.Zero, Sum, Merge);
            return cov._ / (tensor.Width() * tensor.Height());

            Matrix3 Sum(int start, int end)
            {
                var agg = Matrix3.Zero;
                var tmp = Matrix3.Zero;
                var (m1, m2, m3) = mean;
                var data = ((DenseTensor<float>)tensor).Buffer.Span;

                for (int i = start; i < end; i += 3)
                {
                    tmp.FromVector(data[i + 0] - m1, data[i + 1] - m2, data[i + 2] - m3);
                    agg.Add(tmp, agg);
                }

                return agg;
            }

            Matrix3 Merge(Matrix3 acc, Matrix3 cur)
            {
                return acc.Add(cur, acc);
            }
        }

        /// <summary>
        /// Perform a linear colour transfer operation.
        /// </summary>
        /// <param name="A">Coefficient matrix</param>
        /// <param name="b">Colour offset</param>
        /// <param name="input">Input image data in ARGB format</param>
        /// <param name="output">Output image data in ARGB format</param>
        public static void LinearTransfer(Matrix3 A, IVector3 b, Tensor<float> input, Tensor<float> output)
        {
            var stride = input.Strides[1];
            Parallel.For(0, input.Height(), row =>
            {
                var source = ((DenseTensor<float>)input).Buffer.Span;
                var destination = ((DenseTensor<float>)output).Buffer.Span;
                var (b1, b2, b3) = b;
                var (start, end) = (row * stride, (row + 1) * stride);

                for (int i = start; i < end; i += 3)
                {
                    var (x1, x2, x3) = A.Mul(source[i + 0], source[i + 1], source[i + 2]);
                    destination[i + 0] = b1 + x1;
                    destination[i + 1] = b2 + x2;
                    destination[i + 2] = b3 + x3;
                }
            });
        }

        /// <summary>
        /// Return the luminosity (as per Y'UV standard) of a colour.
        /// </summary>
        /// <param name="data">Source pixels in interlaced RGB format</param>
        /// <param name="offset">Offset of the blue-component (i.e. pixel offset)</param>
        /// <returns>Luminosity (Y-signal) of the colour</returns>
        public static float Luma(Span<float> data, int offset)
        {
            return data[offset + 0] * 0.299f +
                   data[offset + 1] * 0.587f +
                   data[offset + 2] * 0.114f;
        }

        /// <summary>
        /// Convert RGB to unscaled Y'UV
        /// </summary>
        /// <param name="r">Red component [0..1]</param>
        /// <param name="g">Green component [0..1]</param>
        /// <param name="b">Blue component [0..1]</param>
        /// <returns>Tuple of (Y, U, V) components (technically Y Cb Cr)</returns>
        public static (float, float, float) RgbToYuv(float r, float g, float b)
        {
            var y = r * 0.299f + g * 0.587f + b * 0.114f;
            return (y, 0.493f * (b - y), 0.877f * (r - y));
        }

        /// <summary>
        /// Convert Y'UV to RGB.
        /// </summary>
        /// <param name="y">Luminosity (Y-signal)</param>
        /// <param name="u">U (actually: Cb) component</param>
        /// <param name="v">V (actually: Cr) component</param>
        /// <param name="destination">Target for storing RGB result</param>
        /// <param name="offset">Target offset (e.g. index of the red component)</param>
        public static void YuvToRgb(float y, float u, float v, Span<float> destination, int offset)
        {
            destination[offset + 0] = Clamp(y + 1 / 0.877f * v);
            destination[offset + 1] = Clamp(y - 0.144f / (0.587f * 0.493f) * u - 0.299f / (0.587f * 0.877f) * v);
            destination[offset + 2] = Clamp(y + 1 / 0.493f * u);

            float Clamp(float value) => value < 0 ? 0 : value > 1 ? 1 : value; 
        }

        /// <summary>
        /// Aggregate values (of a tensor) in parallel.
        /// </summary>
        /// <typeparam name="T">Type of the aggregate value</typeparam>
        /// <param name="total">Total number of (tensor-) elements</param>
        /// <param name="granularity">Number of channels (in the tensor)</param>
        /// <param name="init">Initial value</param>
        /// <param name="sum">
        /// Function that returns the sums of a slice of data.
        /// The arguments arethe first (inclusive) and last (exclusive) element
        /// index. Actual data must be passed via captures. Indexes are guaranteed
        /// to not overlap so access within these boundaries is thread-safe.
        /// </param>
        /// <param name="merge">Function that combines two sums and returns a total</param>
        /// <returns>Total aggregate over all elements</returns>
        public static T Aggregate<T>(int total, int granularity, T init, Func<int, int, T> sum, Func<T, T, T> merge)
        {
            var numCpus = Environment.ProcessorCount;
            var blockSize = (total / (granularity * numCpus)) * granularity;
            var blockCount = total / blockSize + ((total % blockSize) > 0 ? 1 : 0);
            var _lock = new object();
            T result = init;

            Parallel.For(0, blockCount, block =>
            {
                var start = block * blockSize;
                var end = Math.Min(start + blockSize, total);
                var agg = sum(start, end);

                lock (_lock)
                {
                    result = merge(result, agg);
                }
            });

            return result;
        }
    }
}
