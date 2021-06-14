using System;

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    /// <summary>
    /// Type of the model to use.
    /// </summary>
    public enum ModelType
    {
        /// <summary>
        /// Fast model.
        /// </summary>
        Fast,
        /// <summary>
        /// High quality model.
        /// </summary>
        Quality,
        /// <summary>
        /// Default model type
        /// </summary>
        Default = Quality
    }

    /// <summary>
    /// Provider for ML model interfaces.
    /// </summary>
    public interface IModelProvider : IDisposable
    {
        /// <summary>
        /// Get or set the style model type.
        /// </summary>
        ModelType StyleType { get; set; }

        /// <summary>
        /// Get or set the content transform model type.
        /// </summary>
        ModelType TransformType { get; set; }

        /// <summary>
        /// Get or set the computation device.
        /// </summary>
        int ComputationDevice { get; set; }

        /// <summary>
        /// Get the style extraction model.
        /// </summary>
        IStyleModel Style { get; }

        /// <summary>
        /// Get the content transform model.
        /// </summary>
        ITransformModel Transform { get; }
    }
}
