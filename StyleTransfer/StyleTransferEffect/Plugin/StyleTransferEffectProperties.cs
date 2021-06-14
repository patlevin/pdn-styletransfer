// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    /// <summary>
    /// Configuration properties of the effect
    /// </summary>
    public class StyleTransferEffectProperties
    {
        // Maximum effect amount (in percent)
        private const int MAX_AMOUNT = 100;
        // Minimum style size (in percent)
        private const int MIN_SIZE = 10;
        // Maximum style size (in percent)
        private const int MAX_SIZE = 100;
        // default size(in percent)
        private const int DEFAULT_SIZE = 50;

        /// <summary>
        /// Initialise defaults
        /// </summary>
        public StyleTransferEffectProperties()
        {
            StyleModel = ModelType.Quality;
            TransformerModel = ModelType.Quality;
            IsValid = false;
        }

        /// <summary>
        /// Get or set whether the token data is valid
        /// (e.g. all fields populated and not cancelled by user)
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Get or set whether a preset was last used.
        /// </summary>
        public bool IsPreset { get; set; }

        /// <summary>
        /// Get or set the effect amount in percent [0..100]
        /// </summary>
        public int StyleAmount
        {
            get => styleAmount.Value;
            set => styleAmount.Value = value;
        }

        /// <summary>
        /// Get or set the effect scaling in percent [10..100]
        /// </summary>
        public int StyleSize
        {
            get => styleSize.Value;
            set => styleSize.Value = value;
        }

        /// <summary>
        /// Get or set the image containing the target style
        /// </summary>
        public string StyleImage { get; set; }

        /// <summary>
        /// Get or set the style model
        /// </summary>
        public ModelType StyleModel { get; set; }

        /// <summary>
        /// Get or set the transformer model
        /// </summary>
        public ModelType TransformerModel { get; set; }

        /// <summary>
        /// Get or set whether to match the style image's aspect ratio
        /// with the original image
        /// </summary>
        public bool MatchAspectRatio { get; set; }

        /// <summary>
        /// Get or set the name of the preset to use
        /// </summary>
        public string PresetName { get; set; }

        /// <summary>
        /// Get the style ratio [0..1]
        /// </summary>
        public float StyleRatio => StyleAmount / (1.0f * MAX_AMOUNT);

        /// <summary>
        /// Get the style size [0..1]
        /// </summary>
        public float StyleScale => StyleSize / (1.0f * MAX_SIZE);

        /// <summary>
        /// Get or set the name of the color transfer method.
        /// </summary>
        public string ColorTransfer { get; set; }

        /// <summary>
        /// Get or set the compute device index (-1: CPU, 0: GPU, 1: 2nd GPU)
        /// </summary>
        public int ComputeDevice { get; set; }

        private readonly RangedValue<int> styleAmount = new RangedValue<int>(MAX_AMOUNT, 0, MAX_AMOUNT);

        private readonly RangedValue<int> styleSize = new RangedValue<int>(DEFAULT_SIZE, MIN_SIZE, MAX_SIZE);
    }
}
