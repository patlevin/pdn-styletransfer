using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaintDotNet.Effects.ML.StyleTransfer.Maths;

namespace PaintDotNet.Effects.ML.StyleTransfer.Test
{
    public static class Ext
    {
        public static Matrix3 T(this Matrix3 A)
        {
            var C = Matrix3.Zero;
            C.Row(0).Copy(A.Column(0));
            C.Row(1).Copy(A.Column(1));
            C.Row(2).Copy(A.Column(2));
            return C;
        }
    }

    [TestClass]
    public class LinAlgTests
    {
        [TestMethod("Cholesky decomposition returns success and lower triangular matrix")]
        public void TestCholesky()
        {
            var A = new Matrix3(new float[] { 25, 15, -5, 15, 18, 0, -5, 0, 11 });
            var L = new Matrix3(new float[] { 5, 0, 0, 3, 3, 0, -1, 1, 3 });

            Assert.IsTrue(LinAlg.Cholesky(A, out Matrix3 C));
            Assert.AreEqual(L, C);
            Assert.AreEqual(A, C._ * C.T());
        }

        [TestMethod("Cholesky decomposition returns false if matrix isn't positive definite")]
        public void TestCholeskyNonPositiveDefinite()
        {
            var A = new Matrix3(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            Assert.IsFalse(LinAlg.Cholesky(A, out Matrix3 C));
        }

        [TestMethod("Det returns matrix determinant |M|")]
        public void TestDet()
        {
            var A = new Matrix3(new float[] { 33, 105, 105, 10, 28, 30, -10, -60, -62 });

            Assert.AreEqual(2112, LinAlg.Det(A));
        }

        [TestMethod("Invert returns true and inverse matrix for invertible matrixes")]
        public void TestInvert()
        {
            var A = new Matrix3(new float[] { 2, 1, 0, 0, 2, 0, 2, 0, 1 });
            var Ai = new Matrix3(new float[] { 0.5f, -0.25f, 0, 0, 0.5f, 0, -1, 0.5f, 1 });

            Assert.IsTrue(LinAlg.Invert(A, out Matrix3 I));
            Assert.AreEqual(Ai, I);
        }


        [TestMethod("Invert returns false for non-invertible matrixes")]
        public void TestInvertNotInvertible()
        {
            var A = new Matrix3(new float[] { 2, 1, 0, 0, 2, 0, 4, 2, 0 });

            Assert.IsFalse(LinAlg.Invert(Matrix3.Zero, out Matrix3 I1));
            Assert.IsFalse(LinAlg.Invert(A, out Matrix3 I2));
        }

        [TestMethod("Poly returns the last three coefficients of the characteristical polynome")]
        public void TestPoly()
        {
            var A = new Matrix3(new float[] { 1, 1, 0, 2, 2, 0, 1, 2, -2 });

            Assert.AreEqual((-1, -6, 0), LinAlg.Poly(A));
        }

        [TestMethod("SolveCubic returns solutions of normalized cubic equation")]
        public void TestSolveCubic()
        {
            var root = LinAlg.SolveCubic(-1, -6, 0);

            Assert.AreEqual(3, root.Length);
            Assert.AreEqual((-2, 3, 0), (root[0], root[1], root[2]));
        }

        [TestMethod("Eigenvalues returns eigenvalues of a matrix")]
        public void TestEigenvalues()
        {
            var A = new Matrix3(new float[] { 1, 1, 0, 2, 2, 0, 1, 2, -2 });
            var Eigenvalues = new Vector3(-2, 3, 0);
            var e = LinAlg.Eigenvalues(A);
            Assert.AreEqual(Eigenvalues, new Vector3(e));
        }

        [TestMethod("Eigenvalues returns eigenvalues with multplicity > 1")]
        public void TestEigenvaluesMultiplicity()
        {
            var A = new Matrix3(new float[] { 5, -10, -5, 2, 14, 2, -4, -8, 6 });

            var lambda = LinAlg.Eigenvalues(A);
            Assert.AreEqual(5f, lambda[0]);
            Assert.AreEqual(10f, lambda[1]);
            if (lambda.Length > 2)
                Assert.AreEqual(10f, lambda[2]);
        }

        [TestMethod("Eigenvectors returns matrix of normalized column vectors")]
        public void TestEigenvectorsThreeEigenvalues()
        {
            var Sqrt6 = (float)System.Math.Sqrt(6);
            var A = new Matrix3(new float[] { 1, 1, 0, 2, 2, 0, 1, 2, -2 });
            var V = new Matrix3(new float[]
            {
                0, 1 / Sqrt6, -2 / 3f,
                0, 2 / Sqrt6, 2 / 3f,
                1, 1 / Sqrt6, 1 / 3f
            });

            var M = LinAlg.Eigenvectors(A, new float[] { -2, 3, 0 });
            Assert.AreEqual(V, M);
        }

        [TestMethod("Eigenvectors handles eigenvalues with multiplicity 2")]
        public void TestEigenvectorsTwoEigenvalues()
        {
            var Sqrt45 = (float)System.Math.Sqrt(45);
            var Sqrt5 = (float)System.Math.Sqrt(5);
            var Sqrt2 = (float)System.Math.Sqrt(2);
            var A = new Matrix3(new float[] { 5, -10, -5, 2, 14, 2, -4, -8, 6 });
            var V = new Matrix3(new float[]
            {
                 5 / Sqrt45, -2 / Sqrt5, -1 / Sqrt2,
                -2 / Sqrt45,  1 / Sqrt5, 0,
                 4 / Sqrt45,  0, 1 / Sqrt2
            });

            var M = LinAlg.Eigenvectors(A, new float[] { 5, 10 }, out Matrix3 L);
            Assert.AreEqual(V, M);
            Assert.AreEqual(5f, L[0, 0]);
            Assert.AreEqual(10f, L[1, 1]);
            Assert.AreEqual(10f, L[2, 2]);
        }

        [TestMethod("Eigenvectors handles eigenvalues of sparse matrix")]
        public void TestEigenvectorsSparse()
        {
            var Sqrt6 = (float)System.Math.Sqrt(6);
            var Sqrt2 = (float)System.Math.Sqrt(2);
            var A = new Matrix3(new float[] { 3, -1, 0, 2, 0, 0, -2, 2, -1 });
            var V = new Matrix3(new float[]
            {
                0, 1 / Sqrt6, 1 / Sqrt2,
                0, 2 / Sqrt6, 1 / Sqrt2,
                1, 1 / Sqrt6, 0,
            });

            var M = LinAlg.Eigenvectors(A, new float[] { -1, 1, 2 });
            Assert.AreEqual(V, M);
        }

        [TestMethod("ApplyFunction fn() calculates V•fn(diag(L))•V*")]
        public void TestApplyFunction()
        {
            var A = new Matrix3(new float[] { 5, -10, -5, 2, 14, 2, -4, -8, 6 });
            var E = new Matrix3(new float[]
            {
                3125, -193750, -96875,
                38750, 177500, 38750,
                -77500, -155000, 22500
            });

            var success = LinAlg.ApplyFunction(A, x => (float)System.Math.Pow(x, 5), out Matrix3 C);

            Assert.IsTrue(success);
            Assert.IsTrue(E.Equals(C, MathF.RelativeComparer(0.00001f)));
        }
    }
}
