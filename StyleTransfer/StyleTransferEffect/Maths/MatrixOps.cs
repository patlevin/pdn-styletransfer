using System;

namespace PaintDotNet.Effects.ML.StyleTransfer.Maths
{
    /// <summary>
    /// Matrix3 operations extension class.
    /// </summary>
    /// <remarks>
    /// Operator overloading is omitted for performance reasons.
    /// For convience, operation chaining is still supported, though.
    /// </remarks>
    public static class MatrixOps
    {
        /// <summary>
        /// Return an integer as a matrix column index
        /// </summary>
        /// <param name="i">Integer to be converted to a column index</param>
        /// <returns>Column index that matches the integer</returns>
        public static ColumnIndex Col(this int i)
        {
            return (ColumnIndex)i;
        }

        /// <summary>
        /// Return the sum of two matrixes
        /// </summary>
        /// <param name="A">Left hand side matrix</param>
        /// <param name="B">Right hand side matrix</param>
        /// <param name="result">Matrix that receives the sum</param>
        /// <returns>Sum of the matrixes: A + B</returns>
        public static Matrix3 Add(this Matrix3 A, Matrix3 B, ref Matrix3 result)
        {
            return Apply(A, B, (s, t) => s + t, ref result);
        }

        /// <summary>
        /// Return the element-wise sum of a matrix and a scalar
        /// </summary>
        /// <param name="A">Matrix to add to</param>
        /// <param name="b">Scalar to add to each element of A</param>
        /// <param name="result">Matrix that receives the sum</param>
        /// <returns>Element-wise sum of the matrix and the scalar: A + b</returns>
        public static Matrix3 Add(this Matrix3 A, float b, ref Matrix3 result)
        {
            return Apply(A, b, (s, t) => s + t, ref result);
        }

        /// <summary>
        /// Return the element-wise sum of a scalar and a matrix
        /// </summary>
        /// <param name="a">Scalar to add to each element of B</param>
        /// <param name="B">Matrix to add to</param>
        /// <param name="result">Matrix that receives the sum</param>
        /// <returns>Element-wise sum of the matrix and the scalar: a + B</returns>
        public static Matrix3 Add(this float a, Matrix3 B, ref Matrix3 result)
        {
            return Apply(a, B, (s, t) => s + t, ref result);
        }

        /// <summary>
        /// Return the difference of two matrixes
        /// </summary>
        /// <param name="A">Matrix to subtract from</param>
        /// <param name="B">Matrix to subtract</param>
        /// <param name="result">Matrix that receives the difference</param>
        /// <returns>Difference of the matrixes: A - B</returns>
        public static Matrix3 Sub(this Matrix3 A, Matrix3 B, ref Matrix3 result)
        {
            return Apply(A, B, (s, t) => s - t, ref result);
        }

        /// <summary>
        /// Return the element-wise difference of a scalar and a matrix
        /// </summary>
        /// <param name="A">Matrix to subtract the scalar from</param>
        /// <param name="b">Scalar to subtract from each element of A</param>
        /// <param name="result">Matrix that receives the difference</param>
        /// <returns>Element-wise difference of the matrix and the scalar: A - b</returns>
        public static Matrix3 Sub(this Matrix3 A, float b, ref Matrix3 result)
        {
            return Apply(A, b, (s, t) => s - t, ref result);
        }

        /// <summary>
        /// Return the element-wise difference of a scalar and a matrix
        /// </summary>
        /// <param name="a">Scalar to get the difference of</param>
        /// <param name="B">Matrix to be subtracted</param>
        /// <param name="result">Matrix that receives the difference</param>
        /// <returns>Element-wise difference of the scalar and the matrix: a - B</returns>
        public static Matrix3 Sub(this float a, Matrix3 B, ref Matrix3 result)
        {
            return Apply(a, B, (s, t) => s - t, ref result);
        }

        /// <summary>
        /// Return the product of two matrixes
        /// </summary>
        /// <param name="A">Left hand side matrix</param>
        /// <param name="B">Right hand side matrix</param>
        /// <param name="result">Matrix that receives the product of A and B</param>
        /// <returns>The matrix product of A amd B: A * B</returns>
        public static Matrix3 Mul(this Matrix3 A, Matrix3 B, ref Matrix3 result)
        {
            var a = A.m;
            var b = B.m;
            var c = result.m;

            c[0] = a[0] * b[0] + a[1] * b[3] + a[2] * b[6];
            c[1] = a[0] * b[1] + a[1] * b[4] + a[2] * b[7];
            c[2] = a[0] * b[2] + a[1] * b[5] + a[2] * b[8];
  
            c[3] = a[3] * b[0] + a[4] * b[3] + a[5] * b[6];
            c[4] = a[3] * b[1] + a[4] * b[4] + a[5] * b[7];
            c[5] = a[3] * b[2] + a[4] * b[5] + a[5] * b[8];

            c[6] = a[6] * b[0] + a[7] * b[3] + a[8] * b[6];
            c[7] = a[6] * b[1] + a[7] * b[4] + a[8] * b[7];
            c[8] = a[6] * b[2] + a[7] * b[5] + a[8] * b[8];

            return result;
        }

        /// <summary>
        /// Return the product of a matrix and a column vector
        /// </summary>
        /// <param name="A">Left hand side matrix</param>
        /// <param name="x">Column vector or transposed row vector</param>
        /// <param name="result">Vector that receives the product of A and x</param>
        /// <returns>Transformed vector: A * x</returns>
        public static IVector3 Mul(this Matrix3 A, IVector3 x, ref IVector3 result)
        {
            var a = A.m;
            var (x1, x2, x3) = x;

            result[0] = a[0] * x1 + a[1] * x2 + a[2] * x3;
            result[1] = a[3] * x1 + a[4] * x2 + a[5] * x3;
            result[2] = a[6] * x1 + a[7] * x2 + a[8] * x3;

            return result;
        }

