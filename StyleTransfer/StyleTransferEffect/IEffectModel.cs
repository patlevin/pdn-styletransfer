// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    using Microsoft.ML.OnnxRuntime;

    /// <summary>
    /// Inference model for AI-based effects
    /// </summary>
    public interface IEffectModel
    {
        /// <summary>
        /// Load an ONNX model from file.
        /// </summary>
        /// <param name="modelFile">Path to the ONNX-model file</param>
        void Load(string modelFile);

        /// <summary>
        /// Load an ONNX model from memory.
        /// </summary>
        /// <param name="modelData">Read-only binary ONNX model data</param>
        void Load(byte[] modelData);

        /// <summary>
        /// Get the current inference session of the model.
        /// Invalid unless a model is loaded first.
        /// </summary>
        InferenceSession Session { get; }
    }
}
