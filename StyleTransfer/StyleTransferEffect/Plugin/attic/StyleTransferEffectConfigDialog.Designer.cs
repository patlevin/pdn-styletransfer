// SPDX-License-Identifier: MIT
// Copyright © 2019 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    partial class StyleTransferEffectConfigDialog
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.labelStyle = new System.Windows.Forms.Label();
            this.buttonSelectStyle = new System.Windows.Forms.Button();
            this.trackBarSize = new PaintDotNet.Effects.ML.StyleTransfer.Plugin.ExtendedTrackBar();
            this.labelSize = new System.Windows.Forms.Label();
            this.numericUpDownSize = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownAmount = new System.Windows.Forms.NumericUpDown();
            this.labelAmount = new System.Windows.Forms.Label();
            this.trackBarAmount = new System.Windows.Forms.TrackBar();
            this.groupBoxStyleModel = new System.Windows.Forms.GroupBox();
            this.radioButtonStyleFast = new System.Windows.Forms.RadioButton();
            this.radioButtonStyleQuality = new System.Windows.Forms.RadioButton();
            this.groupBoxTransformModel = new System.Windows.Forms.GroupBox();
            this.radioButtonTransformFast = new System.Windows.Forms.RadioButton();
            this.radioButtonTransformQuality = new System.Windows.Forms.RadioButton();
            this.labelStyleDimensions = new System.Windows.Forms.Label();
            this.labelSizeWarning = new System.Windows.Forms.Label();
            this.buttonResetAmount = new System.Windows.Forms.Button();
            this.buttonResetSize = new System.Windows.Forms.Button();
            this.pictureBoxStyle = new System.Windows.Forms.PictureBox();
            this.panelWarning = new System.Windows.Forms.Panel();
            this.toolTipHelp = new System.Windows.Forms.ToolTip(this.components);
            this.helpProvider = new PaintDotNet.Effects.ML.StyleTransfer.Plugin.HelpProvider();
            this.checkBoxAspect = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAmount)).BeginInit();
            this.groupBoxStyleModel.SuspendLayout();
            this.groupBoxTransformModel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStyle)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Enabled = false;
            this.buttonOk.Location = new System.Drawing.Point(152, 482);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 0;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.ButtonOkClick);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(238, 482);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.ButtonCancelClick);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            this.openFileDialog.Filter = "Image FIles|*.bmp;*.gif;*.jpg;*.jpeg;*png|All files|*.*";
            this.openFileDialog.Title = "Select Style Image";
            // 
            // labelStyle
            // 
            this.labelStyle.AutoSize = true;
            this.labelStyle.Location = new System.Drawing.Point(14, 11);
            this.labelStyle.Name = "labelStyle";
            this.labelStyle.Size = new System.Drawing.Size(62, 13);
            this.labelStyle.TabIndex = 3;
            this.labelStyle.Text = "Style Image";
            // 
            // buttonSelectStyle
            // 
            this.buttonSelectStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSelectStyle.Location = new System.Drawing.Point(56, 185);
            this.buttonSelectStyle.Name = "buttonSelectStyle";
            this.buttonSelectStyle.Size = new System.Drawing.Size(205, 23);
            this.buttonSelectStyle.TabIndex = 4;
            this.buttonSelectStyle.Text = "Select Style Image";
            this.buttonSelectStyle.UseVisualStyleBackColor = true;
            this.buttonSelectStyle.Click += new System.EventHandler(this.ButtonSelectStyleClick);
            // 
            // trackBarSize
            // 
            this.trackBarSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarSize.CentreMarkerColor = System.Drawing.Color.LimeGreen;
            this.trackBarSize.LargeChange = 10;
            this.trackBarSize.Location = new System.Drawing.Point(6, 229);
            this.trackBarSize.MarkerColor = System.Drawing.SystemColors.Highlight;
            this.trackBarSize.Maximum = 100;
            this.trackBarSize.Minimum = 10;
            this.trackBarSize.Name = "trackBarSize";
            this.trackBarSize.RangeEnd = 60;
            this.trackBarSize.RangeMarkerSize = 8;
            this.trackBarSize.RangeStart = 40;
            this.trackBarSize.ShowRange = false;
            this.trackBarSize.Size = new System.Drawing.Size(226, 45);
            this.trackBarSize.TabIndex = 5;
            this.trackBarSize.TickFrequency = 5;
            this.trackBarSize.Value = 50;
            this.trackBarSize.Scroll += new System.EventHandler(this.TrackBarSizeScroll);
            // 
            // labelSize
            // 
            this.labelSize.Location = new System.Drawing.Point(20, 213);
            this.labelSize.Name = "labelSize";
            this.labelSize.Size = new System.Drawing.Size(150, 13);
            this.labelSize.TabIndex = 6;
            this.labelSize.Text = "Style Image Size";
            // 
            // numericUpDownSize
            // 
            this.numericUpDownSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDownSize.Location = new System.Drawing.Point(238, 229);
            this.numericUpDownSize.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownSize.Name = "numericUpDownSize";
            this.numericUpDownSize.Size = new System.Drawing.Size(43, 22);
            this.numericUpDownSize.TabIndex = 7;
            this.numericUpDownSize.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownSize.ValueChanged += new System.EventHandler(this.NumericSizeValueChanged);
            // 
            // numericUpDownAmount
            // 
            this.numericUpDownAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDownAmount.Location = new System.Drawing.Point(238, 334);
            this.numericUpDownAmount.Name = "numericUpDownAmount";
            this.numericUpDownAmount.Size = new System.Drawing.Size(43, 22);
            this.numericUpDownAmount.TabIndex = 10;
            this.numericUpDownAmount.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownAmount.ValueChanged += new System.EventHandler(this.NumericAmountChanged);
            // 
            // labelAmount
            // 
            this.labelAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelAmount.AutoSize = true;
            this.labelAmount.Location = new System.Drawing.Point(20, 318);
            this.labelAmount.Name = "labelAmount";
            this.labelAmount.Size = new System.Drawing.Size(76, 13);
            this.labelAmount.TabIndex = 9;
            this.labelAmount.Text = "Stylize Amount";
            // 
            // trackBarAmount
            // 
            this.trackBarAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarAmount.LargeChange = 10;
            this.trackBarAmount.Location = new System.Drawing.Point(6, 334);
            this.trackBarAmount.Maximum = 100;
            this.trackBarAmount.Name = "trackBarAmount";
            this.trackBarAmount.Size = new System.Drawing.Size(226, 45);
            this.trackBarAmount.TabIndex = 8;
            this.trackBarAmount.TickFrequency = 5;
            this.trackBarAmount.Value = 50;
            this.trackBarAmount.Scroll += new System.EventHandler(this.TrackBarAmountScroll);
            // 
            // groupBoxStyleModel
            // 
            this.groupBoxStyleModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBoxStyleModel.Controls.Add(this.radioButtonStyleFast);
            this.groupBoxStyleModel.Controls.Add(this.radioButtonStyleQuality);
            this.groupBoxStyleModel.Location = new System.Drawing.Point(17, 389);
            this.groupBoxStyleModel.Name = "groupBoxStyleModel";
            this.groupBoxStyleModel.Size = new System.Drawing.Size(134, 75);
            this.groupBoxStyleModel.TabIndex = 11;
            this.groupBoxStyleModel.TabStop = false;
            this.groupBoxStyleModel.Text = "Style Generation";
            // 
            // radioButtonStyleFast
            // 
            this.radioButtonStyleFast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButtonStyleFast.AutoSize = true;
            this.radioButtonStyleFast.Location = new System.Drawing.Point(6, 50);
            this.radioButtonStyleFast.Name = "radioButtonStyleFast";
            this.radioButtonStyleFast.Size = new System.Drawing.Size(45, 17);
            this.radioButtonStyleFast.TabIndex = 1;
            this.radioButtonStyleFast.Text = "Fast";
            this.radioButtonStyleFast.UseVisualStyleBackColor = true;
            // 
            // radioButtonStyleQuality
            // 
            this.radioButtonStyleQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButtonStyleQuality.AutoSize = true;
            this.radioButtonStyleQuality.Checked = true;
            this.radioButtonStyleQuality.Location = new System.Drawing.Point(6, 27);
            this.radioButtonStyleQuality.Name = "radioButtonStyleQuality";
            this.radioButtonStyleQuality.Size = new System.Drawing.Size(82, 17);
            this.radioButtonStyleQuality.TabIndex = 0;
            this.radioButtonStyleQuality.TabStop = true;
            this.radioButtonStyleQuality.Text = "High Quality";
            this.radioButtonStyleQuality.UseVisualStyleBackColor = true;
            this.radioButtonStyleQuality.CheckedChanged += new System.EventHandler(this.StyleModelChanged);
            // 
            // groupBoxTransformModel
            // 
            this.groupBoxTransformModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxTransformModel.Controls.Add(this.radioButtonTransformFast);
            this.groupBoxTransformModel.Controls.Add(this.radioButtonTransformQuality);
            this.groupBoxTransformModel.Location = new System.Drawing.Point(176, 389);
            this.groupBoxTransformModel.Name = "groupBoxTransformModel";
            this.groupBoxTransformModel.Size = new System.Drawing.Size(134, 75);
            this.groupBoxTransformModel.TabIndex = 12;
            this.groupBoxTransformModel.TabStop = false;
            this.groupBoxTransformModel.Text = "Image Transformation";
            // 
            // radioButtonTransformFast
            // 
            this.radioButtonTransformFast.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButtonTransformFast.AutoSize = true;
            this.radioButtonTransformFast.Location = new System.Drawing.Point(6, 50);
            this.radioButtonTransformFast.Name = "radioButtonTransformFast";
            this.radioButtonTransformFast.Size = new System.Drawing.Size(45, 17);
            this.radioButtonTransformFast.TabIndex = 3;
            this.radioButtonTransformFast.Text = "Fast";
            this.radioButtonTransformFast.UseVisualStyleBackColor = true;
            // 
            // radioButtonTransformQuality
            // 
            this.radioButtonTransformQuality.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButtonTransformQuality.AutoSize = true;
            this.radioButtonTransformQuality.Checked = true;
            this.radioButtonTransformQuality.Location = new System.Drawing.Point(6, 27);
            this.radioButtonTransformQuality.Name = "radioButtonTransformQuality";
            this.radioButtonTransformQuality.Size = new System.Drawing.Size(82, 17);
            this.radioButtonTransformQuality.TabIndex = 2;
            this.radioButtonTransformQuality.TabStop = true;
            this.radioButtonTransformQuality.Text = "High Quality";
            this.radioButtonTransformQuality.UseVisualStyleBackColor = true;
            this.radioButtonTransformQuality.CheckedChanged += new System.EventHandler(this.TransformationModelChanged);
            // 
            // labelStyleDimensions
            // 
            this.labelStyleDimensions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelStyleDimensions.Location = new System.Drawing.Point(131, 11);
            this.labelStyleDimensions.Name = "labelStyleDimensions";
            this.labelStyleDimensions.Size = new System.Drawing.Size(150, 13);
            this.labelStyleDimensions.TabIndex = 15;
            this.labelStyleDimensions.Text = "0x0 px";
            this.labelStyleDimensions.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelStyleDimensions.Visible = false;
            // 
            // labelSizeWarning
            // 
            this.labelSizeWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelSizeWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSizeWarning.ForeColor = System.Drawing.Color.Red;
            this.labelSizeWarning.Location = new System.Drawing.Point(3, 279);
            this.labelSizeWarning.Name = "labelSizeWarning";
            this.labelSizeWarning.Size = new System.Drawing.Size(310, 13);
            this.labelSizeWarning.TabIndex = 16;
            this.labelSizeWarning.Text = "Warning, big image!";
            this.labelSizeWarning.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.labelSizeWarning.Visible = false;
            // 
            // buttonResetAmount
            // 
            this.buttonResetAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonResetAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonResetAmount.Image = global::PaintDotNet.Effects.ML.StyleTransfer.Properties.Resources.Reset;
            this.buttonResetAmount.Location = new System.Drawing.Point(287, 334);
            this.buttonResetAmount.Name = "buttonResetAmount";
            this.buttonResetAmount.Size = new System.Drawing.Size(22, 23);
            this.buttonResetAmount.TabIndex = 14;
            this.buttonResetAmount.UseVisualStyleBackColor = true;
            this.buttonResetAmount.Click += new System.EventHandler(this.ButtonResetAmountClick);
            // 
            // buttonResetSize
            // 
            this.buttonResetSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonResetSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonResetSize.Image = global::PaintDotNet.Effects.ML.StyleTransfer.Properties.Resources.Reset;
            this.buttonResetSize.Location = new System.Drawing.Point(287, 229);
            this.buttonResetSize.Name = "buttonResetSize";
            this.buttonResetSize.Size = new System.Drawing.Size(22, 22);
            this.buttonResetSize.TabIndex = 13;
            this.buttonResetSize.UseVisualStyleBackColor = true;
            this.buttonResetSize.Click += new System.EventHandler(this.ButtonResetSizeClick);
            // 
            // pictureBoxStyle
            // 
            this.pictureBoxStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxStyle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBoxStyle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxStyle.InitialImage = null;
            this.pictureBoxStyle.Location = new System.Drawing.Point(56, 27);
            this.pictureBoxStyle.Name = "pictureBoxStyle";
            this.pictureBoxStyle.Size = new System.Drawing.Size(205, 152);
            this.pictureBoxStyle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxStyle.TabIndex = 2;
            this.pictureBoxStyle.TabStop = false;
            this.pictureBoxStyle.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.PictureBoxStyleLoadCompleted);
            this.pictureBoxStyle.Click += new System.EventHandler(this.PictureBoxStyleClick);
            this.pictureBoxStyle.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintStyle);
            // 
            // panelWarning
            // 
            this.panelWarning.BackgroundImage = global::PaintDotNet.Effects.ML.StyleTransfer.Properties.Resources.Warning;
            this.panelWarning.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelWarning.Location = new System.Drawing.Point(4, 209);
            this.panelWarning.Name = "panelWarning";
            this.panelWarning.Size = new System.Drawing.Size(16, 16);
            this.panelWarning.TabIndex = 17;
            this.panelWarning.Visible = false;
            // 
            // toolTipHelp
            // 
            this.toolTipHelp.AutomaticDelay = 50;
            this.toolTipHelp.AutoPopDelay = 2000;
            this.toolTipHelp.InitialDelay = 50;
            this.toolTipHelp.ReshowDelay = 1000;
            // 
            // helpProvider
            // 
            this.helpProvider.ToolTip = this.toolTipHelp;
            // 
            // checkBoxAspect
            // 
            this.checkBoxAspect.AutoSize = true;
            this.checkBoxAspect.Enabled = false;
            this.checkBoxAspect.Location = new System.Drawing.Point(16, 295);
            this.checkBoxAspect.Name = "checkBoxAspect";
            this.checkBoxAspect.Size = new System.Drawing.Size(168, 17);
            this.checkBoxAspect.TabIndex = 18;
            this.checkBoxAspect.Text = "Match Aspect Ratio To Image";
            this.checkBoxAspect.UseVisualStyleBackColor = true;
            // 
            // StyleTransferEffectConfigDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(323, 511);
            this.Controls.Add(this.checkBoxAspect);
            this.Controls.Add(this.panelWarning);
            this.Controls.Add(this.labelSizeWarning);
            this.Controls.Add(this.labelStyleDimensions);
            this.Controls.Add(this.buttonResetAmount);
            this.Controls.Add(this.buttonResetSize);
            this.Controls.Add(this.groupBoxTransformModel);
            this.Controls.Add(this.groupBoxStyleModel);
            this.Controls.Add(this.numericUpDownAmount);
            this.Controls.Add(this.labelAmount);
            this.Controls.Add(this.trackBarAmount);
            this.Controls.Add(this.numericUpDownSize);
            this.Controls.Add(this.labelSize);
            this.Controls.Add(this.trackBarSize);
            this.Controls.Add(this.buttonSelectStyle);
            this.Controls.Add(this.labelStyle);
            this.Controls.Add(this.pictureBoxStyle);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.HelpButton = true;
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "StyleTransferEffectConfigDialog";
            this.Text = "Style Transfer";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarAmount)).EndInit();
            this.groupBoxStyleModel.ResumeLayout(false);
            this.groupBoxStyleModel.PerformLayout();
            this.groupBoxTransformModel.ResumeLayout(false);
            this.groupBoxTransformModel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxStyle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.PictureBox pictureBoxStyle;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label labelStyle;
        private System.Windows.Forms.Button buttonSelectStyle;
        private ExtendedTrackBar trackBarSize;
        private System.Windows.Forms.Label labelSize;
        private System.Windows.Forms.NumericUpDown numericUpDownSize;
        private System.Windows.Forms.NumericUpDown numericUpDownAmount;
        private System.Windows.Forms.Label labelAmount;
        private System.Windows.Forms.TrackBar trackBarAmount;
        private System.Windows.Forms.GroupBox groupBoxStyleModel;
        private System.Windows.Forms.RadioButton radioButtonStyleFast;
        private System.Windows.Forms.RadioButton radioButtonStyleQuality;
        private System.Windows.Forms.GroupBox groupBoxTransformModel;
        private System.Windows.Forms.RadioButton radioButtonTransformFast;
        private System.Windows.Forms.RadioButton radioButtonTransformQuality;
        private System.Windows.Forms.Button buttonResetSize;
        private System.Windows.Forms.Button buttonResetAmount;
        private System.Windows.Forms.Label labelStyleDimensions;
        private System.Windows.Forms.Label labelSizeWarning;
        private System.Windows.Forms.Panel panelWarning;
        private System.Windows.Forms.ToolTip toolTipHelp;
        private HelpProvider helpProvider;
        private System.Windows.Forms.CheckBox checkBoxAspect;
    }
}
