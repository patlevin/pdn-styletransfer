namespace PaintDotNet.Effects.ML.StyleTransfer.Maths
{
    /// <summary>
    /// Matrix operator overloads
    /// </summary>
    public struct Matrix3Wrapper
    {
        public Matrix3Wrapper(Matrix3 M)
        {
            wrapped = M;
        }

        public static Matrix3 operator -(Matrix3Wrapper A, Matrix3 B)
        {
            var C = Matrix3.Zero;
            return A.wrapped.Sub(B, C);
        }

        public static Matrix3 operator *(Matrix3Wrapper A, Matrix3 B)
        {
            var C = Matrix3.Zero;
            return A.wrapped.Mul(B, C);
        }

        public static IVector3 operator *(Matrix3Wrapper A, IVector3 x)
        {
            var (y1, y2, y3) = A.wrapped.Mul(x[0], x[1], x[2]);
            return new Vector3(y1, y2, y3);
        }

        public static Matrix3 operator /(Matrix3Wrapper A, float b)
        {
            var C = Matrix3.Zero;
            return A.wrapped.Div(b, C);
        }

        private readonly Matrix3 wrapped;
    }
}
