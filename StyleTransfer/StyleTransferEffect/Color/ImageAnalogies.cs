// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin
using System;
using PaintDotNet.Effects.ML.StyleTransfer.Maths;

namespace PaintDotNet.Effects.ML.StyleTransfer.Color
{
    /// <summary>
    /// Linear colour histrogram matching based on eigenvalue decomposition.
    /// </summary>
    /// <remarks>
    /// This linear transfer implements the linear colour histrogram matching
    /// method suggested by Hertzmann [1] in Appendix B. The idea is to match
    /// the mean and covariance of the source image.
    /// 
    /// The proposed linear transformation takes the shape y = A•x + b, where
    /// A is a matrix and b is an RGB vector. A is derived from the covariance
    /// matrixes of the source and target images:
    /// 
    /// A = Σ<sub>S</sub><sup>½</sup>•Σ<sub>T</sub><sup>-½</sup>
    /// 
    /// Σ<sub>S</sub> and Σ<sub>T</sub> denote the covariance matrixes of the
    /// source and target image respectively. The square root of the matrixes
    /// is calculated using the eigenvalue decomposition:
    /// 
    /// Σ = V•λ•V<sup>-1</sup>, where V is an eigenspace and λ is a diagonal
    /// matrix containing the eigenvalues of Σ.
    /// 
    /// Any algebraic function, can then be performed on the elements of the
    /// main diagonal λ and we define f(Σ) = V•f(λ)•V<sup>-1</sup>.
    /// 
    /// The vector b can be obtained using b = µ<sub>S</sub> - A•µ<sub>T</sub>. 
    /// µ<sub>S</sub> and µ<sub>T</sub> are the mean RGB vectors of the source
    /// and target images.
    /// 
    /// [1] Hertzmann, A. "Algorithms for Rendering in Artistic Styles."
    ///     PhD thesis, New York University, 2001.
    /// </remarks>
    [TransferMethod(
        name: "Image Analogies",
        description: "Linear colour transfer based on eigenvalue decomposition")]
    public sealed class ImageAnalogies : LinearTransfer
    {
        /// <inheritdoc/>
        protected override bool GetCoefficients(Matrix3 sourceSigma, Matrix3 targetSigma, out Matrix3 A)
        {
            A = Matrix3.Zero;
            var sourceSqrtSigma = Matrix3.Zero;
            var success = LinAlg.ApplyFunction(sourceSigma,
                                               x => (float)Math.Sqrt(Math.Abs(x)),
                                               ref sourceSqrtSigma);
            if (!success)
                return false;

            var targetSqrtSigma = Matrix3.Zero;
            success = LinAlg.ApplyFunction(targetSigma,
                                           x => 1 / (float)Math.Sqrt(Math.Abs(x)),
                                           ref targetSqrtSigma);
            if (!success)
                return false;

            A = sourceSqrtSigma._ * targetSqrtSigma;
            return true;
        }
    }
}
