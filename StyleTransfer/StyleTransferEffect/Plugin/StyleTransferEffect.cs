// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;

    /// <summary>
    /// Style Transfer Effect plugin implementation
    /// </summary>
    /// <remarks>
    /// Issues:
    /// • due to a bug in PDN 4.2.8 (and possibly earlier), the effect is configured multiple times
    /// • there is no GPU acceleration available (yet) due to compatibility issues with DirectML
    ///   and the ONNX runtime version that models require
    /// 
    /// Why not using IndirectUI and PropertyBasedEffect?
    /// • the effect is a little more complex than the usual image effects and requires
    ///   some unique approaches (e.g. custom tile based processing) for best results
    /// • there is no image control in IndirectUI that allows selecting and previewing
    /// • because of the long calculation times, I'd like to provide some feedback to
    ///   the user; this is not a feature that PDN supports out-of-the-box
    /// • localisation (I wanted that feature) is simpler when using custom forms and dialogs
    /// 
    /// Processing Workflow:
    /// 1. PDN creates a new effect instance every time the effect is opened via the menu or "repeat"-shortcut
    /// 2. during initialisation, OnSetRenderInfo is called (twice, due to a bug) with the last known config data
    /// 3. if called via menu, CreateConfigDialog)() is called *before* OnSetRenderInfo()
    /// 4. OnRender() is not called after config dialog closes
    /// 
    /// </remarks>
    public class StyleTransferEffect : Effect<StyleTransferEffectConfigToken>
    {
        // tile margin in percent [0..1]
        private const float MARGIN = 0.2f;

        // Effect graph (combined model graph)
        private readonly EffectGraph graph = new EffectGraph();
        // Flag: call OnRender() from OnSetRenderInfo(), set after config dialog closes
        private bool mustCallOnRender = false;
        // Tile size for tiled rendering; 0 => don't use tile-based rendering
        private int tileSize = 0;

        // Effect options - configurableand using a single render call
        private static readonly EffectOptions options = new EffectOptions
        {
            Flags = EffectFlags.Configurable,
            RenderingSchedule = EffectRenderingSchedule.None
        };

        #region Patch for assembly binding redirect
        // Flag to limit binding redirection to one-shot behaviour
        private static bool IsLoaded = false;
        private static readonly object syncLock = new object();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance",
            "CA1810:Initialize reference type static fields inline",
            Justification = "Required by workaround for assembly binding redirect patch")]
        static StyleTransferEffect()
        {
            if (!IsLoaded)
            {
                lock (syncLock)
                {
                    if (!IsLoaded)
                    { 
                        AssemblyBindingRedirectHelper.ConfigureBindingRedirects();
                        IsLoaded = true;
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Initialise the effect
        /// </summary>
        public StyleTransferEffect()
            : base(StringResources.EffectName, Properties.Resources.Icon, SubmenuNames.Artistic, options)
        {
            IsRenderingEnabled = true;
        }

        /// <summary>
        /// Set to false to prevent from rendering in the background while being configured
        /// </summary>
        public bool IsRenderingEnabled
        {
            get; set;
        }

        /// <summary>
        /// Return the estimated amount of memory required to process the effect
        /// </summary>
        /// <param name="size">Size of the input image</param>
        /// <param name="scalingFactor">Scaling factor</param>
        /// <returns>Estimated number of bytes required to process the effect</returns>
        public static long GetEstimatedRequiredMemory(Size size, float scalingFactor)
        {
            return EffectGraph.GetEstimatedRequiredMemory(size.ScaleBy(scalingFactor));
        }

        /// <summary>
        /// Return the suggested scale range for the style image in percent (0..100)
        /// </summary>
        /// <param name="size">Style image size in pixels</param>
        /// <returns>Tuple of minimum and maximum suggested scale values in percent</returns>
        public static (int min, int max) GetSuggestedSizeRange(Size size)
        {
            return (size.Width > size.Height)
                ? GetOptimalScale(size.Height)
                : GetOptimalScale(size.Width);

            (int, int) GetOptimalScale(int dim)
            {
                var min = ((EffectGraph.OptimalStyleSizeLo + 0f) / dim) * 100;
                var max = ((EffectGraph.OptimalStyleSizeHi + 0f) / dim) * 100;
                return ((int)(min + 0.5f), (int)(max + 0.5f));
            }
        }

        /// <summary>
        /// Return the config dialog
        /// </summary>
        /// <returns>Config dialog form</returns>
        public override EffectConfigDialog CreateConfigDialog()
        {
            // called whenever the effect is selected from the Effect-menu:
            // disable all background rendering of the effect until it was
            // successfully configured by the user
            IsRenderingEnabled = false;
            var dialog = new StyleTransferEffectConfigDialog();
            dialog.FormClosing += ConfigDialogClosing;
            return dialog;
        }

        /// <summary>
        /// Apply configuration and set inputs and outputs
        /// </summary>
        /// <param name="newToken">User configuration from dialog</param>
        /// <param name="dstArgs">Destination data (outputs)</param>
        /// <param name="srcArgs">Source data (inputs)</param>
        protected override void OnSetRenderInfo(StyleTransferEffectConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            Contract.Requires(newToken != null && srcArgs != null);
            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);

            // There's a bug in PDN 4.2.8 (possibly earlier versions as well) where
            // OnSetRenderInfo(...) is called *twice* by the caller...
            // This effect is *very* expensive in terms of both memory and CPU-time,
            // so we do what we can to avoid processing it.

            // props.IsValid - denotes whether all required settings are configured
            // this.IsRenderingEnabled - false while the configuration dialog is displayed
            // graph.CanRun - false, if the model cannot be evaluated (e.g. session not started)

            var properties = newToken.Properties;
            if (properties.IsValid && IsRenderingEnabled)
            {
                graph.Params.StyleRatio = properties.StyleRatio;
                if (properties.IsPreset)
                {
                    var tensor = Presets.Instance[properties.PresetName].Style;
                    graph.Params.SetStyleVector(tensor);
                }
                else
                {
                    using (var style = new Bitmap(properties.StyleImage))
                    {
                        var scaling = GetLimitedScalingFactor(style.Size, properties.StyleScale);
                        graph.Params.Style = style.ToTensor(scaling);
                    }
                }

                var tiles = EffectGraph.GetRecommendedTileSize(SrcArgs.Size, MARGIN);
                if (tiles == SrcArgs.Size)
                {
                    graph.Params.Content = srcArgs.Surface.ToTensor();
                    tileSize = 0;
                }
                else
                {
                    tileSize = tiles.Width;
                }

                graph.Style.Load(properties.StyleModel == ModelType.Fast
                    ? ModelData.StyleFast : ModelData.StyleQuality);
                graph.Transformer.Load(properties.TransformerModel == ModelType.Fast
                    ? ModelData.TransformerFast : ModelData.TransformerQuality);

                if (mustCallOnRender)
                {
                    OnRender(Array.Empty<Rectangle>(), 0, 0);
                }
            }
        }

        // Return adjusted scaling factor so that scaled tensor is at least of minimum supported size
        // and doesn't require more memory than is available
        private float GetLimitedScalingFactor(Size size, float scalingFactor)
        {
            var limit = EffectGraph.MinimumStyleSize;
            var requestedSize = size.ScaleBy(scalingFactor);
            if (requestedSize.Width > requestedSize.Height && requestedSize.Height < limit)
            {
                return limit / (size.Height + 0f);
            }
            else if (!(requestedSize.Width > requestedSize.Height) && requestedSize.Width < limit)
            {
                return limit / (size.Width + 0f);
            }
            else return GetUpperScalingLimit(requestedSize, scalingFactor);
        }

        // Get scaling factor limited to available memory
        private static float GetUpperScalingLimit(SizeF scaledSize, float scalingFactor)
        {
            // estimate required memory and use 60% as threshold value
            var required = EffectGraph.GetEstimatedRequiredMemory(scaledSize);
            var threshold = (EffectGraph.AvailableMemory * 100) / 60;
            if (required > threshold)
            {
                // limit scaling factor to fit available memory
                var maxPixels = EffectGraph.GetMaximumPixelCount(threshold);
                var originalPixels = ((double)scaledSize.Width * scaledSize.Height) / scalingFactor;
                return (float)(maxPixels / originalPixels);
            }
            else return scalingFactor;
        }

        /// <summary>
        /// Render the effect to the target surface - we render everything here and ignore tiling
        /// </summary>
        /// <param name="renderRects">Tiles to render (ignored here)</param>
        /// <param name="startIndex">Start index into rects (ignored here)</param>
        /// <param name="length">Number of rects to render (ignored here)</param>
        protected override void OnRender(Rectangle[] renderRects, int startIndex, int length)
        {
            if (graph.CanRun && Token.Properties.IsValid && IsRenderingEnabled)
            {
                var progressInfo = GetProgressInfo();

                if (tileSize > 0)
                {
                    var tileFormat = StringResources.Get("TileInfoFormat");
                    var renderer = new TiledRenderer(SrcArgs.Surface, DstArgs.Surface, tileSize, MARGIN);
                    renderer.Update += UpdateTiled;
                    renderer.Process(graph);

                    void UpdateTiled(object sender, TileEventArgs e)
                    {
                        var info = StringResources.Get("GraphEvent." + e.What.ToString());
                        var tile = string.Format(CultureInfo.InvariantCulture, tileFormat, e.TileNumber, e.TileCount);
                        progressInfo(tile + " " + info);
                    }
                }
                else
                {
                    graph.Update += UpdateGraph;
                    var result = graph.Run();
                    graph.Update -= UpdateGraph;
                    result.ToSurface(DstArgs.Surface);

                    void UpdateGraph(object sender, EffectGraphEventArgs e)
                    {
                        progressInfo(StringResources.Get("GraphEvent." + e.What.ToString()));
                    }
                }

                mustCallOnRender = false;
            }
        }

        /// <summary>
        /// Free managed and unmanaged resources
        /// </summary>
        /// <param name="disposing"><c>true</c>, iff called by <see cref="Dispose"/></param>
        protected override void OnDispose(bool disposing)
        {
            base.OnDispose(disposing);
            if (disposing)
            {
                graph.Dispose();
            }
        }

        // Force OnRender() in OnSetRenderInfo(), if config dialog closes
        private void ConfigDialogClosing(object sender, FormClosingEventArgs e)
        {
            mustCallOnRender = e.CloseReason == CloseReason.None
                && ((Form)sender).DialogResult == DialogResult.OK;
        }

        // HACK: find an open form with a public property called "HeaderText"
        //       this should be the progress dialog that we can hijack :)
        // Return a function that updates the progress information if such form is found
        private Action<string> GetProgressInfo()
        {
            var forms = Application.OpenForms;

            for (int i = forms.Count - 1; i >= 0; --i)
            {
                var form = forms[i];
                var info = form.GetType().GetProperty("HeaderText");
                if (info != null)
                {
                    return SetHeaderText;

                    void SetHeaderText(string text)
                    {
                        if (!form.IsDisposed)
                        {
                            Action SetText = () => info.SetValue(form, text);
                            form.BeginInvoke(SetText);
                        }
                    }
                }
            }

            return IgnoreRequest;

            void IgnoreRequest(string text) { }
        }
    }
}
