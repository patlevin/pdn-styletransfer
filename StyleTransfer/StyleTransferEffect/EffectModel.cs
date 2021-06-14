// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    using Microsoft.ML.OnnxRuntime;
    using Microsoft.ML.OnnxRuntime.Tensors;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Inference model for AI-based effects holding all relevant data
    /// </summary>
    internal class EffectModel : IEffectModel
    {
        /// <inheritdoc/>
        public bool Ready => Session != null;

        /// <inheritdoc/>
        public void SetDevice(int deviceId)
        {
            Session?.Dispose();
            options?.Dispose();
            options = new SessionOptions
            {
                GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL,
                ExecutionMode = deviceId < 0 ?
                    ExecutionMode.ORT_PARALLEL :
                    ExecutionMode.ORT_SEQUENTIAL,
                EnableMemoryPattern = deviceId < 0
            };

            if (deviceId >= 0)
            {
                options.AppendExecutionProvider_DML(deviceId);
            }

            Session = new InferenceSession(model, options);
        }

        /// <inheritdoc/>
        public void Load(string modelFile, int deviceId)
        {
            Load(File.ReadAllBytes(modelFile), deviceId);
        }

        /// <inheritdoc/>
        public void Load(byte[] modelData, int deviceId)
        {
            model = modelData;
            if (model?.Length > 0)
            {
                SetDevice(deviceId);
                OnModelLoaded();
            }
        }

        /// <summary>
        /// Get the current inference session - <c>null</c> if no model was loaded first
        /// </summary>
        protected InferenceSession Session { get; private set; }

        /// <summary>
        /// Allows specialised models to perform actions (e.g. validation)
        /// when the model was updated
        /// </summary>
        protected virtual void OnModelLoaded()
        { }

        /// <summary>
        /// Return the name of the input tensor that matches the given dimensions.
        /// </summary>
        /// <param name="dimensions">Dimensions of the requested tensor</param>
        /// <returns>Name of the input tensor that matches the given dimensions</returns>
        protected string TensorBySize(IStructuralEquatable dimensions)
        {
            return Session.InputMetadata
                          .Single(item => MatchDimensions(item.Value, dimensions))
                          .Key;
        }

        /// <summary>
        /// Run inference on the model given the provided named inputs.
        /// </summary>
        /// <param name="inputs">Named input tensors</param>
        /// <returns>Single inference result tensor</returns>
        protected Tensor<float> Run(IReadOnlyCollection<NamedOnnxValue> inputs)
        {
            using (var results = Session.Run(inputs))
            {
                return results.Single().AsTensor<float>().Clone();
            }
        }

        // Return whether node input meta data matches the given dimensions
        static private bool MatchDimensions(NodeMetadata metadata, IStructuralEquatable expected)
        {
            return expected.Equals(metadata.Dimensions, StructuralComparisons.StructuralEqualityComparer);
        }

        private byte[] model;

        private SessionOptions options;

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
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
