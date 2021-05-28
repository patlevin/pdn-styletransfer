using System;

namespace PaintDotNet.Effects.ML.StyleTransfer.Maths
{
    /// <summary>
    /// IVector3 operations extension class.
    /// </summary>
    public static class VectorOps
    {
        public static T[] CloneT<T>(this T[] array)
        {
            return (T[])array.Clone();
        }

        /// <summary>
        /// Deconstructing into vector elements.
        /// </summary>
        public static void Deconstruct(this IVector3 v, out float x, out float y, out float z)
        {
            x = v[0];
            y = v[1];
            z = v[2];
        }

        /// <summary>
        /// Swap two vectors
        /// </summary>
        /// <param name="a">Vector to be swapped</param>
        /// <param name="b">Vector to swap with</param>
        public static void Swap(this IVector3 a, IVector3 b)
        {
            (a[0], b[0]) = (b[0], a[0]);
            (a[1], b[1]) = (b[1], a[1]);
            (a[2], b[2]) = (b[2], a[2]);
        }

        /// <summary>
        /// Copy the contents of another vector.
        /// </summary>
        public static void Copy(this IVector3 a, IVector3 b)
        {
            a[0] = b[0];
            a[1] = b[1];
            a[2] = b[2];
        }

        /// <summary>
        /// Subtract a vector.
        /// </summary>
        /// <param name="a">Left hand vector</param>
        /// <param name="b">Vector to be subtracted</param>
        /// <param name="result">Vector that receives the result</param>
        /// <returns>Difference vector a-b</returns>
        public static IVector3 Sub(this IVector3 a, IVector3 b, ref IVector3 result)
        {
            result[0] = a[0] - b[0];
            result[1] = a[1] - b[1];
            result[2] = a[2] - b[2];
            return result;
        }

        /// <summary>
        /// Return the result of scalar multiplication (i.e. scale a vector)
        /// </summary>
        /// <param name="v">Vector to be scaled</param>
        /// <param name="s">Scaling factor</param>
        /// <param name="result">Vector that receives the result</param>
        /// <returns>Scaled vector a * s</returns>
        public static IVector3 Mul(this IVector3 v, float s, ref IVector3 result)
        {
            result[0] = v[0] * s;
            result[1] = v[1] * s;
            result[2] = v[2] * s;
            return result;
        }

        /// <summary>
        /// Return the result of an element-wise division by a scalar
        /// </summary>
        /// <param name="v">Vector to be element-wise divided by s</param>
        /// <param name="s">Scalar divisor</param>
        /// <param name="result">Vector that receives the result</param>
        /// <returns>Scaled vector a * 1/s</returns>
        public static IVector3 Div(this IVector3 v, float s, ref IVector3 result)
        {
            return v.Mul(1 / s, ref result);
        }

        /// <summary>
        /// Return the dot (inner-)product of two vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <returns>Dot product a.b</returns>
        public static float Dot(this IVector3 a, IVector3 b)
        {
            return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
        }

        /// <summary>
        /// Normalize a vector
        /// </summary>
        /// <param name="v">Vector to be normalized</param>
        /// <returns>Normalized result vector a/|a|</returns>
        public static IVector3 Normalize(this IVector3 v)
        {
            return v.Normalize(ref v);
        }

        /// <summary>
        /// Normalize a vector
        /// </summary>
        /// <param name="v">Vector to be normalized</param>
        /// <param name="norm">Normalised vector</param>
        /// <returns>Normalized result vector a/|a|</returns>
        public static IVector3 Normalize(this IVector3 v, ref IVector3 norm)
        {
            return v.Div(v.Magnitude(), ref norm);
        }

        /// <summary>
        /// Return the magnitude of a vector
        /// </summary>
        /// <param name="v">Vector</param>
        /// <returns>Magnitude of the vector |v|</returns>
        public static float Magnitude(this IVector3 v)
        {
            return (float)Math.Sqrt(v.Dot(v));
        }
    }
}
