// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    using Microsoft.ML.OnnxRuntime;

    using System.Collections;
    using System.Linq;

    /// <summary>
    /// Style Model implementation - reads the input parameter name 
    /// </summary>
    internal class StyleModel : EffectModel
    {
        static readonly IStructuralEquatable IMAGE_DIMENSIONS = new int[] { -1, -1, -1, 3 };

        /// <summary>
        /// Input image tensor name
        /// </summary>
        public string InputImage { get; private set; }

        // Extract input tensor name while verifying model parameters
        protected override void OnModelLoaded(InferenceSession session)
        {
            InputImage = session.InputMetadata.Single(kvp => DimEquals(kvp.Value, IMAGE_DIMENSIONS)).Key;
        }
    }
}
