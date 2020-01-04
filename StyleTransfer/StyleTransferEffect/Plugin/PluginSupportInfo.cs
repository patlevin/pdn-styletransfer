// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Plugin information
    /// HINT: doesn't seem to work at the moment, possibly due to the nested namespace...
    /// </summary>
    public class PluginSupportInfo : IPluginSupportInfo
    {
        /// <summary>
        /// Display-name of the plugin
        /// </summary>
        public string DisplayName => StringResources.EffectName;

        /// <summary>
        /// Plugin author
        /// </summary>
        public string Author => this.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;

        /// <summary>
        /// Copyright information
        /// </summary>
        public string Copyright => this.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;

        /// <summary>
        /// Plugin version
        /// </summary>
        public Version Version => GetType().Assembly.GetName().Version;

        /// <summary>
        /// Plugin URI
        /// </summary>
        public Uri WebsiteUri => new Uri("https://github.com/patlevin/pdn-style-transfer");

        private T GetCustomAttribute<T>() where T : Attribute
        {
            return GetType().Assembly.CustomAttributes.OfType<T>().First();
        }
    }
}
