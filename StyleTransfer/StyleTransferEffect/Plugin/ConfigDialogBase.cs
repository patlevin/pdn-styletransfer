// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    /// <summary>
    /// Concrete base class for the configuration dialog.
    /// Exists only to prevent the VS2k19 forms designer
    /// from crashing horribly.
    /// </summary>
    public class ConfigDialogBase : EffectConfigDialog<StyleTransferEffect, StyleTransferEffectConfigToken>
    {
        /// <summary>
        /// Return EffectConfig token
        /// </summary>
        /// <returns>Default effect config topken</returns>
        protected override StyleTransferEffectConfigToken CreateInitialToken()
        {
            return new StyleTransferEffectConfigToken();
        }

        /// <summary>
        /// Copy properties from token to dialog UI controls.
        /// Implemented in actual dialog.
        /// </summary>
        /// <param name="effectTokenCopy"></param>
        protected override void InitDialogFromToken(StyleTransferEffectConfigToken effectTokenCopy)
        {
        }

        /// <summary>
        /// Load valus from UI controls into token properties.
        /// Implemented in actual dialog.
        /// </summary>
        /// <param name="writeValuesHere">Token that receives the values from the UI control</param>
        protected override void LoadIntoTokenFromDialog(StyleTransferEffectConfigToken writeValuesHere)
        {
        }
    }
}
