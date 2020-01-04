// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin.Test
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Windows.Forms;

    public partial class FormTest : Form
    {
        private static KeyValuePair<TKey, TValue> Kvp<TKey, TValue>(TKey key, TValue value)
            => new KeyValuePair<TKey, TValue>(key, value);
        private static readonly KeyValuePair<string, CultureInfo>[] lang = new KeyValuePair<string, CultureInfo>[]
        {
            Kvp("neutral", CultureInfo.InvariantCulture),
            Kvp("English (Great Britain)", CultureInfo.GetCultureInfo("en-GB")),
            Kvp("German", CultureInfo.GetCultureInfo("de")),
            Kvp("French", CultureInfo.GetCultureInfo("fr")),
            Kvp("Russian", CultureInfo.GetCultureInfo("ru-RU")),
            Kvp("Spanish (Spain)", CultureInfo.GetCultureInfo("es-ES")),
            Kvp("Spanish (Mexico", CultureInfo.GetCultureInfo("es-MX")),
            Kvp("Portuguese (Portugal)", CultureInfo.GetCultureInfo("pt-PT")),
            Kvp("Portuguese (Brazil)", CultureInfo.GetCultureInfo("pt-BR")),
            Kvp("Japanese", CultureInfo.GetCultureInfo("ja"))
        };

        public FormTest()
        {
            InitializeComponent();

            comboBoxCulture.BeginUpdate();
            comboBoxCulture.DataSource = lang;
            comboBoxCulture.DisplayMember = "Key";
            comboBoxCulture.ValueMember = "Value";
            comboBoxCulture.EndUpdate();
        }

        private void ButtonShowDialogClick(object sender, EventArgs e)
        {
            using (var dlg = new StyleTransferEffectConfigDialog())
            {
                var result = dlg.ShowDialog();
                labelResult.Text = result == DialogResult.OK ? "CONFIGURED" : "CANCELLED";
                if (result == DialogResult.OK)
                {
                    ShowProperties(dlg.EffectToken.Properties);
                }
            }
        }

        private void ShowProperties(StyleTransferEffectProperties properties)
        {
            listViewProperties.BeginUpdate();
            listViewProperties.Items.Clear();
            foreach (var prop in typeof(StyleTransferEffectProperties).GetProperties())
            {
                listViewProperties.Items.Add(new ListViewItem(new string[]
                {
                    prop.Name,
                    (prop.GetValue(properties) ?? "[null]").ToString()
                }));
            }
            listViewProperties.EndUpdate();
        }

        private void ComboBoxCultureSelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCulture.SelectedValue as CultureInfo != null)
            {
                Thread.CurrentThread.CurrentUICulture = comboBoxCulture.SelectedValue as CultureInfo;
            }
        }
    }
}
