// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin.Test
{
    partial class FormTest
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonShowDialog = new System.Windows.Forms.Button();
            this.labelResult = new System.Windows.Forms.Label();
            this.comboBoxCulture = new System.Windows.Forms.ComboBox();
            this.listViewProperties = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.effectPreview1 = new PaintDotNet.Effects.ML.StyleTransfer.Plugin.EffectPreview();
            this.SuspendLayout();
            // 
            // buttonShowDialog
            // 
            this.buttonShowDialog.Location = new System.Drawing.Point(12, 10);
            this.buttonShowDialog.Name = "buttonShowDialog";
            this.buttonShowDialog.Size = new System.Drawing.Size(89, 22);
            this.buttonShowDialog.TabIndex = 0;
            this.buttonShowDialog.Text = "Show Dialog";
            this.buttonShowDialog.UseVisualStyleBackColor = true;
            this.buttonShowDialog.Click += new System.EventHandler(this.ButtonShowDialogClick);
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResult.Location = new System.Drawing.Point(12, 44);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(91, 16);
            this.labelResult.TabIndex = 1;
            this.labelResult.Text = "[not started]";
            // 
            // comboBoxCulture
            // 
            this.comboBoxCulture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCulture.FormattingEnabled = true;
            this.comboBoxCulture.Location = new System.Drawing.Point(182, 12);
            this.comboBoxCulture.Name = "comboBoxCulture";
            this.comboBoxCulture.Size = new System.Drawing.Size(117, 21);
            this.comboBoxCulture.TabIndex = 2;
            this.comboBoxCulture.SelectedIndexChanged += new System.EventHandler(this.ComboBoxCultureSelectedIndexChanged);
            // 
            // listViewProperties
            // 
            this.listViewProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewProperties.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listViewProperties.HideSelection = false;
            this.listViewProperties.Location = new System.Drawing.Point(0, 281);
            this.listViewProperties.MultiSelect = false;
            this.listViewProperties.Name = "listViewProperties";
            this.listViewProperties.Size = new System.Drawing.Size(311, 142);
            this.listViewProperties.TabIndex = 3;
            this.listViewProperties.UseCompatibleStateImageBehavior = false;
            this.listViewProperties.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            // 
            // effectPreview1
            // 
            this.effectPreview1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.effectPreview1.SliderColor = System.Drawing.Color.AliceBlue;
            this.effectPreview1.InitialImage = null;
            this.effectPreview1.Location = new System.Drawing.Point(55, 63);
            this.effectPreview1.Name = "effectPreview1";
            this.effectPreview1.OriginalImage = global::PaintDotNet.Effects.ML.StyleTransfer.Plugin.Test.Properties.Resources.Beach;
            this.effectPreview1.PreviewImage = global::PaintDotNet.Effects.ML.StyleTransfer.Plugin.Test.Properties.Resources.BeachPreview;
            this.effectPreview1.Size = new System.Drawing.Size(200, 200);
            this.effectPreview1.TabIndex = 4;
            // 
            // FormTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 423);
            this.Controls.Add(this.effectPreview1);
            this.Controls.Add(this.listViewProperties);
            this.Controls.Add(this.comboBoxCulture);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.buttonShowDialog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormTest";
            this.ShowIcon = false;
            this.Text = "Style Transfer Configuration";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonShowDialog;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.ComboBox comboBoxCulture;
        private System.Windows.Forms.ListView listViewProperties;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private EffectPreview effectPreview1;
    }
}

