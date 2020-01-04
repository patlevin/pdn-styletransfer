// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    using System.IO;
    using System.IO.Compression;

    /// <summary>
    /// Model data repository
    /// </summary>
    internal static class ModelData
    {
        // Model file names within the archive
        private const string STYLE_FAST = "style_mobilenet.onnx";
        private const string STYLE_QUALITY = "style_inception.onnx";
        private const string TRANSFORMER_FAST = "transformer_separable.onnx";
        private const string TRANSFORMER_QUALITY = "transformer_inception.onnx";

        // model data
        private static byte[] styleFast;
        private static byte[] styleQuality;
        private static byte[] transformerFast;
        private static byte[] transformerQuality;

        private static bool ModelsLoaded => styleFast != null;

        // Load model data from archive
        private static void LoadModels()
        {
            if (!ModelsLoaded && File.Exists(Directories.ModelArchive))
            {
                using (var fileStream = new FileStream(Directories.ModelArchive, FileMode.Open, FileAccess.Read))
                using (var ziparchive = new ZipArchive(fileStream, ZipArchiveMode.Read))
                {
                    styleFast = ReadModel(ziparchive, STYLE_FAST);
                    styleQuality = ReadModel(ziparchive, STYLE_QUALITY);
                    transformerFast = ReadModel(ziparchive, TRANSFORMER_FAST);
                    transformerQuality = ReadModel(ziparchive, TRANSFORMER_QUALITY);
                }
            }
        }

        // Read model data from archive
        private static byte[] ReadModel(ZipArchive archive, string name)
        {
            var entry = archive.GetEntry(name);
            if (entry == null)
            {
                throw new InvalidDataException(name);
            }

            using (var stream = entry.Open())
            {
                using (var output = new MemoryStream())
                {
                    var buffer = new byte[32768];
                    int read = 0;

                    do
                    {
                        read = stream.Read(buffer, 0, buffer.Length);
                        output.Write(buffer, 0, read);
                    } while (read == buffer.Length);

                    return output.ToArray();
                }
            }
        }

        /// <summary>
        /// Get the fast style model
        /// </summary>
        internal static byte[] StyleFast
        {
            get
            {
                LoadModels();
                return styleFast;
            }
        }

        /// <summary>
        /// Get the high-quality style model
        /// </summary>
        internal static byte[] StyleQuality
        {
            get
            {
                LoadModels();
                return styleQuality;
            }
        }

        /// <summary>
        /// Get the fast transformer model
        /// </summary>
        internal static byte[] TransformerFast
        {
            get
            {
                LoadModels();
                return transformerFast;
            }
        }

        /// <summary>
        /// Get the high-quality transformer model
        /// </summary>
        internal static byte[] TransformerQuality
        {
            get
            {
                LoadModels();
                return transformerQuality;
            }
        }
    }
}
