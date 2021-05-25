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

        public static Matrix3 operator +(Matrix3Wrapper A, Matrix3 B)
        {
            var C = Matrix3.Zero;
            return A.wrapped.Add(B, ref C);
        }

        public static Matrix3 operator +(Matrix3Wrapper A, float b)
        {
            var C = Matrix3.Zero;
            return A.wrapped.Add(b, ref C);
        }

        public static Matrix3 operator +(float a, Matrix3Wrapper B)
        {
            var C = Matrix3.Zero;
            return a.Add(B.wrapped, ref C);
        }

        public static Matrix3 operator -(Matrix3Wrapper A, Matrix3 B)
        {
            var C = Matrix3.Zero;
            return A.wrapped.Sub(B, ref C);
        }

        public static Matrix3 operator -(Matrix3Wrapper A, float b)
        {
            var C = Matrix3.Zero;
            return A.wrapped.Sub(b, ref C);
        }

        public static Matrix3 operator -(float a, Matrix3Wrapper B)
        {
            var C = Matrix3.Zero;
            return a.Sub(B.wrapped, ref C);
        }

        public static Matrix3 operator -(Matrix3Wrapper M)
        {
            var C = Matrix3.Zero;
            return M.wrapped.Neg(ref C);
        }

        public static Matrix3 operator *(Matrix3Wrapper A, Matrix3 B)
        {
            var C = Matrix3.Zero;
            return A.wrapped.Mul(B, ref C);
        }

        public static Matrix3 operator *(Matrix3Wrapper A, float b)
        {
            var C = Matrix3.Zero;
            return A.wrapped.Mul(b, ref C);
        }

        public static Matrix3 operator *(float a, Matrix3Wrapper B)
        {
            var C = Matrix3.Zero;
            return a.Mul(B.wrapped, ref C);
        }

        public static IVector3 operator *(Matrix3Wrapper A, IVector3 x)
        {
            var y = Vector3.Zero;
            return A.wrapped.Mul(x, ref y);
        }

        public static Matrix3 operator /(Matrix3Wrapper A, float b)
        {
            var C = Matrix3.Zero;
            return A.wrapped.Div(b, ref C);
        }

        public static Matrix3 operator /(float a, Matrix3Wrapper B)
        {
            var C = Matrix3.Zero;
            return a.Div(B.wrapped, ref C);
        }

        private readonly Matrix3 wrapped;
    }
}
