using System.Collections;
using System.Collections.Generic;

namespace PaintDotNet.Effects.ML.StyleTransfer.Maths
{
    /// <summary>
    /// Some single precision maths functions and extension functions
    /// </summary>
    public static class MathF
    {
        public const float Epsilon = 9e-8f;

        /// <summary>
        /// Return the square of x
        /// </summary>
        public static float Sqr(this float x)
        {
            return x * x;
        }

        /// <summary>
        /// Return the cube of x
        /// </summary>
        public static float Cube(this float x)
        {
            return x * x * x;
        }

        /// <summary>
        /// Return the principal cube root of x
        /// </summary>
        /// <param name="x">Value to get the cube root of</param>
        /// <returns>Principal cube root</returns>
        public static float Cbrt(float x)
        {
            return x > 0 ? Cbrt1(x) :
                   x < 0 ? -Cbrt1(-x) :
                   0f;
        }

        /// <summary>
        /// Round the number to zero if it's lower than a given epsilon.
        /// </summary>
        /// <param name="x">Number to be rounded</param>
        /// <param name="epsilon">Threshold for rounding to zero</param>
        /// <returns>0, iff |x| < epsilon</returns>
        public static float RoundToZero(this float x, float epsilon = Epsilon)
        {
            return System.Math.Abs(x) < epsilon ? 0 : x;
        }

        /// <summary>
        /// Return whether two floats are almost equal
        /// </summary>
        /// <param name="a">Floating point value</param>
        /// <param name="b">Value to compare with</param>
        /// <param name="epsilon">Tolerance value</param>
        /// <returns><c>true</c>, iff |a-b| < epsilon</returns>
        public static bool AlmostEqual(this float a, float b, float epsilon = Epsilon)
        {
            return System.Math.Abs(a - b) < epsilon;
        }

        /// <summary>
        /// Return an epsilon-based equality comparer
        /// </summary>
        public static IEqualityComparer DefaultComparer => defaultComparer;

        /// <summary>
        /// Return an epsilon-based equality comparer
        /// </summary>
        /// <param name="epsilon">Tolerance for equality tests</param>
        public static IEqualityComparer Comparer(float epsilon)
        {
            return new EpsilonComparer(epsilon);
        }

        /// <summary>
        /// Return a comparer that uses relative error
        /// </summary>
        /// <param name="maxError">Maximum relative error</param>
        /// <returns>Equality comparer</returns>
        public static IEqualityComparer RelativeComparer(float maxError)
        {
            return new RelativeErrorComparer(maxError);
        }

        private static float Cbrt1(float x)
        {
            var s = 1f;

            while (x < 1f)
            {
                x *= 8f;
                s *= 0.5f;
            }

            while (x > 8f)
            {
                x *= 0.125f;
                s *= 2f;
            }

            var r = 1.5f;
            r -= 1f / 3f * (r - x / r.Sqr());
            r -= 1f / 3f * (r - x / r.Sqr());
            r -= 1f / 3f * (r - x / r.Sqr());
            r -= 1f / 3f * (r - x / r.Sqr());
            r -= 1f / 3f * (r - x / r.Sqr());
            r -= 1f / 3f * (r - x / r.Sqr());

            return r * s;
        }

        private sealed class EpsilonComparer : IEqualityComparer
        {
            public EpsilonComparer(float eps)
            {
                epsilon = eps;
            }

            public new bool Equals(object x, object y)
            {
                if (x is float single)
                    return single.AlmostEqual((float)y, epsilon);
                return EqualityComparer<object>.Default.Equals(x, y);
            }

            public int GetHashCode(object obj)
            {
                return EqualityComparer<object>.Default.GetHashCode(obj);
            }

            private readonly float epsilon;
        }

        private sealed class RelativeErrorComparer : IEqualityComparer
        {
            public RelativeErrorComparer(float maxError)
            {
                err = maxError;
            }

            public new bool Equals(object x, object y)
            {
                if (x is float a)
                {
                    if (a.RoundToZero() == 0)
                        return System.Math.Abs(a - (float)y) < err;
                    return System.Math.Abs(((float)y - a) / a) < err; 
                }
                return EqualityComparer<object>.Default.Equals(x, y);
            }

            public int GetHashCode(object obj)
            {
                return EqualityComparer<object>.Default.GetHashCode(obj);
            }

            private readonly float err;
        }

        private static readonly IEqualityComparer defaultComparer
            = new EpsilonComparer(Epsilon);
    }
}
