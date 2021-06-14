// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

using System;

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    /// <summary>
    /// Inference model for AI-based effects
    /// </summary>
    public interface IEffectModel : IDisposable
    {
        /// <summary>
        /// Indicates whether the model is loaded and ready for inference.
        /// </summary>
        bool Ready { get; }

        /// <summary>
        /// Set the computation device.
        /// </summary>
        /// <param name="deviceId">Compute device identifier (-1: CPU; 0..n: GPUn)</param>
        void SetDevice(int deviceId);

        /// <summary>
        /// Load an ONNX model from file.
        /// </summary>
        /// <param name="modelFile">Path to the ONNX-model file</param>
        /// <param name="deviceId">Compute device identifier (-1: CPU; 0..n: GPUn)</param>
        void Load(string modelFile, int deviceId);

        /// <summary>
        /// Load an ONNX model from memory.
        /// </summary>
        /// <param name="modelData">Read-only binary ONNX model data</param>
        /// <param name="deviceId">Compute device identifier (-1: CPU; 0..n: GPUn)</param>
        void Load(byte[] modelData, int deviceId);
    }
}
