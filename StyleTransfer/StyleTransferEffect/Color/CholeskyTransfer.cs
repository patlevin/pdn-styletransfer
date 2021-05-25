// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin
using PaintDotNet.Effects.ML.StyleTransfer.Maths;

namespace PaintDotNet.Effects.ML.StyleTransfer.Color
{
    /// <summary>
    /// Linear colour histrogram matching based on Cholesky decomposition.
    /// </summary>
    /// <remarks>
    /// This linear histrogram maching method uses the Cholesky decomposition
    /// of the source- and target image covariance matrixes.
    /// The method was proposed by Gatys et al. [1] and uses a linear transfer
    /// y = A•x + b to perform the colour mapping.
    /// 
    /// The matrix A is obtained by using A = L<sub>S</sub>•L<sub>T</sub><sup>-1</sup>,
    /// where L is the Cholesky decomposition Σ = L•L<sup>T</sup> of the
    /// covariance matrix of the source- and target image respectively.
    /// 
    /// The vector b can be obtained using b = µ<sub>S</sub> - A•µ<sub>T</sub>. 
    /// µ<sub>S</sub> and µ<sub>T</sub> are the mean RGB vectors of the source
    /// and target images.
    /// 
    /// [1] Gatys, A., Bethge, M., Hertzmann, A., Shechtman, E.
    ///     "Preserving Color in Neural Artistic Style Transfer."
    ///     arXiv:1606.05897 [cs.CV], 2016
    /// </remarks>
    [TransferMethod(
        name: "Cholesky Transfer",
        description: "Linear colour transfer based on Cholesky-decomposition")]
    public sealed class CholeskyTransfer : LinearTransfer
    {
        /// <inheritdoc/>
        protected override bool GetCoefficients(Matrix3 sourceSigma, Matrix3 targetSigma, out Matrix3 A)
        {
            A = Matrix3.Zero;

            var sourceL = Matrix3.Zero;
            if (!LinAlg.Cholesky(sourceSigma, ref sourceL))
                return false;

            var targetL = Matrix3.Zero;
            if (!LinAlg.Cholesky(targetSigma, ref targetL))
                return false;

            var targetLI = Matrix3.Zero;
            if (!LinAlg.Invert(targetL, ref targetLI))
                return false;

            A = sourceL._ * targetLI;
            return true;
        }
    }
}
