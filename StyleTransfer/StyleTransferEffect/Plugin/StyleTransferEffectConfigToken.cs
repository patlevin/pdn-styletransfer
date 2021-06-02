// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Token for the effect plugin configuration
    /// </summary>
    public class StyleTransferEffectConfigToken : EffectConfigToken
    {
        /// <summary>
        /// Initialise with defaults
        /// </summary>
        public StyleTransferEffectConfigToken() : base()
        {
            Properties = new StyleTransferEffectProperties();
        }

        /// <summary>
        /// Effect properties
        /// </summary>
        public StyleTransferEffectProperties Properties { get; }

        /// <summary>
        /// Copy from other instance
        /// </summary>
        /// <param name="other">Instance to copy from</param>
        protected StyleTransferEffectConfigToken(StyleTransferEffectConfigToken other) : base(other)
        {
            Contract.Requires(other != null);
            Properties = other.Properties;
        }

        /// <summary>
        /// Return a copy of the configuration data
        /// </summary>
        /// <returns>Instance containing copied data</returns>
        public override object Clone()
        {
            return new StyleTransferEffectConfigToken(this);
        }
    }
}
