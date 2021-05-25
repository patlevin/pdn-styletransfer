using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaintDotNet.Effects.ML.StyleTransfer.Maths;

namespace PaintDotNet.Effects.ML.StyleTransfer.Test
{
    [TestClass]
    public class MatrixOpsTest
    {
        [TestMethod("FromVector sets matrix to col•col.T")]
        public void TestFromVectorColumn()
        {
            var M = Matrix3.Zero;
            var v = new Vector3(2, 3, 4);
            var expected = new Matrix3(new float[]
            {
                4, 6, 8,
                6, 9, 12,
                8, 12, 16
            });

            Assert.AreEqual(expected, M.FromVector(v));
        }

        [TestMethod("FromVector sets matrix to col•row.T")]
        public void TestFromVectorColumnRow()
        {
            var M = Matrix3.Zero;
            var col = new Vector3(4, 5, 6);
            var row = new Vector3(1, 2, 3);
            var expected = new Matrix3(new float[]
            {
                4, 8, 12,
                5, 10, 15,
                6, 12, 18
            });

            Assert.AreEqual(expected, M.FromVector(col, row));
        }

        [TestMethod("Add matrix returns element-wise sum")]
        public void TestAddMatrix()
        {
            var A = new Matrix3(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var B = new Matrix3(new float[] { 10, 11, 12, 13, 14, 15, 16, 17, 18 });
            var Sum = new Matrix3(new float[] { 11, 13, 15, 17, 19, 21, 23, 25, 27 });

            Assert.AreEqual(Sum, A._ + B);
        }

        [TestMethod("Add scalar returns element-wise sum of scalar and matrix")]
        public void TestAddScalar()
        {
            var A = new Matrix3(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var Sum = new Matrix3(new float[] { 11, 12, 13, 14, 15, 16, 17, 18, 19 });

            Assert.AreEqual(Sum, A._ + 10);
            Assert.AreEqual(Sum, 10 + A._);
        }

        [TestMethod("Sub matrix returns element-wise difference")]
        public void TestSubMatrix()
        {
            var A = new Matrix3(new float[] {  2,  5,  8, 11, 14, 17, 20, 23, 26 });
            var B = new Matrix3(new float[] {  1,  2,  3,  4,  5,  6,  7,  8,  9 });
            var C = new Matrix3(new float[] {  1,  3,  5,  7,  9, 11, 13, 15, 17 });

            Assert.AreEqual(C, A._ - B);
        }

        [TestMethod("Sub scalar returns element-wise difference of scalar and matrix")]
        public void TestSubScalar()
        {
            var A = new Matrix3(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var C = new Matrix3(new float[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 });
            var D = new Matrix3(new float[] { 9, 8, 7, 6, 5, 4, 3, 2, 1 });

            Assert.AreEqual(C, A._ - 1);
            Assert.AreEqual(D, 10 - A._);
        }

        [TestMethod("Neg matrix returns element-wise negated matrix")]
        public void TestNeg()
        {
            var M = new Matrix3(new float[] { 1, -2, 3, -4, 5, -6, 7, -8, 9 });
            var Negated = new Matrix3(new float[] { -1, 2, -3, 4, -5, 6, -7, 8, -9 });

            Assert.AreEqual(Negated, -M._);
        }

        [TestMethod("Mul matrix returns result of matrix multiplication")]
        public void TestMulMatrix()
        {
            var A = new Matrix3(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var B = new Matrix3(new float[] { 10, -12, 13, 14, -15, 16, 17, -18, 19 });
            var Product = new Matrix3(new float[]
            {
                89, -96, 102,
                212, -231, 246,
                335, -366, 390
            });

            Assert.AreEqual(Product, A._ * B);
        }

        [TestMethod("Mul scalar returns element-wise product of matrix and scalar")]
        public void TestMulScalar()
        {
            var A = new Matrix3(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var b = 5;
            var Product = new Matrix3(new float[] { 5, 10, 15, 20, 25, 30, 35, 40, 45 });

            Assert.AreEqual(Product, A._ * b);
            Assert.AreEqual(Product, b * A._);
        }

        [TestMethod("Div scalar returns element-wise quotient of matrix and scalar")]
        public void TestDivScalar()
        {
            var A = new Matrix3(new float[] { 5, 10, 15, 20, 25, 30, 35, 40, 45 });
            var B = new Matrix3(new float[]
            {
                362880, 181440, 120960, 90720, 72576, 60480, 51840, 45360, 40320
            });
            var Quotient = new Matrix3(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            Assert.AreEqual(Quotient, A._ / 5);
            Assert.AreEqual(Quotient, 362880 / B._);
        }
    }
}
