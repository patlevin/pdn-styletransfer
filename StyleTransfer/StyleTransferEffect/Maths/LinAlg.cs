using System;
using System.Collections.Generic;

namespace PaintDotNet.Effects.ML.StyleTransfer.Maths
{
    /// <summary>
    /// Linear algebra methods and algorithms.
    /// </summary>
    public static class LinAlg
    {
        /// <summary>
        /// Perform a Cholesky decomposition on a matrix
        /// </summary>
        /// <param name="A">Matrix to be decomposed</param>
        /// <param name="L">Lower triangular matrix</param>
        /// <returns><c>true</c>, iff A is positive-definite</returns>
        public static bool Cholesky(Matrix3 A, ref Matrix3 L)
        {
            L.SetAll(0);
            L[0, 0] = (float)Math.Sqrt(A[0, 0]);

            if (L[0, 0] == 0)
                return false;

            L[1, 0] = 1f / L[0, 0] * A[1, 0];

            var sum = L[1, 0].Sqr();
            if (sum >= A[1, 1])
                return false;

            L[1, 1] = (float)Math.Sqrt(A[1, 1] - sum);
            L[2, 0] = 1f / L[0, 0] * A[2, 0];
            L[2, 1] = 1f / L[1, 1] * (A[2, 1] - L[2, 0] * L[1, 0]);

            sum = L[2, 0].Sqr() + L[2, 1].Sqr();
            if (sum >= A[2, 2])
                return false;

            L[2, 2] = (float)Math.Sqrt(A[2, 2] - sum);
            return true;
        }

        /// <summary>
        /// Return the determinant of a matrix
        /// </summary>
        /// <param name="A">Matrix to get the determinant from</param>
        /// <returns>Determinant |A|</returns>
        public static float Det(Matrix3 A)
        {
            float Det2(float a, float b, float c, float d) => a * d - b * c;

            return A[0, 0] * Det2(A[1, 1], A[1, 2], A[2, 1], A[2, 2]) -
                   A[0, 1] * Det2(A[1, 0], A[1, 2], A[2, 0], A[2, 2]) +
                   A[0, 2] * Det2(A[1, 0], A[1, 1], A[2, 0], A[2, 1]);
        }

        /// <summary>
        /// Calculate the inverse of a matrix
        /// </summary>
        /// <param name="A">(Hermetian) matrix</param>
        /// <param name="Inv">Receives the matrix inverse</param>
        /// <returns><>true</c>, iff A is inversible</returns>
        public static bool Invert(Matrix3 A, ref Matrix3 Inv)
        {
            Inv[0, 0] = A[1, 1] * A[2, 2] - A[1, 2] * A[2, 1];
            Inv[0, 1] = -(A[0, 1] * A[2, 2] - A[0, 2] * A[2, 1]);
            Inv[0, 2] = A[0, 1] * A[1, 2] - A[0, 2] * A[1, 1];

            Inv[1, 0] = -(A[1, 0] * A[2, 2] - A[1, 2] * A[2, 0]);
            Inv[1, 1] = A[0, 0] * A[2, 2] - A[0, 2] * A[2, 0];
            Inv[1, 2] = -(A[0, 0] * A[1, 2] - A[0, 2] * A[1, 0]);

            Inv[2, 0] = A[1, 0] * A[2, 1] - A[1, 1] * A[2, 0];
            Inv[2, 1] = -(A[0, 0] * A[2, 1] - A[0, 1] * A[2, 0]);
            Inv[2, 2] = A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0];

            var detA = A.Row(0).Dot(Inv.Column(0));
            if (Math.Abs(detA).RoundToZero() == 0)
                return false;

            _ = Inv.Div(detA, ref Inv);
            return true;
        }

        /// <summary>
        /// Calculate the characteristical polynome of a matrix 
        /// </summary>
        /// <param name="A">A Matrix</param>
        /// <returns>Coefficients a,b,c of the characteristical polynome l³+a*l²+b*l+c</returns>
        public static (float, float, float) Poly(Matrix3 A)
        {
            var tr = A.Trace;
            return (-tr, -0.5f * ((A._ * A).Trace - tr.Sqr()), -Det(A));
        }