        /// <summary>
        /// Return the element-wise product of a matrix and a scalar
        /// </summary>
        /// <param name="A">Matrix to be multiplied</param>
        /// <param name="b">Scalar to multiply each element by</param>
        /// <param name="result">Matrix that receives the element-wise product</param>
        /// <returns>Element-wise product: A * b</returns>
        public static Matrix3 Mul(this Matrix3 A, float b, ref Matrix3 result)
        {
            return Apply(A, b, (s, t) => s * t, ref result);
        }

        /// <summary>
        /// Return the element-wise product of a scalar and a matrix
        /// </summary>
        /// <param name="a">Scalar to multiply each element by</param>
        /// <param name="B">Matrix to be multiplied</param>
        /// <param name="result">Matrix that receives the element-wise product</param>
        /// <returns>Element-wise product: a * B</returns>
        public static Matrix3 Mul(this float a, Matrix3 B, ref Matrix3 result)
        {
            return Apply(a, B, (s, t) => s * t, ref result);
        }

        /// <summary>
        /// Element-wise division by a scalar
        /// </summary>
        public static Matrix3 Div(this Matrix3 A, float b, ref Matrix3 result)
        {
            return Mul(A, 1 / b, ref result);
        }

        /// <summary>
        /// Element-wise division by a matrix
        /// </summary>
        public static Matrix3 Div(this float a, Matrix3 B, ref Matrix3 result)
        {
            return Apply(a, B, (s, t) => s / t, ref result);
        }

        /// <summary>
        /// Return the element-wise negated matrix
        /// </summary>
        /// <param name="M">Matrix to be negated</param>
        /// <param name="result">Matrix that receives the result</param>
        /// <returns>Negated input matrix</returns>
        public static Matrix3 Neg(this Matrix3 M, ref Matrix3 result)
        {
            var m = M.m;
            var r = result.m;

            r[0] = -m[0];
            r[1] = -m[1];
            r[2] = -m[2];

            r[3] = -m[3];
            r[4] = -m[4];
            r[5] = -m[5];

            r[6] = -m[6];
            r[7] = -m[7];
            r[8] = -m[8];

            return result;
        }

        /// <summary>
        /// Set matrix content to col•col.T
        /// </summary>
        /// <param name="M">Matrix to be updated</param>
        /// <param name="col">Column vector</param>
        /// <returns>Updated matrix</returns>
        public static Matrix3 FromVector(this Matrix3 M, IVector3 col)
        {
            var (a, b, c) = col;
            var ab = a * b;
            var ac = a * c;
            var bc = b * c;
            var m = M.m;

            m[0] = a * a;
            m[1] = ab;
            m[2] = ac;

            m[3] = ab;
            m[4] = b * b;
            m[5] = bc;

            m[6] = ac;
            m[7] = bc;
            m[8] = c * c;

            return M;
        }

        /// <summary>
        /// Set matrix content to col•row
        /// </summary>
        /// <param name="M">Matrix to be updated</param>
        /// <param name="col">Column vector</param>
        /// <returns>Updated matrix</returns>
        public static Matrix3 FromVector(this Matrix3 M, IVector3 col, IVector3 row)
        {
            var (a, b, c) = col;
            var (d, e, f) = row;
            var m = M.m;

            m[0] = d * a;
            m[1] = e * a;
            m[2] = f * a;

            m[3] = d * b;
            m[4] = e * b;
            m[5] = f * b;

            m[6] = d * c;
            m[7] = e * c;
            m[8] = f * c;

            return M;
        }

        private static Matrix3 Apply(Matrix3 A, Matrix3 B, Func<float, float, float> op, ref Matrix3 C)
        {
            var p = A.m;
            var q = B.m;
            var r = C.m;

            r[0] = op(p[0], q[0]);
            r[1] = op(p[1], q[1]);
            r[2] = op(p[2], q[2]);

            r[3] = op(p[3], q[3]);
            r[4] = op(p[4], q[4]);
            r[5] = op(p[5], q[5]);

            r[6] = op(p[6], q[6]);
            r[7] = op(p[7], q[7]);
            r[8] = op(p[8], q[8]);

            return C;
        }

        private static Matrix3 Apply(Matrix3 A, float s, Func<float, float, float> op, ref Matrix3 B)
        {
            var p = A.m;
            var r = B.m;

            r[0] = op(p[0], s);
            r[1] = op(p[1], s);
            r[2] = op(p[2], s);

            r[3] = op(p[3], s);
            r[4] = op(p[4], s);
            r[5] = op(p[5], s);

            r[6] = op(p[6], s);
            r[7] = op(p[7], s);
            r[8] = op(p[8], s);

            return B;
        }

        private static Matrix3 Apply(float s, Matrix3 A, Func<float, float, float> op, ref Matrix3 B)
        {
            var p = A.m;
            var r = B.m;

            r[0] = op(s, p[0]);
            r[1] = op(s, p[1]);
            r[2] = op(s, p[2]);

            r[3] = op(s, p[3]);
            r[4] = op(s, p[4]);
            r[5] = op(s, p[5]);

            r[6] = op(s, p[6]);
            r[7] = op(s, p[7]);
            r[8] = op(s, p[8]);

            return B;
        }
    }
}
