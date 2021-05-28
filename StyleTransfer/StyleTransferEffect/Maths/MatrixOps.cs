using System;

namespace PaintDotNet.Effects.ML.StyleTransfer.Maths
{
    using Vec3 = ValueTuple<float, float, float>;

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
        /// <param name="C">Matrix that receives the sum</param>
        /// <returns>Sum of the matrixes: A + B</returns>
        public static Matrix3 Add(this Matrix3 A, Matrix3 B, Matrix3 C)
        {
            // performance fix: used in a per-pixel loop and unrolling and
            // forgoing the use of a lambda helps greatly with performance
            var a = A.m;
            var b = B.m;
            var c = C.m;

            c[0] = a[0] + b[0];
            c[1] = a[1] + b[1];
            c[2] = a[2] + b[2];

            c[3] = a[3] + b[3];
            c[4] = a[4] + b[4];
            c[5] = a[5] + b[5];

            c[6] = a[6] + b[6];
            c[7] = a[7] + b[7];
            c[8] = a[8] + b[8];

            return C;
        }

        /// <summary>
        /// Return the difference of two matrixes
        /// </summary>
        /// <param name="A">Matrix to subtract from</param>
        /// <param name="B">Matrix to subtract</param>
        /// <param name="C">Matrix that receives the difference</param>
        /// <returns>Difference of the matrixes: A - B</returns>
        public static Matrix3 Sub(this Matrix3 A, Matrix3 B, Matrix3 C)
        {
            var a = A.m;
            var b = B.m;
            var c = C.m;

            c[0] = a[0] - b[0];
            c[1] = a[1] - b[1];
            c[2] = a[2] - b[2];

            c[3] = a[3] - b[3];
            c[4] = a[4] - b[4];
            c[5] = a[5] - b[5];

            c[6] = a[6] - b[6];
            c[7] = a[7] - b[7];
            c[8] = a[8] - b[8];

            return C;
        }

        /// <summary>
        /// Return the product of two matrixes
        /// </summary>
        /// <param name="A">Left hand side matrix</param>
        /// <param name="B">Right hand side matrix</param>
        /// <param name="C">Matrix that receives the product of A and B</param>
        /// <returns>The matrix product of A amd B: A * B</returns>
        public static Matrix3 Mul(this Matrix3 A, Matrix3 B, Matrix3 C)
        {
            var a = A.m;
            var b = B.m;
            var c = C.m;

            c[0] = a[0] * b[0] + a[1] * b[3] + a[2] * b[6];
            c[1] = a[0] * b[1] + a[1] * b[4] + a[2] * b[7];
            c[2] = a[0] * b[2] + a[1] * b[5] + a[2] * b[8];
  
            c[3] = a[3] * b[0] + a[4] * b[3] + a[5] * b[6];
            c[4] = a[3] * b[1] + a[4] * b[4] + a[5] * b[7];
            c[5] = a[3] * b[2] + a[4] * b[5] + a[5] * b[8];

            c[6] = a[6] * b[0] + a[7] * b[3] + a[8] * b[6];
            c[7] = a[6] * b[1] + a[7] * b[4] + a[8] * b[7];
            c[8] = a[6] * b[2] + a[7] * b[5] + a[8] * b[8];

            return C;
        }

        /// <summary>
        /// Return the product of a matrix and a column vector
        /// </summary>
        /// <param name="A">Left hand side matrix</param>
        /// <param name="x1">First column vector component</param>
        /// <param name="x2">Second column vector component</param>
        /// <param name="x3">Third column vector component</param>
        /// <returns>Transformed vector components: A * x</returns>
        public static Vec3 Mul(this Matrix3 A, float x1, float x2, float x3)
        {
            // modified after profiling: runs inside a per-pixel loop and
            // to forgo IVector arguments improved performance significantly
            var a = A.m;
            return (a[0] * x1 + a[1] * x2 + a[2] * x3,
                    a[3] * x1 + a[4] * x2 + a[5] * x3,
                    a[6] * x1 + a[7] * x2 + a[8] * x3);
        }

        /// <summary>
        /// Return the element-wise product of a matrix and a scalar
        /// </summary>
        /// <param name="A">Matrix to be multiplied</param>
        /// <param name="b">Scalar to multiply each element by</param>
        /// <param name="C">Matrix that receives the element-wise product</param>
        /// <returns>Element-wise product: A * b</returns>
        public static Matrix3 Mul(this Matrix3 A, float b, Matrix3 C)
        {
            var a = A.m;
            var c = C.m;

            c[0] = a[0] * b;
            c[1] = a[1] * b;
            c[2] = a[2] * b;

            c[3] = a[3] * b;
            c[4] = a[4] * b;
            c[5] = a[5] * b;

            c[6] = a[6] * b;
            c[7] = a[7] * b;
            c[8] = a[8] * b;

            return C;
        }

        /// <summary>
        /// Element-wise division by a scalar
        /// </summary>
        public static Matrix3 Div(this Matrix3 A, float b, Matrix3 C)
        {
            return Mul(A, 1 / b, C);
        }

        /// <summary>
        /// Return the element-wise negated matrix
        /// </summary>
        /// <param name="A">Matrix to be negated</param>
        /// <param name="C">Matrix that receives the result</param>
        /// <returns>Negated input matrix</returns>
        public static Matrix3 Neg(this Matrix3 A, Matrix3 C)
        {
            var a = A.m;
            var c = C.m;

            c[0] = -a[0];
            c[1] = -a[1];
            c[2] = -a[2];

            c[3] = -a[3];
            c[4] = -a[4];
            c[5] = -a[5];

            c[6] = -a[6];
            c[7] = -a[7];
            c[8] = -a[8];

            return C;
        }

        /// <summary>
        /// Set matrix content to col•col.T
        /// </summary>
        /// <param name="M">Matrix to be updated</param>
        /// <param name="a">First column vector element</param>
        /// <param name="b">Second column vector element</param>
        /// <param name="c">Third column vector element</param>
        /// <returns>Updated matrix</returns>
        public static Matrix3 FromVector(this Matrix3 M, float a, float b, float c)
        {
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
    }
}