        /// <summary>
        /// Solve a cubic equation of the form x³ + ax² + bx + c = 0
        /// </summary>
        /// <param name="a">Coefficient for x squared</param>
        /// <param name="b">Coefficient for x</param>
        /// <param name="c">Residual coefficient</param>
        /// <returns>Real roots of the cubic</returns>
        public static float[] SolveCubic(float a, float b, float c)
        {
            const float Eps = 1e-6f;
            var a2 = a * a;
            // convert x³ + ax²+ bx + c to x³ + qx + r
            var q = (a2 - 3 * b) / 9;
            var r = (a * (2 * a2 - 9 * b) + 27 * c) / 54;
            var r2 = r * r;
            // solve depressed cubic poly
            var q3 = q.Cube();
            if (r2 <= q3 + Eps)
            {
                // 3 real solutions
                var t = r / (float)Math.Sqrt(q3);
                t = (float)Math.Acos(t < -1f ? -1f : t > 1f ? 1f : t);
                a /= 3;
                q = -2 * (float)Math.Sqrt(q);
                return new float[]
                {
                    (q * (float)Math.Cos(t / 3) - a).RoundToZero(),
                    (q * (float)Math.Cos((t + 2 * Math.PI) / 3) - a).RoundToZero(),
                    (q * (float)Math.Cos((t - 2 * Math.PI) / 3) - a).RoundToZero()
                };
            }
            else
            {
                // one or two real roots
                var A = -MathF.Cbrt(Math.Abs(r) + (float)Math.Sqrt(r2 - q3));
                if (r < 0)
                    A = -A;

                var B = A == 0 ? 0 : q / A;
                a /= 3;
                var x1 = (A + B) - a;
                var x3 = 0.5f * (float)Math.Sqrt(3) * (A - B);

                return (Math.Abs(x3) < (float)Math.Sqrt(Eps))
                        ? new float[] { x1.RoundToZero(), (-0.5f * (A + B) - a).RoundToZero() }
                        : new float[] { x1.RoundToZero() };
            }
        }

        /// <summary>
        /// Calculate the eigenvalues of a matrix
        /// </summary>
        /// <param name="A">(Hermetian) matrix</param>
        /// <returns>Array of eigenvalues</returns>
        public static float[] Eigenvalues(Matrix3 A)
        {
            var (a, b, c) = Poly(A);
            return SolveCubic(a, b, c);
        }

        /// <summary>
        /// Calculate the eigenvectors of a matrix
        /// </summary>
        /// <param name="A">(Hermetian) matrix</param>
        /// <param name="eigenvalues">Eigenvalues of the matrix (may be updated)</param>
        /// <param name="L">Diagonal matrix with eigenvalues</param>
        /// <returns>Matrix containing the eigenvectors (as columns)</returns>
        public static Matrix3 Eigenvectors(Matrix3 A, float[] eigenvalues, out Matrix3 L)
        {
            var v = new List<IVector3>();
            var l = new List<float>();
            var prev = 0f;
            for (int i = 0; i < eigenvalues.Length; ++i)
            {
                // multiplicities are handled by back substitution, ignore
                if (eigenvalues[i] == prev)
                    continue;

                var E = Matrix3.Diag(eigenvalues[i]);
                var U = TriangularizeUpper(A._ - E);
                var multiplicity = BackSubstitution(U, v);
                // correlate eigenvectors with eigenvalue
                while (multiplicity-- > 0)
                    l.Add(eigenvalues[i]);

                prev = eigenvalues[i];
            }

            // no idea how to handle this correctly
            if (v.Count < 3)
            {
                L = null;
                return null;
            }

            var V = Matrix3.Zero;

            V[0.Col()] = v[0];
            V[1.Col()] = v[1];
            V[2.Col()] = v[2];

            L = Matrix3.Diag(new Vector3(l.ToArray()));

            return V;
        }

