namespace PaintDotNet.Effects.ML.StyleTransfer.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;

    class ModelProvider : IModelProvider
    {
        public ModelType StyleType { get; set; }

        public ModelType TransformType { get; set; }

        public int ComputationDevice { get; set; }

        public IStyleModel Style => styleModel;

        public ITransformModel Transform => transformModel;

        public ModelProvider(bool load = false)
        {
            if (load)
            {
                styleModel.Load(LoadResource("resources.style_mobilenet.onnx"), -1);
                transformModel.Load(LoadResource("resources.transformer_separable.onnx"), -1);
            }
        }

        public void Dispose()
        {
            Style.Dispose();
            Transform.Dispose();
            GC.SuppressFinalize(this);
        }

        private byte[] LoadResource(string name)
        {
            var assembly = typeof(EffectGraphTest).Assembly;
            using (var stream = assembly.GetManifestResourceStream(GetType(), name))
            {
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                return data;
            }
        }

        private readonly IStyleModel styleModel = new StyleModel();
        private readonly ITransformModel transformModel = new TransformerModel();
    }

    [TestClass]
    public class EffectGraphTest
    {
        [TestMethod]
        public void EffectParamsIsNotNull()
        {
            using (var provider = new ModelProvider())
            {
                var graph = new EffectGraph(provider);
                Assert.IsNotNull(graph.Params);
            }
        }

        [
            TestMethod,
            Description("Some models (e.g. separable transformer) are very "
            + "picky with regards to the ONNX OpSet used - test for compatibility here")]
        public void CanRunCompatibleModel()
        {
            using (var provider = new ModelProvider(load: true))
            {
                var graph = new EffectGraph(provider);
                var assembly = typeof(EffectGraphTest).Assembly;

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
