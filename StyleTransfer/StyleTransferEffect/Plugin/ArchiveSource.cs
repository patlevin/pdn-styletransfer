// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;

    /// <summary>
    /// ZIP-archive source; used for presets that come pre-packaged with the plugin.
    /// </summary>
    internal sealed class ArchiveSource : PresetSource
    {
        private ZipArchive archive;
        private Stream stream;

        /// <summary>
        /// Return parsed CSV presest list as tuplesof presetname and example file.
        /// </summary>
        /// <param name="path">Name and path to the ZIP archive</param>
        /// <returns>List of tuples containing preset name and example file</returns>
        internal override List<(string, string)> Open(string path)
        {
            Reset();

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            stream = new FileStream(path, FileMode.Open, FileAccess.Read);
            archive = new ZipArchive(stream, ZipArchiveMode.Read);

            return ParseListing("presets.csv");
        }

        /// <summary>
        /// Return a file stream for reading a (compressed) archive entry
        /// </summary>
        /// <param name="name">Name of the file</param>
        /// <param name="sizeInBytes">Receives the file size on bytes</param>
        /// <returns>Stream to read the file</returns>
        internal override Stream OpenFile(string name, out int sizeInBytes)
        {
            var entry = archive.GetEntry(name);
            if (entry == null)
            {
                throw new FileNotFoundException(name);
            }

            sizeInBytes = (int)entry.Length;
            return entry.Open();
        }

        /// <summary>
        /// Return whether a file is contained in the archive
        /// </summary>
        /// <param name="name">Name of the file</param>
        /// <returns><c>true</c>, iff the file is in the archive</returns>
        internal override bool ContainsFile(string name)
        {
            return archive.GetEntry(name) != null;
        }

        /// <summary>
        /// Closes the archive.
        /// </summary>
        protected override void OnDispose()
        {
            Reset();
        }

        /// <summary>
        /// Disposes the current data if applicable
        /// </summary>
        private void Reset()
        {
            archive?.Dispose();
            stream?.Dispose();
        }
    }
}
