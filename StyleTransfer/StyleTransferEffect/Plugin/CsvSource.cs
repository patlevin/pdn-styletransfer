// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// CSV file source (i.e. preset files reside in some directory)
    /// </summary>
    internal sealed class CsvFileSource : PresetSource
    {
        private string basePath;

        /// <summary>
        /// Return parsed contents of CSV file
        /// </summary>
        /// <param name="path">Name and path of the CSV preset list</param>
        /// <returns>List of tuplescontaining preset name and example file</returns>
        internal override List<(string, string)> Open(string path)
        {
            basePath = Path.GetDirectoryName(path);
            return ParseListing(path);
        }

        /// <summary>
        /// Return a stream for accessing the given file.
        /// Realative paths are resolved with respect to the CSV list file.
        /// </summary>
        /// <param name="name">Name of the file</param>
        /// <param name="sizeInBytes">Receives the file size in bytes</param>
        /// <returns>Stream for accessing the file</returns>
        internal override Stream OpenFile(string name, out int sizeInBytes)
        {
            var path = Path.IsPathRooted(name) ? name : Path.Combine(basePath, name);
            sizeInBytes = (int)new FileInfo(path).Length;
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// Returns whether the given file exists.
        /// Realative paths are resolved with respect to the CSV list file.
        /// </summary>
        /// <param name="name">File name</param>
        /// <returns><c>true</c>, iff the file exists</returns>
        internal override bool ContainsFile(string name)
        {
            var path = Path.IsPathRooted(name) ? name : Path.Combine(basePath, name);
            return File.Exists(path);
        }
    }
}
