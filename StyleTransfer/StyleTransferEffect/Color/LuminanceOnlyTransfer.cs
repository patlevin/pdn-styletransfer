// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Color
{
    [TransferMethod(
        name: "Luma-only Transfer",
        description: "Only transfer luminosity and copy colour channels as-is")]
    public sealed class LuminanceOnlyTransfer : ColorTransfer
    {
        /// <inheritdoc/>
        protected override bool DoTransferColor(ImageData source, ImageData target, ImageData output)
        {
            var sampler = new Sampler(source);

            var targetData = target.Data;
            var outputData = output.Data;
            var fY = 1f / target.Height;
            var fX = 1f / target.Width;

            for (int row = 0, height = target.Height, k = 0; row < height; ++row)
            {
                for (int col = 0, width = target.Width; col < width; ++col, k += 4)
                {
                    var (r, g, b) = sampler[fX * col, fY * row];
                    PixelOps.RgbToYuv(r, g, b, out float _, out float u, out float v);
                    var y = PixelOps.Luma(targetData, k);
                    PixelOps.YuvToRgb(y, u, v, outputData, k);
                    outputData[k + 3] = targetData[k + 3];
                }
            }

            return true;
        }
    }
}
