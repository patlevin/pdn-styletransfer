// SPDX-License-Identifier: MIT
// Copyright © 2020 Patrick Levin

namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    /// <summary>
    /// Trackbar control with some added features (a range indicator)
    /// </summary>
    [Description("A TrackBar control with an optional range indicator")]
    public partial class ExtendedTrackBar : UserControl
    {
        private int rangeStart = 0;
        private int rangeEnd = 5;
        private int rangeMarkerSize = 8;
        private Color markerColour = Color.BlueViolet;
        private Color centreMarkerColour = Color.ForestGreen;

        /// <summary>
        /// Initialises the control and its components
        /// </summary>
        public ExtendedTrackBar()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Get or set the value added or subtracted when the scroll box is moved by a large distance
        /// </summary>
        [Category("Behavior"), DefaultValue(5)]
        public int LargeChange
        {
            get => trackBar.LargeChange;
            set => trackBar.LargeChange = value;
        }

        /// <summary>
        /// Get or set the upper limit of the track bar's value range
        /// </summary>
        [Category("Behavior"), DefaultValue(10)]
        public int Maximum
        {
            get => trackBar.Maximum;
            set => trackBar.Maximum = value;
        }

        /// <summary>
        /// Get or set the lower limit of the track bar's value range
        /// </summary>
        [Category("Behavior"), DefaultValue(0)]
        public int Minimum
        {
            get => trackBar.Minimum;
            set => trackBar.Minimum = value;
        }

        /// <summary>
        /// Get or set the value added or subtracted when the scroll box is moved by a small distance
        /// </summary>
        [Category("Behavior"), DefaultValue(1)]
        public int SmallChange
        {
            get => trackBar.SmallChange;
            set => trackBar.SmallChange = value;
        }

        /// <summary>
        /// Get or set the numeric value of the current position of the track bar
        /// </summary>
        [Category("Behavior"), DefaultValue(0), Bindable(true)]
        public int Value
        {
            get => trackBar.Value;
            set => trackBar.Value = value;
        }

        /// <summary>
        /// Get or set whether the range is displayed
        /// </summary>
        [Category("Behavior"), DefaultValue(true)]
        public bool ShowRange
        {
            get => panelRangeOverlay.Visible;
            set => panelRangeOverlay.Visible = value;
        }

        /// <summary>
        /// Event that fires when the track bar scroll box moved
        /// </summary>
        [Category("Behavior")]
        public new event EventHandler Scroll
        {
            add => trackBar.Scroll += value;
            remove => trackBar.Scroll -= value;
        }

        /// <summary>
        /// Event that fires when the track bar value changed
        /// </summary>
        [Category("Action")]
        public event EventHandler ValueChanged
        {
            add => trackBar.ValueChanged += value;
            remove => trackBar.ValueChanged -= value;
        }

        /// <summary>
        /// Get or set the delta between ticks drawn on the trackbar
        /// </summary>
        [Category("Appearance"), DefaultValue(1)]
        public int TickFrequency
        {
            get => trackBar.TickFrequency;
            set => trackBar.TickFrequency = value;
        }

        /// <summary>
        /// Get or set the track bar value that represents the lower limit of the range marker
        /// </summary>
        [Category("Appearance"), DefaultValue(0)]
        public int RangeStart
        {
            get => rangeStart;
            set
            {
                value = Math.Max(Minimum, Math.Min(Maximum, Math.Min(rangeEnd, value)));
                if (value != rangeStart)
                {
                    rangeStart = value;
                    panelRangeOverlay.Invalidate();
                }
            }
        }

        /// <summary>
        /// Get or set the track bar value that represents the upper limit of the range marker
        /// </summary>
        [Category("Appearance"), DefaultValue(10)]
        public int RangeEnd
        {
            get => rangeEnd;
            set
            {
                value = Math.Max(Minimum, Math.Min(Maximum, Math.Max(rangeStart, value)));
                if (value != rangeEnd)
                {
                    rangeEnd = value;
                    panelRangeOverlay.Invalidate();
                }
            }
        }

        /// <summary>
        /// Get or set the range limit marker colours
        /// </summary>
        [Category("Appearance"), DefaultValue(typeof(Color), "0xFF0000")]
        public Color MarkerColor
        {
            get => markerColour;
            set
            {
                if (value != markerColour)
                {
                    markerColour = value;
                    panelRangeOverlay.Invalidate();
                }
            }
        }

        /// <summary>
        /// Get or set the colour of the marker displayed at the centre of the range
        /// </summary>
        [Category("Appearance"), DefaultValue(typeof(Color), "0x00FF00")]
        public Color CentreMarkerColor
        {
            get => centreMarkerColour;
            set
            {
                if (value != centreMarkerColour)
                {
                    centreMarkerColour = value;
                    panelRangeOverlay.Invalidate();
                }
            }
        }

        /// <summary>
        /// Get or set the range limit marker size in pixels
        /// </summary>
        [Category("Appearance"), DefaultValue(10)]
        public int RangeMarkerSize
        {
            get => rangeMarkerSize;
            set
            {
                if (value != rangeMarkerSize)
                {
                    rangeMarkerSize = value;
                    panelRangeOverlay.Invalidate();
                }
            }
        }

        /// <summary>
        /// Paint-event handler that draws a range indicator below the track bar
        /// </summary>
        /// <param name="sender">Object that sent the Paint-event</param>
        /// <param name="e">Apint event arguments</param>
        protected virtual void PaintRangeOverlay(object sender, PaintEventArgs e)
        {
            Contract.Requires(e != null);

            var left = GetRangeMarkerPosition(rangeStart);
            var right = GetRangeMarkerPosition(rangeEnd);

            // draw markers and range only if there is enough space to work with
            if (rangeEnd - rangeStart > 2 * rangeMarkerSize)
            {
                DrawMarker(e.Graphics, left, false);
                DrawMarker(e.Graphics, right, true);
                DrawRange(e.Graphics, left, right);
            }

            DrawCentreMarker(e.Graphics, left, right);
        }

        /// <summary>
        /// Draw a marker that points at the centre of the displayed range
        /// </summary>
        /// <param name="graphics">Graphics instance for drawing</param>
        /// <param name="left">Position of the left range limit marker</param>
        /// <param name="right">Position of the right range limit marker</param>
        protected virtual void DrawCentreMarker(Graphics graphics, Point left, Point right)
        {
            Contract.Requires(graphics != null);

            var pt = new Point((left.X + right.X) / 2, 4);
            var poly = new Point[]
            {
                pt, new Point(pt.X-4, pt.Y+8),
                new Point(pt.X+4, pt.Y+8), pt
            };

            using (var path = new GraphicsPath())
            using (var brush = new SolidBrush(centreMarkerColour))
            {
                path.AddPolygon(poly);
                graphics.FillPath(brush, path);
            }
        }

        /// <summary>
        /// Draw a track below the track bar that shows the displayed range
        /// </summary>
        /// <param name="g">Graphics instance for drawing</param>
        /// <param name="left">Position of the left range limit marker</param>
        /// <param name="right">Position of the right range limit marker</param>
        protected virtual void DrawRange(Graphics g, Point left, Point right)
        {
            Contract.Requires(g != null);

            var pos = trackBar.Value;
            var rangeColour = pos < rangeStart || pos > rangeEnd? Color.LightGray : markerColour;

            using (var brush = new SolidBrush(rangeColour))
            {
                g.FillRectangle(brush, left.X, 0, right.X - left.X, 4);
            }
        }

        /// <summary>
        /// Draw a range limit marker at a given point
        /// </summary>
        /// <param name="g">Graphics instance for drawing</param>
        /// <param name="pt">Pointat which to draw the marker</param>
        /// <param name="mirror">if <c>true</c>, the marker is mirrored to represent the upper (right side) limit</param>
        protected virtual void DrawMarker(Graphics g, Point pt, bool mirror)
        {
            Contract.Requires(g != null);

            var dir = mirror ? -1 : 1;
            var tri = new Point[]
            {
                pt, new Point(pt.X, pt.Y + rangeMarkerSize),
                new Point(pt.X + dir * rangeMarkerSize, pt.Y + rangeMarkerSize), pt
            };

            using (var path = new GraphicsPath())
            using (var brush = new SolidBrush(markerColour))
            {
                path.AddPolygon(tri);
                g.FillPath(brush, path);
            }
        }

        // calculate the marker position of a given track bar value
        private Point GetRangeMarkerPosition(int value)
        {
            const int PADDING_X = 13;
            const int PADDING_Y = 4;

            var delta = (value - Minimum) / Math.Max(Maximum - Minimum, 1.0f);
            var x = PADDING_X + (int)(delta * (trackBar.ClientSize.Width - 2 * PADDING_X));
            return new Point(x, PADDING_Y);
        }

        // Track-bar value change event handler - redraws the range indicator
        private void TrackBarValueChanged(object sender, EventArgs e)
        {
            panelRangeOverlay.Invalidate();
        }
    }
}
