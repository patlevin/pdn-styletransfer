// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Resources;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// Localised string resources
    /// </summary>
    public static class StringResources
    {
        private static readonly Lazy<ResourceManager> rm =
            new Lazy<ResourceManager>(() => new ResourceManager(
                "PaintDotNet.Effects.ML.StyleTransfer.Strings", typeof(StringResources).Assembly));

        /// <summary>
        /// Get the current culture info
        /// </summary>
        public static CultureInfo Culture => Thread.CurrentThread.CurrentUICulture;

        /// <summary>
        /// Return a localised string by name
        /// </summary>
        /// <param name="name">Resource name</param>
        /// <returns>Localised text if available</returns>
        public static string Get(string name)
        {
            return rm.Value.GetString(name, Thread.CurrentThread.CurrentUICulture);
        }

        /// <summary>
        /// Return the localised content of a Control's "Text"-property based on the control's name
        /// </summary>
        /// <param name="control">Form control</param>
        /// <returns>Localised "Text"-property of the control if available</returns>
        public static string GetText(Control control)
        {
            Contract.Requires(control != null);
            return Get("ConfigDialog." + control.Name) ?? control.Name;
        }

        /// <summary>
        /// Return the localised help text for a given control
        /// </summary>
        /// <param name="control">Controltoget the help text for</param>
        /// <returns>Localised help text if available</returns>
        public static string GetHelp(Control control)
        {
            Contract.Requires(control != null);
            return Get("Help." + control.Name);
        }

        /// <summary>
        /// Set the localised "Text"-property of a control
        /// </summary>
        /// <param name="control">Control to localise - must have a "Text"-property</param>
        public static void SetText(Control control)
        {
            Contract.Requires(control != null);
            var prop = control.GetType().GetProperty("Text");
            Contract.Requires(prop != null);
            prop.SetValue(control, GetText(control));
        }

        /// <summary>
        /// Localised title of the file dialog
        /// </summary>
        public static string FileDialogTitle => Get("FileDialog.Title");

        /// <summary>
        /// Localised filter description for the file dialog
        /// </summary>
        public static string FileDialogFilter => $"{Get("FileDialog.Filter.Images")}|*.bmp;*.gif;*.jpg;*.jpeg;*png|{Get("FileDialog.Filter.AllFiles")}|*.*";

        /// <summary>
        /// Localised error message
        /// </summary>
        public static string MessageStyleModelNotLoaded => Get(nameof(MessageStyleModelNotLoaded));

        /// <summary>
        /// Localised error message
        /// </summary>
        public static string MessageTransformerModelNotLoaded => Get(nameof(MessageTransformerModelNotLoaded));

        /// <summary>
        /// Localised effect name
        /// </summary>
        public static string EffectName => Get(nameof(EffectName));

        /// <summary>
        /// Localised text for no preset selection
        /// </summary>
        public static string NoPreset => Get("PresetChoiceNoPreset");
    }
}
