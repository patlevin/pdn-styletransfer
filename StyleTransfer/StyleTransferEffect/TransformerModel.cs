// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    using Microsoft.ML.OnnxRuntime;

    using System.Collections;
    using System.Linq;

    /// <summary>
    /// Transformer Model implementation - gets the input parameter names
    /// </summary>
    internal class TransformerModel : EffectModel
    {
        private static readonly IStructuralEquatable STYLE_DIMENSIONS = new int[] { -1, 1, 1, 100 };
        private static readonly IStructuralEquatable IMAGE_DIMENSIONS = new int[] { -1, -1, -1, 3 };

        /// <summary>
        /// Content image input tensor name
        /// </summary>
        public string ContentImage { get; private set; }

        /// <summary>
        /// Style vector input tensor name
        /// </summary>
        public string StyleVector { get; private set; }

        // Find content- and style input tensor names
        protected override void OnModelLoaded(InferenceSession session)
        {
            StyleVector = session.InputMetadata
                .Single(kvp => DimEquals(kvp.Value, STYLE_DIMENSIONS)).Key;
            ContentImage = session.InputMetadata
                .Single(kvp => DimEquals(kvp.Value, IMAGE_DIMENSIONS)).Key;
        }
    }
}
