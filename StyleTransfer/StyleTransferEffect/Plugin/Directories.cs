// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    using System;
    using System.IO;

    /// <summary>
    /// Local directories usd by the plugin.
    /// Separate class so we can use it in the config dialog as well.
    /// </summary>
    public static class Directories
    {
        private const string ARCHIVE_NAME = "StyleTransferModels.zip";
        private const string PRESETS_NAME = "StyleTransferPresets.zip";
        private const string PRESETS_LIST = "presets.csv";

        /// <summary>
        /// Get the full path to the model archive
        /// </summary>
        public static string ModelArchive
        {
            get
            {
                // try assembly subdir first
                var localPath = Path.GetDirectoryName(typeof(Directories).Assembly.Location);
                var archivePath = Path.Combine(localPath, ARCHIVE_NAME);
                if (!File.Exists(archivePath))
                {
                    // try local AppData next
                    var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    archivePath = Path.Combine(appData, "paint.net", ARCHIVE_NAME);
                }

                return archivePath;
            }
        }

        /// <summary>
        /// Get the full path to the presets (try plugin folder first, then local app data, and finally user data folder
        /// </summary>
        public static string PresetsArchive
        {
            get 
            {
                // try assembly location first
                var localPath = Path.GetDirectoryName(typeof(Directories).Assembly.Location);
                var archivePath = Path.Combine(localPath, PRESETS_NAME);
                if (File.Exists(archivePath))
                {
                    return archivePath;
                }

                // try local AppData next
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                archivePath = Path.Combine(appData, "paint.net", PRESETS_NAME);
                if (File.Exists(archivePath))
                {
                    return archivePath;
                }

#if DEBUG
                // try dev folder
                archivePath = Path.Combine(localPath, @"..\..\..\assets\presets", PRESETS_NAME);
                if (File.Exists(archivePath))
                {
                    return archivePath;
                }
#endif

                // try user data
                var userDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                archivePath = Path.Combine(userDocs, "paint.net User Files", "Style Transfer Presets");
                return Path.Combine(archivePath, PRESETS_LIST);
            }
        }
    }
}
