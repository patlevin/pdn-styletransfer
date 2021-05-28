// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin
using Microsoft.ML.OnnxRuntime.Tensors;
using PaintDotNet.Effects.ML.StyleTransfer.Maths;

namespace PaintDotNet.Effects.ML.StyleTransfer.Color
{
    /// <summary>
    /// Base class for methods using linear color transfer.
    /// </summary>
    public abstract class LinearTransfer : ColorTransfer
    {
        /// <inheritdoc/>
        protected sealed override bool DoTransferColor(Tensor<float> source, Tensor<float> target, Tensor<float> output)
        {
            var sourceMean = PixelOps.Mean(source);
            var targetMean = PixelOps.Mean(target);

            var sourceSigma = PixelOps.Covariance(source, sourceMean);
            var targetSigma = PixelOps.Covariance(target, targetMean);

            if (GetCoefficients(sourceSigma, targetSigma, out Matrix3 A))
            {
                var b = Vector3.Zero;
                _ = sourceMean.Sub(A._ * targetMean, ref b);
                PixelOps.LinearTransfer(A, b, target, output);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculate the coefficient matrix for linear transfers of the form Ax + b
        /// </summary>
        /// <param name="sourceSigma">Covariance matrix of the source image</param>
        /// <param name="targetSigma">Covariance matrix of the target image</param>
        /// <param name="A">Coefficient matrix</param>
        /// <returns><c>true</c>, iff the coefficient matrix could be calculated</returns>
        protected abstract bool GetCoefficients(Matrix3 sourceSigma, Matrix3 targetSigma, out Matrix3 A);
    }
}
