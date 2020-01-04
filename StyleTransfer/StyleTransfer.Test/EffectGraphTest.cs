namespace PaintDotNet.Effects.ML.StyleTransfer.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Drawing;

    [TestClass]
    public class EffectGraphTest
    {
        [TestMethod]
        public void StyleModelIsNotNull()
        {
            using (var graph = new EffectGraph())
            {
                Assert.IsNotNull(graph.Style);
            }
        }

        [TestMethod]
        public void TransformerModelIsNotNull()
        {
            using (var graph = new EffectGraph())
            {
                Assert.IsNotNull(graph.Transformer);
            }
        }

        [TestMethod]
        public void EffectParamsIsNotNull()
        {
            using (var graph = new EffectGraph())
            {
                Assert.IsNotNull(graph.Params);
            }
        }

        [
            TestMethod,
            Description("Some models (e.g. separable transformer) are very "
            + "picky with regards to the ONNX OpSet used - test for compatibility here")]
        public void CanRunCompatibleModel()
        {
            using (var graph = new EffectGraph())
            {
                var assembly = typeof(EffectGraphTest).Assembly;
                using (var stream = assembly.GetManifestResourceStream(GetType(), "resources.style_mobilenet.onnx"))
                {
                    var model = new byte[stream.Length];
                    stream.Read(model, 0, model.Length);
                    graph.Style.Load(model);
                }

                using (var stream = assembly.GetManifestResourceStream(GetType(), "resources.transformer_separable.onnx"))
                {
                    var model = new byte[stream.Length];
                    stream.Read(model, 0, model.Length);
                    graph.Transformer.Load(model);
                }

                using (var bitmap = new Bitmap(assembly.GetManifestResourceStream(GetType(), "resources.style.png")))
                {
                    graph.Params.Style = bitmap.ToTensor();
                }

                using (var bitmap = new Bitmap(assembly.GetManifestResourceStream(GetType(), "resources.content.png")))
                {
                    graph.Params.Content = bitmap.ToTensor();
                }

                var result = graph.Run();
                Assert.IsNotNull(result);
                Assert.AreEqual(graph.Params.Content.Length, result.Length);
                CollectionAssert.AreEqual(graph.Params.Content.Dimensions.ToArray(),
                    result.Dimensions.ToArray());
            }
        }
    }
}
