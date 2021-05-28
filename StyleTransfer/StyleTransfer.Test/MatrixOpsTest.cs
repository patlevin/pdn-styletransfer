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
            var expected = new Matrix3(new float[]
            {
                4, 6, 8,
                6, 9, 12,
                8, 12, 16
            });

            Assert.AreEqual(expected, Matrix3.Zero.FromVector(2, 3, 4));
        }

        [TestMethod("Sub matrix returns element-wise difference")]
        public void TestSubMatrix()
        {
            var A = new Matrix3(new float[] {  2,  5,  8, 11, 14, 17, 20, 23, 26 });
            var B = new Matrix3(new float[] {  1,  2,  3,  4,  5,  6,  7,  8,  9 });
            var C = new Matrix3(new float[] {  1,  3,  5,  7,  9, 11, 13, 15, 17 });

            Assert.AreEqual(C, A._ - B);
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

        [TestMethod("Div scalar returns element-wise quotient of matrix and scalar")]
        public void TestDivScalar()
        {
            var A = new Matrix3(new float[] { 5, 10, 15, 20, 25, 30, 35, 40, 45 });
            var Quotient = new Matrix3(new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

            Assert.AreEqual(Quotient, A._ / 5);
        }
    }
}
