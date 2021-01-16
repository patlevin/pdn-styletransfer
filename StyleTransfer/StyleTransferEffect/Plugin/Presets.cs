// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    using Microsoft.ML.OnnxRuntime.Tensors;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.IO;

    /// <summary>
    /// Container for effect presets
    /// </summary>
    /// <remarks>
    /// There are two ways of storing presets:
    /// 1) a ZIP file containing a file "presets.csv", style data and example images
    /// 2) a CSV file formatted like "presets.csv" with data files and images
    ///    residing in the same directory
    /// 
    /// presets.csv - Preset listing file
    /// • TAB-delimited utf-8 encoded text file
    /// • two columns: preset name and example file name
    /// 
    /// The style data has the same name as the preset (no file extension).
    /// The example file has the same name as the effect plus file format extension.
    /// 
    /// Example:
    /// 
    /// presets.csv contents
    /// My Preset   Landscape.jpg
    /// Preset 2    Landscape.jpg
    /// 
    /// Folder or ZIP file contents
    /// My Preset         (binary style data file)
    /// My Preset.jpg     (stylised example file)
    /// Preset 2          (binary style data file)
    /// Preset 2.jpg      (stylised example file)
    /// Landscape.jpg     (original example file)
    /// 
    /// TODO support CRUD operations for user-created presets
    /// </remarks>
    public class Presets : IDisposable
    {
        // Number of bytes per perset style data
        private const int StyleDataSize = 100 * sizeof(float);

        // example images
        private readonly Dictionary<string, Bitmap> exampleImages
            = new Dictionary<string, Bitmap>();

        // available presets
        private readonly List<Preset> items = new List<Preset>();

        // Preset data source
        private PresetSource source;

        // Shared instance
        private static Presets instance;

        // Get the instance of the class
        public static Presets Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Presets();
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets the available presets
        /// </summary>
        public IReadOnlyList<Preset> Items => items;

        /// <summary>
        /// Return a loaded preset by name.
        /// </summary>
        /// <param name="name">Name of the preset</param>
        /// <returns>Preset instance that has a matching Name-property or <c>null</c></returns>
        public Preset this[string name]
        {
            get => items.Find(p => p.Name == name);
        }

        /// <summary>
        /// Return the example image for the given preset
        /// </summary>
        /// <param name="preset">Preset from <see cref="Items"/></param>
        /// <returns><see cref="Image"/> containing the example image</returns>
        public Image GetExample(Preset preset)
        {
            Contract.Requires(preset != null);
            return exampleImages[preset.Example];
        }

        /// <summary>
        /// Return the preview for the given preset
        /// </summary>
        /// <param name="preset">Preset from <see cref="Items"/></param>
        /// <returns><see cref="Image"/> containing the stylised example image or null</returns>
        public Image GetPreview(Preset preset)
        {
            Contract.Requires(preset != null);

            var preview = preset.Name + ".jpg";
            if (exampleImages.TryGetValue(preview, out Bitmap result))
            {
                return result;
            }

            if (source.ContainsFile(preview))
            {
                _ = LoadExample(preview);
                return exampleImages[preview];
            }

            // not found
            return null;
        }

        /// <summary>
        /// Load presets from ZIP archive or preset.csv-listing
        /// </summary>
        /// <param name="fileName">ZIP file name or CSV listing</param>
        /// <exception cref="FileNotFoundException">File is missing</exception>
        /// <exception cref="IOException">File couldn't be read or opened (lack of permissions or broken file)</exception>
        /// <exception cref="InvalidDataException">File corrupted or unexpected format</exception>
        public void LoadFrom(string fileName)
        {
            Contract.Requires(fileName != null);

            Reset();
            source = fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
                ? (PresetSource)new ArchiveSource() : new CsvFileSource();

            foreach (var (name, example) in source.Open(fileName))
            {
                var data = LoadStyleData(name);
                var image = LoadExample(example);
                items.Add(new Preset(name, data, image));
            }
        }

        // Load example image if necessary
        private string LoadExample(string example)
        {
            if (!exampleImages.ContainsKey(example))
            {
                using (var stream = source.OpenFile(example, out _))
                {
                    exampleImages.Add(example, new Bitmap(stream));
                }
            }

            return example;
        }

        // Read style data (from file or archive)
        private Tensor<float> LoadStyleData(string name)
        {
            using (var stream = source.OpenFile(name, out int sizeInBytes))
            using (var reader = new BinaryReader(stream))
            {
                if (sizeInBytes != StyleDataSize)
                {
                    throw new InvalidDataException($"{name}: invalid style data");
                }

                var n = sizeInBytes / sizeof(float);
                var data = new float[n];
                for (int i = 0; i < n; ++i)
                {
                    data[i] = reader.ReadSingle();
                }

                return new DenseTensor<float>(data, new int[] { 1, 1, 1, n });
            }
        }

        // Clear and dispose all data
        private void Reset()
        {
            foreach (var image in exampleImages.Values)
            {
                image.Dispose();
            }

            source?.Dispose();

            exampleImages.Clear();
            items.Clear();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Dispose all mamanged resources
        /// </summary>
        /// <param name="disposing"><c>true</c>, if called by <see cref="Dispose"/></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Reset();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
