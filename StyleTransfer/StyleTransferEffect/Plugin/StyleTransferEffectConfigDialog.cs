// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    using Color;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// Style Transfer Effect configuration dialog
    /// </summary>
    public partial class StyleTransferEffectConfigDialog  : ConfigDialogBase
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
            LoadPresets();
            LoadColorTransferMethods();
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

            if (properties.ColorTransfer != null)
            {
                comboBoxColor.SelectedValue = properties.ColorTransfer;
            }

            if (properties.IsPreset)
            {
                comboBoxPreset.SelectedValue = properties.PresetName;
                tabControlMode.SelectedTab = tabPagePresets;
                buttonOk.Enabled = comboBoxPreset.SelectedIndex > 0;
            }
            else
            {
                comboBoxPreset.SelectedIndex = 0;
                tabControlMode.SelectedTab = tabPageCustom;
            }
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
            properties.IsPreset = comboBoxPreset.SelectedIndex > 0 && tabControlMode.SelectedTab == tabPagePresets;
            properties.PresetName = properties.IsPreset ? (string)comboBoxPreset.SelectedValue : string.Empty;
            properties.ColorTransfer = (string)comboBoxColor.SelectedValue;
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
            comboBoxPreset.Items.Clear();
            comboBoxPreset.Items.Add(StringResources.NoPreset);
            comboBoxPreset.SelectedIndex = 0;

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

        // Load any available presets and include a dummy entry for "no selection"
        private void LoadPresets()
        {
            if (Presets.Instance.Items.Count == 0)
            {
                Presets.Instance.LoadFrom(Directories.PresetsArchive);
            }

            var dummy = new Preset(StringResources.NoPreset, null, null);
            var list = new List<Preset>(Presets.Instance.Items.Count + 1)
            {
                dummy
            };
            list.AddRange(Presets.Instance.Items);
            list.ForEach(p => p.DisplayName = StringResources.Get("Preset:" + p.Name));
            list.Sort((l, r) => string.Compare(l.DisplayName, r.DisplayName, false, StringResources.Culture));

            comboBoxPreset.DataSource = list;
            comboBoxPreset.DisplayMember = "DisplayName";
            comboBoxPreset.ValueMember = "Name";
        }

        private void LoadColorTransferMethods()
        {
            var list = new List<ColorTransferDescriptor>(TransferMethods.All);
            list.Insert(0,
                new ColorTransferDescriptor(StringResources.NoColorTransfer, string.Empty));

            list.ForEach(entry =>
            {
                var localized = StringResources.Get("ColorTransfer:" + entry.Name);
                entry.DisplayName = localized ?? entry.DisplayName;
            });

            comboBoxColor.DisplayMember = "DisplayName";
            comboBoxColor.ValueMember = "Name";
            comboBoxColor.DataSource = list;
        }

        #region Event Handlers
        private void ButtonOkClick(object sender, EventArgs e)
        {
            EffectToken.Properties.IsValid = true;
            EffectToken.Properties.IsPreset = comboBoxPreset.SelectedIndex > 0
                && tabControlMode.SelectedTab == tabPagePresets;
            EffectToken.Properties.PresetName = EffectToken.Properties.IsPreset
                ? (string)comboBoxPreset.SelectedValue
                : string.Empty;

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

        private void ComboBoxPresetSelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxPreset.SelectedIndex == 0)
            {
                tabControlMode.SelectedTab = tabPageCustom;
                buttonOk.Enabled = pictureBoxStyle.Image != null;

                effectPreview.BeginUpdate();
                effectPreview.OriginalImage = null;
                effectPreview.PreviewImage = null;
                effectPreview.EndUpdate();
            }
            else
            {
                var name = comboBoxPreset.SelectedValue as string;
                var preset = Presets.Instance[name];
                effectPreview.BeginUpdate();
                effectPreview.OriginalImage = Presets.Instance.GetExample(preset);
                effectPreview.PreviewImage = Presets.Instance.GetPreview(preset);
                effectPreview.SliderPosition = 0.5;
                effectPreview.EndUpdate();

                buttonOk.Enabled = true;
            }
        }

        // Enable OK-button depending on the selected mode's state
        private void TabSelected(object sender, TabControlEventArgs e)
        {
            if (e.Action == TabControlAction.Selected)
            {
                buttonOk.Enabled = e.TabPage == tabPagePresets
                    ? comboBoxPreset.SelectedIndex > 0
                    : pictureBoxStyle.Image != null;
            }
        }

        private void ComboBoxColorsSelectedIndexChanged(object sender, EventArgs e)
        {
            EffectToken.Properties.ColorTransfer = (string)comboBoxColor.SelectedValue;
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
