using System;

namespace PaintDotNet.Effects.ML.StyleTransfer.Maths
{
    /// <summary>
    /// IVector3 operations extension class.
    /// </summary>
    /// <remarks>
    /// The design of x.Op(..., ref result) supports chaining operations while
    /// at the same time offering clear semantics.
    /// 
    /// Stand-alone vectors are supported just like row- or column vectors in
    /// matrixes. One issue with operator overloading is unnecessary copies
    /// and unclear semantics: how would one implement
    /// A.Row(0) = A.Col(1) + A.Col(2) in terms of operator overloading without
    /// copies?
    /// </remarks>
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
        /// Copy vector elements from an array
        /// </summary>
        /// <typeparam name="T">Type of the array elements</typeparam>
        /// <param name="v">Vector to copy elements to</param>
        /// <param name="array">Array to copy vector elements from</param>
        /// <param name="startAt">Index of the first vector element in the array</param>
        public static void CopyFrom<T>(this IVector3 v, T[] array, int startAt) where T : IConvertible
        {
            v[0] = array[startAt + 0].ToSingle(null);
            v[1] = array[startAt + 1].ToSingle(null);
            v[2] = array[startAt + 2].ToSingle(null);
        }

        public static void CopyTo(this IVector3 v, float[] array, int targetIndex)
        {
            array[targetIndex + 0] = v[0];
            array[targetIndex + 1] = v[1];
            array[targetIndex + 2] = v[2];
        }

        public static void CopyTo(this IVector3 v, int[] array, int targetIndex)
        {
            array[targetIndex + 0] = (int)v[0];
            array[targetIndex + 1] = (int)v[1];
            array[targetIndex + 2] = (int)v[2];
        }

        public static void CopyTo(this IVector3 v, byte[] array, int targetIndex)
        {
            var (a, b, c) = v;
            array[targetIndex + 0] = (byte)(a < 0 ? 0 : a > 255 ? 255 : a);
            array[targetIndex + 1] = (byte)(b < 0 ? 0 : b > 255 ? 255 : b);
            array[targetIndex + 2] = (byte)(c < 0 ? 0 : c > 255 ? 255 : c);
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
        /// Add a vector.
        /// </summary>
        /// <param name="a">Left hand vector</param>
        /// <param name="b">Vector to be added</param>
        /// <param name="result">Vector that receives the result</param>
        /// <returns>Sum vector a+b</returns>
        public static IVector3 Add(this IVector3 a, IVector3 b, ref IVector3 result)
        {
            result[0] = a[0] + b[0];
            result[1] = a[1] + b[1];
            result[2] = a[2] + b[2];
            return result;
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
        /// Return the cross (outer-)product of two vectors
        /// </summary>
        /// <param name="a">First vector</param>
        /// <param name="b">Second vector</param>
        /// <param name="result">Vector that receives the result</param>
        /// <returns>Result vector a x b</returns>
        public static IVector3 Cross(this IVector3 a, IVector3 b, ref IVector3 result)
        {
            var (a1, a2, a3) = a;
            var (b1, b2, b3) = b;
            result[0] = a2 * b3 - a3 * b2;
            result[1] = a3 * b1 - a1 * b3;
            result[2] = a1 * b2 - a2 * b1;
            return result;
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
