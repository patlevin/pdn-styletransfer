// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Abstract base class for the source of preset data.
    /// Preset data can be loaded from directories or ZIP
    /// archives.
    /// </summary>
    internal abstract class PresetSource : IDisposable
    {
        /// <summary>
        /// Return a list of names and example files from the preset source.
        /// </summary>
        /// <param name="path">Name of the source (archive, CSV list, or a directory name)</param>
        /// <returns>List containing tuples of preset name and example file name</returns>
        internal abstract List<(string, string)> Open(string path);

        /// <summary>
        /// Return a readable stream for the given file name
        /// </summary>
        /// <param name="name">Name of the requesed file</param>
        /// <param name="sizeInBytes">Recaives the file size in bytes</param>
        /// <returns>Readable stream to access the file</returns>
        internal abstract Stream OpenFile(string name, out int sizeInBytes);

        /// <summary>
        /// Return whether the preset source contains a given file
        /// </summary>
        /// <param name="name">Name of the file to find</param>
        /// <returns><c>true</c>, iff a file with the provided name exists</returns>
        internal abstract bool ContainsFile(string name);

        /// <summary>
        /// Return preset name and example file name tuples from the given file
        /// </summary>
        /// <param name="fileName">Name of a CSV file (tab-delimited) that contains the preset names</param>
        /// <returns>List of tuples that contain preset name and example file name</returns>
        protected List<(string, string)> ParseListing(string fileName)
        {
            var result = new List<(string, string)>();
            using (var stream = OpenFile(fileName, out _))
            using (var reader = new StreamReader(stream))
            {
                var seps = new char[] { '\t' };
                var n = 1;
                for (var line = reader.ReadLine(); line != null; line = reader.ReadLine(), ++n)
                {
                    var parts = line.Split(seps, 2);
                    if (parts.Length != 2)
                    {
                        throw new InvalidDataException($"Invalid entry at line {n}: {line}");
                    }

                    result.Add((parts[0], parts[1]));
                }
            }

            return result;
        }

        /// <summary>
        /// Override to dispose any fields and properties that are disposable
        /// </summary>
        protected virtual void OnDispose() { }

        /// <summary>
        /// IDisposable implementation
        /// </summary>
        public void Dispose()
        {
            OnDispose();
        }
    }
}
