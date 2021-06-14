// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    using Microsoft.ML.OnnxRuntime;
    using Microsoft.ML.OnnxRuntime.Tensors;

    /// <summary>
    /// Transformer Model implementation - gets the input parameter names
    /// </summary>
    internal class TransformerModel : EffectModel, ITransformModel
    {
        /// <inheritdoc/>
        public Tensor<float> Run(Tensor<float> content, Tensor<float> styleVector)
        {
            var inputs = new NamedOnnxValue[]
            {
                NamedOnnxValue.CreateFromTensor(contentName, content),
                NamedOnnxValue.CreateFromTensor(styleName, styleVector)
            };

            return Run(inputs);
        }

        // Find content- and style input tensor names
        protected override void OnModelLoaded()
        {
            contentName = TensorBySize(new int[] { -1, -1, -1, 3 });
            styleName = TensorBySize(new int[] { -1, 1, 1, 100 });
        }

        private string contentName;

        private string styleName;
    }
}