        /// <summary>
        /// Calculate the eigenvectors of a matrix
        /// </summary>
        /// <param name="A">(Hermetian) matrix</param>
        /// <param name="eigenvalues">Eigenvalues of the matrix (may be updated)</param>
        /// <returns>Matrix containing the eigenvectors (as columns)</returns>
        public static Matrix3 Eigenvectors(Matrix3 A, float[] eigenvalues)
        {
            return Eigenvectors(A, eigenvalues, out Matrix3 _);
        }

        /// <summary>
        /// Apply an arbitrary function to a matrix.
        /// </summary>
        /// <param name="M">Matrix to apply the function to</param>
        /// <param name="fn">Function to be applied</param>
        /// <param name="result">Matrix that receives the result</param>
        /// <returns><c>true</c>, iff the function could be applied</returns>
        public static bool ApplyFunction(Matrix3 M, Func<float, float> fn, ref Matrix3 result)
        {
            var lambda = Eigenvalues(M);
            var V = Eigenvectors(M, lambda, out Matrix3 L);
            if (V == null)
                return false;

            L[0, 0] = fn(L[0, 0]);
            L[1, 1] = fn(L[1, 1]);
            L[2, 2] = fn(L[2, 2]);

            var VInv = Matrix3.Zero;
            if (!Invert(V, ref VInv))
                throw new InvalidOperationException("non-invertible eigenbasis - numeric instability?");

            _ = (V._ * L).Mul(VInv, ref result);

            return true;
        }

        // run Gauss algorithm for converting A into an upper triangular matrix
        private static Matrix3 TriangularizeUpper(Matrix3 A)
        {
            var U = new Matrix3(A.m.CloneT());

            // replace zero on main diagonal
            bool EliminateZero(int i)
            {
                if (U[i, i] != 0)
                    return true;

                for (int j = i + 1; U[i, i] == 0 && j < Matrix3.N; ++j)
                    if (U[j, i] != 0)
                        U.Row(i).Swap(U.Row(j));

                return U[i, i] != 0;
            }

            void Reduce(int i)
            {
                for (int j = i + 1; j < Matrix3.N; ++j)
                {
                    var c = U[j, i] / U[i, i];
                    U[j, 0] -= c * U[i, 0];
                    U[j, 1] -= c * U[i, 1];
                    U[j, 2] -= c * U[i, 2];
                }
            }

            if (!EliminateZero(0))
                return U;

            Reduce(0);

            if (!EliminateZero(1))
                return U;

            Reduce(1);

            return U;
        }

        // hacky approach: try to detect free parameters and exploit the
        // particular failure cases of triangulisation; return the number
        // of found/selected solutions
        private static int BackSubstitution(Matrix3 U, IList<IVector3> v)
        {
            // no idea how to solve this
            if (U[0, 0].RoundToZero() == 0)
                return 0;

            if (U[1, 1].RoundToZero() == 0 && U[2, 2].RoundToZero() == 0)
            {
                // two free parameters s, t: w.l.o.g. use s=1, t=0 and s=0, t=1
                v.Add(new Vector3(-U[0, 1] / U[0, 0], 1, 0).Normalize());
                v.Add(new Vector3(-U[0, 2] / U[0, 0], 0, 1).Normalize());
                return 2;
            }

            if (U[1, 1].RoundToZero() == 0)
            {
                // second row must be zero -> z must be zero; choose y = 1
                v.Add(new Vector3(-U[0, 1] / U[0, 0], 1f, 0f).Normalize());
                return 1;
            }

            var x = Vector3.Zero;
            x[2] = U[2, 2].RoundToZero() != 0 ? U[2, 2] : 1;
            x[1] = -(U[1, 2] * x[2]) / U[1, 1];
            x[0] = -(U[0, 1] * x[1] + U[0, 2] * x[2]) / U[0, 0];
            v.Add(x.Normalize());

            return 1;
        }
    }
}
