// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// Slightly simplified re-implementation of <c>System.Windows.Forms.HelpProvider</c>.
    /// </summary>
    /// <remarks>
    /// The implementation is customisable (unlike the framework version), doesn't support
    /// help files, but also doesn't rely on the ancient Win32 help API.
    /// The class uses standard WinForms ToolTips to display help for fully supporting
    /// Unicode, custom fonts, and colours.
    /// </remarks>
    [
        ProvideProperty("HelpString", typeof(Control)),
        ProvideProperty("ShowHelp", typeof(Control)),
        ToolboxItemFilter("System.Windows.Forms"),
        Description("Provides help integration"),
        ToolboxBitmap(typeof(System.Windows.Forms.HelpProvider)),
    ]
    public class HelpProvider : Component
    {
        private readonly Dictionary<Control, bool> showHelp = new Dictionary<Control, bool>();
        private readonly Dictionary<Control, string> helpStrings = new Dictionary<Control, string>();
        private readonly HashSet<Control> boundControls = new HashSet<Control>();
        private Control currentControl;
        private Form form;

        public HelpProvider() { }

        [
            Localizable(false),
            DefaultValue(null),
            Description("Tooltip to be used for displaying help")
        ]
        public ToolTip ToolTip
        {
            get;
            set;
        }

        /// <summary>
        /// Return whether hlp shouldbe displayed for a control
        /// </summary>
        /// <param name="control">Control to be queried</param>
        /// <returns><c>true</c>, iff help is vailable for the control and should be displayed</returns>
        [
            Localizable(true),
            Description("Indicates whether help should be displayed"),

        ]
        public virtual bool GetShowHelp(Control control)
        {
            return showHelp.TryGetValue(control, out bool show) && show;
        }

        /// <summary>
        /// Return the help text for a control
        /// </summary>
        /// <param name="control">Control to query the help text from</param>
        /// <returns>Help text for the control or <c>null</c></returns>
        [
            DefaultValue(null),
            Localizable(true),
            Description("Help string for this control"),
        ]
        public virtual string GetHelpString(Control control)
        {
            return helpStrings.TryGetValue(control, out string help) ? help : null;
        }

        /// <summary>
        /// Set whether help text should be displayed for a control
        /// </summary>
        /// <param name="control">Control to show or hide the help text for</param>
        /// <param name="value">Flag indicating whether help should be displayed forthe control</param>
        public virtual void SetShowHelp(Control control, bool value)
        {
            Contract.Requires(control != null);
            Update(showHelp, control, value);
            UpdateEventBinding(control);
        }

        /// <summary>
        /// Set the help text for a control
        /// </summary>
        /// <param name="control">Control to set the help text for</param>
        /// <param name="helpString">Help text to be displayed or <c>null</c></param>
        public virtual void SetHelpString(Control control, string helpString)
        {
            Contract.Requires(control != null);
            Update(helpStrings, control, helpString);
            SetShowHelp(control, helpString?.Length > 0);
            UpdateEventBinding(control);
        }

        /// <summary>
        /// Show help popup for control on help request
        /// </summary>
        /// <param name="sender">Control that sent the event</param>
        /// <param name="e">Help event args</param>
        protected virtual void OnControlHelp(object sender, HelpEventArgs e)
        {
            Contract.Requires(e != null);

            var control = sender as Control;
            Contract.Requires(control != null);

            if (!GetShowHelp(control))
            {
                return;
            }

            var helpString = GetHelpString(control);
            if (helpString?.Length > 0)
            {
                if (Control.MouseButtons != MouseButtons.None)
                {
                    // activated by click,so use mouse coordinates
                    ToolTip?.Show(helpString, control, control.PointToClient(e.MousePos));
                }
                else
                {
                    // activated by F1
                    ToolTip?.Show(helpString, control);
                }
                e.Handled = ToolTip != null;
                ResetOnClose(control, e.Handled);
            }
        }

        /// <summary>
        /// Return the help text for a control to an accessibility query
        /// </summary>
        /// <param name="sender">Control that sent the event</param>
        /// <param name="e">Accessibility event args</param>
        protected virtual void OnQueryAccessibilityHelp(object sender, QueryAccessibilityHelpEventArgs e)
        {
            Contract.Requires(e != null);
            e.HelpString = GetHelpString(sender as Control);
        }

        // Bind or unbind help-related events from a control
        private void UpdateEventBinding(Control control)
        {
            if (GetShowHelp(control) && !boundControls.Contains(control))
            {
                control.HelpRequested += new HelpEventHandler(OnControlHelp);
                control.QueryAccessibilityHelp += new QueryAccessibilityHelpEventHandler(OnQueryAccessibilityHelp);
                boundControls.Add(control);
            }
            else if (!GetShowHelp(control) && boundControls.Contains(control))
            {
                control.HelpRequested -= new HelpEventHandler(OnControlHelp);
                control.QueryAccessibilityHelp -= new QueryAccessibilityHelpEventHandler(OnQueryAccessibilityHelp);
                boundControls.Remove(control);
            }
        }

        private void ResetOnClose(Control control, bool handled)
        {
            if (!handled) { return; }

            if (form == null)
            {
                // close on ESC press
                form = control.FindForm();
                form.KeyPreview = true;
                form.KeyDown += new KeyEventHandler(OnEscapePressed);
            }

            currentControl = control;
        }

        private void OnEscapePressed(object sender, KeyEventArgs e)
        {
            if (currentControl != null && e.KeyData == Keys.Escape)
            {
                CloseHelp(sender, e);
            }
        }

        private void CloseHelp(object sender, EventArgs e)
        {
            ToolTip?.RemoveAll();
            currentControl = null;
        }

        // Add or update a key in a dictionary
        private static void Update<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }
    }
}
