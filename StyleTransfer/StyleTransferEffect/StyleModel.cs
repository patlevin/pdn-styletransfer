// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    /// <summary>
    /// Style Model implementation - reads the input parameter name 
    /// </summary>
    internal class StyleModel : EffectModel, IStyleModel
    {
        /// <inheritdoc/>
        public Tensor<float> Run(Tensor<float> styleImage)
        {
            var inputs = new NamedOnnxValue[]
            {
                NamedOnnxValue.CreateFromTensor(inputName, styleImage)
            };

            return Run(inputs);
        }

        // Extract input tensor name while verifying model parameters
        protected override void OnModelLoaded()
        {
            inputName = TensorBySize(new int[] { -1, -1, -1, 3 });
        }

        private string inputName;
    }
}
