// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    using Microsoft.ML.OnnxRuntime;

    using System;
    using System.Collections;
    using System.IO;

    /// <summary>
    /// Inference model for AI-based effects holding all relevant data
    /// </summary>
    internal class EffectModel : IEffectModel, IDisposable
    {
        /// <summary>
        /// Load an ONNX model file
        /// </summary>
        /// <param name="modelFile">Path to an ONNX model file</param>
        public void Load(string modelFile)
        {
            Load(File.ReadAllBytes(modelFile));
        }

        /// <summary>
        /// Load an ONNX model 
        /// </summary>
        /// <param name="modelData">Binary contents of an ONNX model file</param>
        public void Load(byte[] modelData)
        {
            Reset(modelData);
        }

        /// <summary>
        /// Get the current inference session - <c>null</c> if no model was loaded first
        /// </summary>
        public InferenceSession Session { get; private set; }

        /// <summary>
        /// Allows specialised models to perform actions (e.g. validation)
        /// when the model was updated
        /// </summary>
        /// <param name="session">Current inference session for querying meta data</param>
        protected virtual void OnModelLoaded(InferenceSession session) { }

        private void Reset(byte[] modelData)
        {
            Session?.Dispose();
            model = modelData;
            if (model?.Length > 0)
            {
                Session = new InferenceSession(model, options);
                OnModelLoaded(Session);
            }
        }

        // Return whether node input meta data matches the given dimensions
        static protected bool DimEquals(NodeMetadata metadata, IStructuralEquatable x)
        {
            return x.Equals(metadata.Dimensions, StructuralComparisons.StructuralEqualityComparer);
        }

        private byte[] model;

        private readonly SessionOptions options = new SessionOptions
        {
            ExecutionMode = ExecutionMode.ORT_PARALLEL,
            GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL
        };

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    options?.Dispose();
                    Session?.Dispose();
                }

                model = null;

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
