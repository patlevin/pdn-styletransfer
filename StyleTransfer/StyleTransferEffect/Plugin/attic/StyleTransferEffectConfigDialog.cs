// SPDX-License-Identifier: MIT
// Copyright © 2019 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// Style Transfer Effect configuration dialog
    /// </summary>
    public partial class StyleTransferEffectConfigDialog : ConfigDialogBase
    {
        const int DEFAULT_SIZE = 50;            // Default style image size (percentage)
        const int DEFAULT_AMOUNT = 100;         // Default effect amount (percentage)

        /// <summary>
        /// Initialise UI
        /// </summary>
        public StyleTransferEffectConfigDialog()
        {
            InitializeComponent();
            SetDialogIcon();
            LocalizeComponent();
            ApplyLocalisedHelp();
        }

        /// <summary>
        /// Check whether the model files can be found
        /// </summary>
        /// <param name="e">Ignored</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!File.Exists(Directories.ModelArchive))
            {
                // Show messagebox and close
                MessageBox.Show(StringResources.Get("ErrorMissingModel"),
                    StringResources.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        /// <summary>
        /// Copy config to UI
        /// </summary>
        /// <param name="effectTokenCopy">Config to apply</param>
        protected override void InitDialogFromToken(StyleTransferEffectConfigToken effectTokenCopy)
        {
            Contract.Requires(effectTokenCopy != null);

            var properties = effectTokenCopy.Properties;
            // set UI from token
            trackBarAmount.Value = properties.StyleAmount;
            numericUpDownAmount.Value = properties.StyleAmount;
            trackBarSize.Value = properties.StyleSize;
            numericUpDownSize.Value = properties.StyleSize;
            pictureBoxStyle.ImageLocation = properties.StyleImage;
            buttonOk.Enabled = !string.IsNullOrEmpty(properties.StyleImage);
            radioButtonStyleQuality.Checked = properties.StyleModel == ModelType.Quality;
            radioButtonStyleFast.Checked = properties.StyleModel == ModelType.Fast;
            radioButtonTransformQuality.Checked = properties.TransformerModel == ModelType.Quality;
            radioButtonTransformFast.Checked = properties.TransformerModel == ModelType.Fast;
            checkBoxAspect.Checked = properties.MatchAspectRatio;
        }

        /// <summary>
        /// Copy UI state to config
        /// </summary>
        protected override void LoadIntoTokenFromDialog(StyleTransferEffectConfigToken token)
        {
            Contract.Requires(token != null);

            var properties = token.Properties;
            // set token from UI
            properties.StyleAmount = trackBarAmount.Value;
            properties.StyleSize = trackBarSize.Value;
            properties.StyleImage = pictureBoxStyle.ImageLocation;
            properties.StyleModel = radioButtonStyleQuality.Checked ? ModelType.Quality : ModelType.Fast;
            properties.TransformerModel = radioButtonTransformQuality.Checked ? ModelType.Quality : ModelType.Fast;
            properties.MatchAspectRatio = checkBoxAspect.Checked;
        }

        // Once the scaled style image exceed a certain size, warn the user.
        // The models are not GPU-accellerated and might be very slow to
        // calculate if the image size is rather large plus a large amount
        // of RAM is required
        private void WarnAboutImageSize()
        {
            var s = trackBarSize.Value / 100.0;
            var pixels = (long?)(pictureBoxStyle.Image?.Width * pictureBoxStyle.Image?.Height * s * s);
            var maximum = EffectGraph.GetMaximumPixelCount((long)(EffectGraph.AvailableMemory * 0.8));
            if (pixels > maximum)
            {
                labelStyleDimensions.ForeColor = labelSizeWarning.ForeColor;
                labelStyleDimensions.Font = labelSizeWarning.Font;
                labelSizeWarning.Visible = true;
                panelWarning.Visible = true;
            }
            else
            {
                labelStyleDimensions.ForeColor = labelStyle.ForeColor;
                labelStyleDimensions.Font = labelStyle.Font;
                labelSizeWarning.Visible = false;
                panelWarning.Visible = false;
            }
        }

        // redraw the style preview
        private void UpdatePreview()
        {
            pictureBoxStyle.Invalidate();
        }

        // Set the dialog icon from bitmap resource
        private void SetDialogIcon()
        {
            using (var icon = Icon.FromHandle(Properties.Resources.Icon.GetHicon()))
            {
                Icon = icon;
            }
        }

        // Loop through all controls (including children), filter based on a predicate
        // and perform an action on the resulting controls
        private void ForEachControl(Predicate<Control> pred, Action<Control> action)
        {
            var stack = new Stack<Control>();
            PushAll(stack, this);

            // recursively process all controls
            while (stack.Count > 0)
            {
                var control = stack.Pop();
                if (pred(control))
                {
                    action(control);
                }
                PushAll(stack, control);
            }

            void PushAll(Stack<Control> s, Control c)
            {
                foreach (Control item in c.Controls)
                {
                    s.Push(item);
                }
            }
        }

        // Apply localised text resources
        private void LocalizeComponent()
        {
            ForEachControl(HasTextProperty, StringResources.SetText);
            openFileDialog.Title = StringResources.FileDialogTitle;
            openFileDialog.Filter = StringResources.FileDialogFilter;
            openFileDialog.FileName = string.Empty;
            Text = StringResources.EffectName;

            bool HasTextProperty(Control control)
            {
                return control.GetType().GetProperty("Text") != null;
            }
        }

        // Set help texts for all supported elements
        private void ApplyLocalisedHelp()
        {
            ForEachControl(HasHelpText, EnableHelp);

            bool HasHelpText(Control control)
            {
                return StringResources.GetHelp(control) != null;
            }

            void EnableHelp(Control control)
            {
                helpProvider.SetShowHelp(control, true);
                helpProvider.SetHelpString(control, StringResources.GetHelp(control));
            }
        }

        #region Event Handlers
        private void ButtonOkClick(object sender, EventArgs e)
        {
            EffectToken.Properties.IsValid = true;

            if (Effect != null)
            {
                Effect.IsRenderingEnabled = true;
            }
        }

        private void ButtonCancelClick(object sender, EventArgs e)
        {
            EffectToken.Properties.IsValid = false;
        }

        private void ButtonSelectStyleClick(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBoxStyle.ImageLocation = openFileDialog.FileName;
                buttonOk.Enabled = true;
            }
            else
            {
                buttonOk.Enabled = pictureBoxStyle.ImageLocation != null;
            }
        }

        private void TrackBarSizeScroll(object sender, EventArgs e)
        {
            var size = trackBarSize.Value;
            numericUpDownSize.Value = trackBarSize.Value;
            EffectToken.Properties.StyleSize = size;
            UpdatePreview();
            WarnAboutImageSize();
        }

        private void NumericSizeValueChanged(object sender, EventArgs e)
        {
            var size = (int)numericUpDownSize.Value;
            trackBarSize.Value = size;
            EffectToken.Properties.StyleSize = size;
            UpdatePreview();
            WarnAboutImageSize();
        }

        private void TrackBarAmountScroll(object sender, EventArgs e)
        {
            var amount = trackBarAmount.Value;
            numericUpDownAmount.Value = amount;
            EffectToken.Properties.StyleAmount = amount;
        }

        private void NumericAmountChanged(object sender, EventArgs e)
        {
            var amount = (int)numericUpDownAmount.Value;
            trackBarAmount.Value = amount;
            EffectToken.Properties.StyleAmount = amount;
        }

        private void ButtonResetSizeClick(object sender, EventArgs e)
        {
            trackBarSize.Value = DEFAULT_SIZE;
            numericUpDownSize.Value = DEFAULT_SIZE;
            EffectToken.Properties.StyleSize = DEFAULT_SIZE;
        }

        private void ButtonResetAmountClick(object sender, EventArgs e)
        {
            trackBarAmount.Value = DEFAULT_AMOUNT;
            numericUpDownAmount.Value = DEFAULT_AMOUNT;
            EffectToken.Properties.StyleAmount = DEFAULT_AMOUNT;
        }

        private void PictureBoxStyleClick(object sender, EventArgs e)
        {
            ButtonSelectStyleClick(sender, e);
        }

        private void PictureBoxStyleLoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                EffectToken.Properties.StyleImage = pictureBoxStyle.ImageLocation;
                var (width, height) = (pictureBoxStyle.Image.Width, pictureBoxStyle.Image.Height);
                labelStyleDimensions.Text = $"{width}x{height} px";
                labelStyleDimensions.Visible = true;
                var (lo, hi) = StyleTransferEffect.GetSuggestedSizeRange(pictureBoxStyle.Image.Size);
                trackBarSize.RangeStart = lo;
                trackBarSize.RangeEnd = hi;
                trackBarSize.ShowRange = true;
                WarnAboutImageSize();
            }
        }

        private void StyleModelChanged(object sender, EventArgs e)
        {
            EffectToken.Properties.StyleModel =
                radioButtonStyleQuality.Checked ? ModelType.Quality : ModelType.Fast;
        }

        private void TransformationModelChanged(object sender, EventArgs e)
        {
            EffectToken.Properties.TransformerModel =
                radioButtonTransformQuality.Checked ? ModelType.Quality : ModelType.Fast;
        }

        // Adds a magenta indicator on top of the style image to give an idea
        // of the selected value in relation to the input image
        private void OnPaintStyle(object sender, PaintEventArgs e)
        {
            if (pictureBoxStyle.Image == null)
            {
                return;
            }

            var preview = pictureBoxStyle.ClientSize;
            var image = pictureBoxStyle.Image != null ? pictureBoxStyle.Image.Size : preview;
            var size = GetSizeIndicatorSize(preview, image, EffectToken.Properties.StyleScale);
            var diff = preview - size;
            var (x, y) = (diff.Width / 2, diff.Height / 2);

            using (var brush = new SolidBrush(Color.FromArgb(160, Color.CadetBlue)))
            {
                e.Graphics.DrawRectangle(Pens.Magenta, x, y, size.Width - 1, size.Height - 1);
                e.Graphics.FillRectangle(brush, x + 1, y + 1, size.Width - 1.5f, size.Height - 1.5f);
            }
        }
        #endregion

        // Return the size of the style image scaling preview
        private static SizeF GetSizeIndicatorSize(Size preview, Size image, float scale)
        {
            if (image.Width > image.Height)
            {
                var ratio = image.Width / (0.0f + preview.Width);
                return new SizeF(preview.Width * scale, (image.Height / ratio) * scale);
            }
            else
            {
                var ratio = image.Height / (0.0f + preview.Height);
                return new SizeF((image.Width / ratio) * scale, preview.Height * scale);
            }
        }
    }
}
