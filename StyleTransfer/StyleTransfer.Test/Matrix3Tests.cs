using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaintDotNet.Effects.ML.StyleTransfer.Maths;

namespace PaintDotNet.Effects.ML.StyleTransfer.Test
{
    [TestClass]
    public class Matrix3Tests
    {
        [TestMethod("Diag returns diagonal matrix with given scalar in prime diagonal")]
        public void TestDiagScalar()
        {
            var expected = new Matrix3(
                new float[] { 79, 0, 0, 0, 79, 0, 0, 0, 79 });

            Assert.AreEqual(expected, Matrix3.Diag(79));
        }

        [TestMethod("Diag returns diagonal matrix with given vector")]
        public void TestDiagVector()
        {
            var expected = new Matrix3(
                new float[] { 1, 0, 0, 0, 2, 0, 0, 0, 3 });

            Assert.AreEqual(expected, Matrix3.Diag(new Vector3(1, 2, 3)));
        }

        [TestMethod("Indexing individual elements returns the correct value")]
        public void TestElementIndexingGetter()
        {
            var M = new Matrix3(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            Assert.AreEqual(1, M[0, 0]);
            Assert.AreEqual(5, M[1, 1]);
            Assert.AreEqual(9, M[2, 2]);
        }

        [TestMethod("Indexing individual elements sets the correct value")]
        public void TestElementIndexingSetter()
        {
            var M = new Matrix3(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            M[0, 0] = -1;
            M[1, 1] = -5;
            M[2, 2] = -9;

            Assert.AreEqual(-1, M[0, 0]);
            Assert.AreEqual(-5, M[1, 1]);
            Assert.AreEqual(-9, M[2, 2]);
        }

        [TestMethod("Indexing row returns the correct row vector")]
        public void TestRowIndexingGetter()
        {
            var M = new Matrix3(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            Assert.AreEqual(new Vector3(1, 2, 3), M[0]);
            Assert.AreEqual(new Vector3(4, 5, 6), M[1]);
            Assert.AreEqual(new Vector3(7, 8, 9), M[2]);
        }

        [TestMethod("Indexing row sets the correct row vector")]
        public void TestRowIndexingSetter()
        {
            var M = new Matrix3(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            M[0] = new Vector3(-1, -2, -3);
            M[2] = new Vector3(-7, -8, -9);

            Assert.AreEqual(new Vector3(-1, -2, -3), M[0]);
            Assert.AreEqual(new Vector3(4, 5, 6), M[1]);
            Assert.AreEqual(new Vector3(-7, -8, -9), M[2]);
        }

        [TestMethod("Indexing column returns the correct column vector")]
        public void TestColumnIndexingGetter()
        {
            var M = new Matrix3(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            Assert.AreEqual(new Vector3(1, 4, 7), M[0.Col()]);
            Assert.AreEqual(new Vector3(2, 5, 8), M[1.Col()]);
            Assert.AreEqual(new Vector3(3, 6, 9), M[2.Col()]);
        }

        [TestMethod("Indexing row sets the correct row vector")]
        public void TestColumnIndexingSetter()
        {
            var M = new Matrix3(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            M[0.Col()] = new Vector3(-1, -4, -7);
            M[2.Col()] = new Vector3(-3, -6, -9);

            Assert.AreEqual(new Vector3(-1, -4, -7), M[0.Col()]);
            Assert.AreEqual(new Vector3(2, 5, 8), M[1.Col()]);
            Assert.AreEqual(new Vector3(-3, -6, -9), M[2.Col()]);
        }

        [TestMethod("Trace returns tr(M)")]
        public void TestTrace()
        {
            var M = new Matrix3(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            Assert.AreEqual(15, M.Trace);
        }

        [TestMethod("T property returns transposed matrix")]
        public void TestTranspose()
        {
            var M = new Matrix3(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var Transposed = new Matrix3(new float[] { 1, 4, 7, 2, 5, 8, 3, 6, 9 });

            Assert.AreEqual(Transposed, M.T);
        }

        [TestMethod("SetAll sets all matrix elements to given value")]
        public void TestSetAll()
        {
            var M = new Matrix3(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            M.SetAll(216);
            for (int i = 0; i < Matrix3.N; ++i)
                for (int j = 0; j < Matrix3.N; ++j)
                    Assert.AreEqual(216, M[i, j]);
        }
    }
}
