using Microsoft.ML.OnnxRuntime.Tensors;

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    /// <summary>
    /// Content transformation model that applies a style to an image.
    /// </summary>
    public interface ITransformModel : IEffectModel
    {
        /// <summary>
        /// Run inference on the transform model to apply a style and return result.
        /// </summary>
        /// <param name="content">Content image to transform as normalised float RGB</param>
        /// <param name="styleVector">Style vector extracted from style image</param>
        /// <returns>Content image with new style applied as normalised RGB (NHWC-format)</returns>
        Tensor<float> Run(Tensor<float> content, Tensor<float> styleVector);
    }
}
