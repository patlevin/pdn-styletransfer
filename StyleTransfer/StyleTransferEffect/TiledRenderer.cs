// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer
{
    using Microsoft.ML.OnnxRuntime.Tensors;
    using PaintDotNet.Rendering;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Drawing;

    /// <summary>
    /// Tile processing event argumnts
    /// </summary>
    public class TileEventArgs
    {
        /// <summary>
        /// Initialise event args from graph event and renderer properties
        /// </summary>
        /// <param name="e">Graph processing event</param>
        /// <param name="tile">Tile location</param>
        /// <param name="tileNum">Tile number</param>
        /// <param name="tileCount">Total number of tiles</param>
        public TileEventArgs(EffectGraphEventArgs e, Rectangle tile, int tileNum, int tileCount)
        {
            Contract.Requires(e != null);
            What = e.What;
            Tile = tile;
            TileNumber = tileNum;
            TileCount = tileCount;
        }

        /// <summary>
        /// Get the current graph event
        /// </summary>
        public GraphEvent What { get; }

        /// <summary>
        /// Get the current tile number
        /// </summary>
        public int TileNumber { get; }

        /// <summary>
        /// Get the total number of tiles
        /// </summary>
        public int TileCount { get; }

        /// <summary>
        /// Get the tile location
        /// </summary>
        public Rectangle Tile { get; }

        /// <summary>
        /// Get or set whether processing should be stopped
        /// </summary>
        public bool Cancelled { get; set; }
    }

    /// <summary>
    /// Tiled effect renderer to enable processing of arbitrarily large images
    /// </summary>
    public class TiledRenderer
    {
        private class TileRenderState
        {
            public TileRenderState(TiledRenderer renderer)
            {
                var n = renderer.TileSize + renderer.Margin;
                SetTileSize(new Size(n, n));

                var k = (n + 2 * renderer.Margin) * 4;
                TensorRow = new byte[k];
                OutputRow = new byte[k];
                BufferRow = new byte[k];
            }

            // Set the current tile size; modifies buffers accordingly
            public void SetTileSize(Size size)
            {
                if (Tensor == null || size != SizeOf(Tensor))
                {
                    var dims = new int[] { 1, size.Height, size.Width, 3 };
                    Tensor = null;
                    Tensor = new DenseTensor<float>(dims);
                }

                Size SizeOf(Tensor<float> t)
                {
                    return new Size(t.Dimensions[2], t.Dimensions[1]);
                }
            }

            // Get current tile buffer
            public Tensor<float> Tensor { get; private set; }

            // Get horizontal margin from input
            public byte[] TensorRow { get; }

            // Get horizontal margin from output
            public byte[] OutputRow { get; }

            // Get blended horizontal margin
            public byte[] BufferRow { get; }
        }

        /// <summary>
        /// Initialise the tile renderer from input, output, and tile sizes
        /// </summary>
        /// <param name="input">Surface containing the input data (the image to be transformed)</param>
        /// <param name="output">Surface containing the output image</param>
        /// <param name="tileSize">Width and height of a tile</param>
        /// <param name="margin">Outer margin of tiles in percent [0..1]</param>
        public TiledRenderer(ISurface<ColorBgra> input, ISurface<ColorBgra> output, int tileSize, float margin)
        {
            var s = new RangedValue<float>(margin, 0.0f, 1.0f).Value;
            TileSize = tileSize;
            Margin = (int)(tileSize * s);
            Input = input;
            Output = output;
        }

        /// <summary>
        /// Fires whenever tile processing is updated
        /// </summary>
        public event EventHandler<TileEventArgs> Update;

        /// <summary>
        /// Get the tile size used in pixels
        /// </summary>
        public int TileSize { get; }

        /// <summary>
        /// Get the margin around tiles in pixels
        /// </summary>
        public int Margin { get; }

        /// <summary>
        /// Get the input surface
        /// </summary>
        public ISurface<ColorBgra> Input { get; }

        /// <summary>
        /// Get the output surface (must be readable!)
        /// </summary>
        public ISurface<ColorBgra> Output { get; }

        /// <summary>
        /// Process an effect graph using tiled input
        /// </summary>
        /// <param name="graph">Fully configured effect graph instance</param>
        public void Process(EffectGraph graph)
        {
            Contract.Requires(graph != null);
            var state = new TileRenderState(this);
            var tiles = GetTiles(new Size(Input.Width, Input.Height), TileSize);
            int index = 0;

            graph.Update += GraphUpdate;

            foreach (var tile in tiles)
            {
                ++index;
                var expanded = AddMargins(tile);
                state.SetTileSize(expanded.Size);
                graph.Params.Content = Input.CopyToTensor(state.Tensor, expanded);
                var result = graph.Run(index == 1);
                CopyToOutput(state, result, tile);
            }

            graph.Update -= GraphUpdate;

            void GraphUpdate(object sender, EffectGraphEventArgs e)
            {
                var args = new TileEventArgs(e, tiles[index - 1], index, tiles.Count);
                Update?.Invoke(this, args);
            }
        }

        // Return all tiles (including margins) from input
        public static IReadOnlyList<Rectangle> GetTiles(Size size, int tileSize)
        {
            var tilesX = Math.DivRem(size.Width, tileSize, out int remX);
            var tilesY = Math.DivRem(size.Height, tileSize, out int remY);

            // expand tiles so that the smallest tile is at least 1/4 the size of a full tile
            if (remX < tileSize / 2) { tilesX -= 1; }
            if (remY < tileSize / 2) { tilesY -= 1; }

            var list = new List<Rectangle>(tilesX * tilesY);
            for (int row = 0; row < tilesY; ++row)
            {
                list.AddRange(GetTiles(row, tileSize));
            }

            if (tilesY * tileSize < size.Height)
            {
                list.AddRange(GetTiles(tilesY, size.Height - tilesY * tileSize));
            }

            return list.AsReadOnly();

            IEnumerable<Rectangle> GetTiles(int row, int height)
            {
                var y = row * tileSize;
                for (int col = 0; col < tilesX; ++col)
                {
                    yield return new Rectangle(col * tileSize, y, tileSize, height);
                }

                if (tilesX * tileSize < size.Width)
                {
                    var width = size.Width - tilesX * tileSize;
                    yield return new Rectangle(tilesX * tileSize, y, width, height);
                }
            }
        }

        // Copy tile to output
        private void CopyToOutput(TileRenderState state, Tensor<float> tensor, Rectangle tile)
        {
            if (tile.Y > 0)
            {
                BlendTopMargin(state, tensor, tile);
            }

            var sx = tile.X > 0 ? Margin : 0;
            var sy = tile.Y > 0 ? Margin : 0;
            var (x, y) = (tile.X, tile.Y);
            var cols = Math.Min(tensor.Width() - sx, Output.Width - tile.X);
            var rows = Math.Min(tensor.Height() - sy, Output.Height - tile.Y);

            var tensorRow = new ArraySegment<byte>(state.TensorRow, 0, cols * 4);
            var marginLeft = new ArraySegment<byte>(state.OutputRow, 0, sx * 4);

            for (int row = sy; row < rows; ++row)
            {
                _ = tensor.GetRowArgb(tensorRow, sx, row + sy);
                Output.GetRowArgb(marginLeft, tile.X, row + y);
                marginLeft.LinearBlend(tensorRow, tensorRow);
                Output.SetRowArgb(tensorRow, x, row + y);
            }
        }

        // Blend a tile's top rows with previous output
        private void BlendTopMargin(TileRenderState state, Tensor<float> tensor, Rectangle tile)
        {
            var (ix, iy) = (tile.X > 0 ? Margin : 0, Margin);
            var (ox, oy) = (tile.X, tile.Y);
            var delta = 1.0f / Margin;
            var c = delta;
            var elements = (tensor.Width() - ix) * 4;
            var tensorRow = new ArraySegment<byte>(state.TensorRow, 0, elements);
            var outputRow = new ArraySegment<byte>(state.OutputRow, 0, elements);
            var resultRow = new ArraySegment<byte>(state.BufferRow, 0, elements);

            for (int i = 0; i < Margin; ++i, ++iy, ++oy, c += delta)
            {
                _ = tensor.GetRowArgb(tensorRow, ix, iy);
                _ = Output.GetRowArgb(outputRow, ox, oy);
                outputRow.MixArgb(tensorRow, resultRow, c);
                Output.SetRowArgb(resultRow, ox, oy);
            }
        }

        // Add margins if applicable
        private Rectangle AddMargins(Rectangle tile)
        {
            var expanded = tile;

            if (tile.X >= Margin)
            {
                // add left margin
                expanded.Offset(-Margin, 0);
                expanded.Width += Margin;
            }

            if (tile.Right < Input.Width - Margin)
            {
                // add right margin
                expanded.Width += Margin;
            }

            if (tile.Y >= Margin)
            {
                // add top margin
                expanded.Offset(0, -Margin);
                expanded.Height += Margin;
            }

            if (tile.Bottom < Input.Height - Margin)
            {
                // add bottom margin
                expanded.Height += Margin;
            }

            return expanded;
        }
    }
}
