using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaintDotNet.Effects.ML.StyleTransfer.Maths;

namespace PaintDotNet.Effects.ML.StyleTransfer.Test
{
    [TestClass]
    public class VectorOpsTests
    {
        [TestMethod("Deconstructuring extracts vector elements")]
        public void TestDeconstruct()
        {
            const float X = 1;
            const float Y = 2;
            const float Z = 3;

            var v = new Vector3(X, Y, Z);
            var (a, b, c) = v;

            Assert.AreEqual(X, a);
            Assert.AreEqual(Y, b);
            Assert.AreEqual(Z, c);
        }

        [TestMethod("Swap exchanges vector elements")]
        public void TestSwap()
        {
            var x = new Vector3(1, 2, 3);
            var y = new Vector3(4, 5, 6);

            x.Swap(y);

            Assert.AreEqual(new Vector3(4, 5, 6), x);
            Assert.AreEqual(new Vector3(1, 2, 3), y);
        }

        [TestMethod("Copy sets vector elements to contents of given vector")]
        public void TestCopy()
        {
            var x = new Vector3(1, 2, 3);
            var y = Vector3.Zero;

            y.Copy(x);

            Assert.AreEqual(x, y);
        }

        [TestMethod("Sub sets result vector to element-wise difference of arguments")]
        public void TestSub()
        {
            var a = new Vector3(1, 2, 3);
            var b = new Vector3(9, -4, -5);
            var c = Vector3.Zero;

            _ = a.Sub(b, ref c);

            Assert.AreEqual(new Vector3(-8, 6, 8), c);
        }

        [TestMethod("Mul sets result vector to element-wise product of arguments")]
        public void TestMul()
        {
            var a = new Vector3(1, 2, 3);
            var b = 4f;
            var c = Vector3.Zero;

            _ = a.Mul(b, ref c);

            Assert.AreEqual(new Vector3(4, 8, 12), c);
        }

        [TestMethod("Div sets result vector to element-wise quotient of arguments")]
        public void TestDiv()
        {
            var a = new Vector3(1, 2, 3);
            var b = 0.25f;
            var c = Vector3.Zero;

            _ = a.Div(b, ref c);

            Assert.AreEqual(new Vector3(4, 8, 12), c);
        }

        [TestMethod("Dot returns inner product of arguments")]
        public void TestDot()
        {
            const float AdotB = 15f;
            var a = new Vector3(1, 2, 3);
            var b = new Vector3(4, -5, 7);

            var dot = a.Dot(b);

            Assert.AreEqual(AdotB, dot);
        }

        [TestMethod("Magnitude returns Sqrt(v.v)")]
        public void TestMagnitude()
        {
            var v = new Vector3(3, 0, 4);
            Assert.AreEqual(5, v.Magnitude());
        }

        [TestMethod("Normalize scales vector to unit magnitude")]
        public void TestNormalize()
        {
            var v = new Vector3(3, 0, 4);

            v.Normalize();

            Assert.AreEqual(new Vector3(0.6f, 0, 0.8f), v);
        }
    }
}
