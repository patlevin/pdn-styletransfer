using Microsoft.ML.OnnxRuntime.Tensors;

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    /// <summary>
    /// Style extraction model.
    /// </summary>
    public interface IStyleModel : IEffectModel
    {
        /// <summary>
        /// Run inference on style extraction model and return style vector.
        /// </summary>
        /// <param name="styleImage">Normalised RGB image tensor in NHWC-format</param>
        /// <returns>Style vector extracted from image in NHWC-format (100 elements)</returns>
        Tensor<float> Run(Tensor<float> styleImage);
    }
}
