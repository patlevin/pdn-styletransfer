// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    using Microsoft.ML.OnnxRuntime.Tensors;

    /// <summary>
    /// Preset data for use with the plugin
    /// </summary>
    public class Preset
    {
        private string localisedName;

        /// <summary>
        /// Initialise a preset
        /// </summary>
        /// <param name="name">Name of the preset (possibly localised)</param>
        /// <param name="styleData">Precalculated style data</param>
        /// <param name="example">Name of the example image</param>
        public Preset(string name, Tensor<float> styleData, string example)
        {
            Name = name;
            Style = styleData;
            Example = example;
        }

        /// <summary>
        /// Gets the name of the preset
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the display name of the preset (possibly localised)
        /// </summary>
        public string DisplayName
        {
            get => string.IsNullOrWhiteSpace(localisedName) ? Name : localisedName;
            set => localisedName = value;
        }

        /// <summary>
        /// Gets the precomputed style vector for the effect
        /// </summary>
        public Tensor<float> Style { get; }

        /// <summary>
        /// Gets the name of the example image
        /// (<see cref="Presets"/> contains a corresponding <see cref="Bitmap"/>)
        /// </summary>
        public string Example { get; }
    }
}
