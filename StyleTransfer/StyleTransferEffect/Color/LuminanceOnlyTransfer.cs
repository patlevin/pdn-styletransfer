// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

using Microsoft.ML.OnnxRuntime.Tensors;
using System.Threading.Tasks;

namespace PaintDotNet.Effects.ML.StyleTransfer.Color
{
    [TransferMethod(
        name: "Luma-only Transfer",
        description: "Only transfer luminosity and copy colour channels as-is")]
    public sealed class LuminanceOnlyTransfer : ColorTransfer
    {
        /// <inheritdoc/>
        protected override bool DoTransferColor(Tensor<float> source, Tensor<float> target, Tensor<float> output)
        {

            var (width, height) = (target.Width(), target.Height());
            var fY = 1f / height;
            var fX = 1f / width;

            Parallel.For(0, height, row =>
            {
                var sampler = new Sampler<float>(source);

                var targetData = ((DenseTensor<float>)target).Buffer.Span;
                var outputData = ((DenseTensor<float>)output).Buffer.Span;
                var k = row * width * 3;
                var yPos = fY * row;
                for (int col = 0; col < width; ++col, k += 3)
                {
                    var (r, g, b) = sampler[fX * col, yPos];
                    var (_, u, v) = PixelOps.RgbToYuv(r, g, b);
                    var y = PixelOps.Luma(targetData, k);
                    PixelOps.YuvToRgb(y, u, v, outputData, k);
                }
            });

            return true;
        }
    }
}
