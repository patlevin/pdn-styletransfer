using System;

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    /// <summary>
    /// Implements the model provider for the plugin.
    /// </summary>
    class ModelProvider : IModelProvider
    {
        /// <summary>
        /// Device identifier of the current CPU.
        /// </summary>
        public const int DEVICE_CPU = -1;

        /// <inheritdoc/>
        public ModelType StyleType
        {
            get => styleType;
            set
            {
                isReloadRequired = isReloadRequired || value != styleType;
                styleType = value;
            }
        }

        /// <inheritdoc/>
        public ModelType TransformType
        {
            get => transformType;
            set
            {
                isReloadRequired = isReloadRequired || value != transformType;
                transformType = value;
            }
        }

        /// <inheritdoc/>
        public int ComputationDevice
        {
            get => deviceId;
            set
            {
                isReloadRequired = isReloadRequired || value != deviceId;
                deviceId = value;
            }
        }

        /// <inheritdoc/>
        public IStyleModel Style => LoadStyleModel();

        /// <inheritdoc/>
        public ITransformModel Transform => LoadTransformModel();

        /// <summary>
        /// Initialise the provider, but don't load any data just yet.
        /// </summary>
        public ModelProvider()
        {
            transformModel = new TransformerModel();
            styleModel = new StyleModel();
        }

        /// <summary>Release DML models</summary>
        public void Dispose()
        {
            styleModel.Dispose();
            transformModel.Dispose();
            GC.SuppressFinalize(this);
        }

        // Init or update style extraction model if required
        private IStyleModel LoadStyleModel()
        {
            if (styleModel.Ready && !isReloadRequired)
            {
                return styleModel;
            }

            switch (styleType)
            {
                case ModelType.Fast:
                {
                    styleModel.Load(ModelData.StyleFast, deviceId);
                    break;
                }
                case ModelType.Quality:
                {
                    styleModel.Load(ModelData.StyleQuality, deviceId);
                    break;
                }
            }

            return styleModel;
        }

        // Init or update content transform model if required
        private ITransformModel LoadTransformModel()
        {
            if (transformModel.Ready && !isReloadRequired)
            {
                return transformModel;
            }

            switch (transformType)
            {
                case ModelType.Fast:
                {
                    transformModel.Load(ModelData.TransformerFast, deviceId);
                    break;
                }
                case ModelType.Quality:
                {
                    transformModel.Load(ModelData.TransformerQuality, deviceId);
                    break;
                }
            }

            return transformModel;
        }

        private readonly ITransformModel transformModel;

        private readonly IStyleModel styleModel;

        private int deviceId = DEVICE_CPU;

        private ModelType styleType = ModelType.Default;

        private ModelType transformType = ModelType.Default;

        private bool isReloadRequired = true;
    }
}
