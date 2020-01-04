// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.XPath;

    /// <summary>
    /// Assembly binding redirection helper
    /// </summary>
    /// <remarks>
    /// paint.net doesn't read and evaluate assembly binding redirections from the App.config.
    /// This leads to dependencies not being found at runtime.
    /// This class was adapted from https://codopia.wordpress.com/2017/07/21/how-to-fix-the-assembly-binding-redirect-problem-in-azure-functions/
    /// to tackle the issue.
    /// </remarks>
    public static class AssemblyBindingRedirectHelper
    {
        private const string DEPENDENCIES = "/configuration/runtime/assemblyBinding/dependentAssembly";
        private const string SCHEMA_XMLNS = "xmlns=\"urn:schemas-microsoft-com:asm.v1\"";

        // Marked internal to enable unit testing
        internal class BindingRedirect
        {
            public string ShortName { get; set; }

            public string PublicKeyToken { get; set; }

            public string RedirectToVersion { get; set; }
        }

        ///<summary>
        /// Reads the "BindingRedirects" field from the app settings and applies the redirection on the
        /// specified assemblies
        /// </summary>
        public static void ConfigureBindingRedirects()
        {
            var redirects = GetBindingRedirects();
            redirects.ForEach(RedirectAssembly);
        }

        // Register assembly resolve handler for the redirected assembly with the current appdomain
        private static void RedirectAssembly(BindingRedirect bindingRedirect)
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

            // this local function properly captures the bindingRedirect argument
            Assembly ResolveAssembly(object sender, ResolveEventArgs args)
            {
                var requestedAssembly = new AssemblyName(args.Name);
                if (requestedAssembly.Name != bindingRedirect.ShortName)
                {
                    return null;
                }

                // a dummy name is used since we only need the class to
                // calculate the actual public key token for us here
                var targetPublicKeyToken = new AssemblyName(
                    $"x, PublicKeyToken={bindingRedirect.PublicKeyToken}").GetPublicKeyToken();
                requestedAssembly.SetPublicKeyToken(targetPublicKeyToken);
                requestedAssembly.Version = new Version(bindingRedirect.RedirectToVersion);
                requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

                AppDomain.CurrentDomain.AssemblyResolve -= ResolveAssembly;

                return Assembly.Load(requestedAssembly);
            }
        }

        // Parse binding redirects from XML config 
        internal static List<BindingRedirect> GetBindingRedirects()
        {
            var result = new List<BindingRedirect>();
            var appConfig = Path.GetFullPath(typeof(AssemblyBindingRedirectHelper).Assembly.Location) + ".config";
            if (!File.Exists(appConfig))
            {
                return result;
            }
            
            var doc = ReadAppConfig(appConfig);
            foreach (XPathNavigator node in doc.CreateNavigator().Select(DEPENDENCIES))
            {
                var entry = new BindingRedirect
                {
                    ShortName = node.SelectSingleNode("assemblyIdentity/@name").Value,
                    PublicKeyToken = node.SelectSingleNode("assemblyIdentity/@publicKeyToken").Value,
                    RedirectToVersion = node.SelectSingleNode("bindingRedirect/@newVersion").Value,
                };
                result.Add(entry);
            }

            return result;
        }

        // Read the app.config into XPathDocument
        private static XPathDocument ReadAppConfig(string path)
        {
            var xml = File.ReadAllText(path).Replace(SCHEMA_XMLNS, string.Empty);
            var settings = new XmlReaderSettings
            {
                MaxCharactersFromEntities = 1,          // no entities
                MaxCharactersInDocument = 4096,         // small files only
                IgnoreProcessingInstructions = true,    // no processing shenanigans
            };

            using (var text = new StringReader(xml))
            using (var reader = XmlReader.Create(text, settings))
            {
                return new XPathDocument(reader);
            }
        }
    }
}
