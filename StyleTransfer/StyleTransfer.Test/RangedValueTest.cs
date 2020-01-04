namespace PaintDotNet.Effects.ML.StyleTransfer.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Basic tests for the RangeValue class
    /// </summary>
    [TestClass]
    public class RangedValueTest
    {
        [TestMethod]
        public void AcceptsValidRange()
        {
            var rv = new RangedValue<int>(1, 0, 5);
            Assert.AreEqual(1, rv.Value);
            Assert.AreEqual(0, rv.Minimum);
            Assert.AreEqual(5, rv.Maximum);
        }

        [TestMethod]
        public void AcceptsSwappedRange()
        {
            var rv = new RangedValue<int>(1, 5, 0);
            Assert.AreEqual(1, rv.Value);
            Assert.AreEqual(0, rv.Minimum);
            Assert.AreEqual(5, rv.Maximum);
        }

        [TestMethod]
        public void CtorClampsValue()
        {
            var rv = new RangedValue<float>(0.0f, 1.5f, 2.5f);
            Assert.AreEqual(rv.Minimum, rv.Value);

            rv = new RangedValue<float>(3.0f, 1.5f, 2.5f);
            Assert.AreEqual(rv.Maximum, rv.Value);
        }

        [TestMethod]
        public void SetterClampsValue()
        {
            var rv = new RangedValue<string>("abc", "aaa", "ccc")
            {
                Value = "def"
            };
            Assert.AreEqual(rv.Maximum, rv.Value);

            rv.Value = "a";
            Assert.AreEqual(rv.Minimum, rv.Value);

            rv.Value = "bcd";
            Assert.AreEqual("bcd", rv.Value);
        }
    }
}
