namespace PaintDotNet.Effects.ML.StyleTransfer.Plugin
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    /// <summary>
    /// User control for previewing effects.
    /// Features a user-controlable image slider to change between "before" and "after" images.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed",
        Justification = "The control strictly DOES NOT take ownership of the images set in its properties.")]
    public partial class EffectPreview : UserControl
    {
        private const int HandleSize = 24;
        private const int HandleRadius = HandleSize / 2;

        private double position = 0.5;  // relative slider position (0..1) -> (left..right)
        private bool canDrag = false;
        private bool dragging = false;
        private bool updating = false;
        private Color sliderColour = Color.AliceBlue;
        private Brush sliderBrush = new SolidBrush(Color.AliceBlue);
        private Image originalImage = null;
        private Image previewImage = null;
        private Image initialImage = null;

        public EffectPreview()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Get or set the initial image that is shown if no original or preview are set
        /// </summary>
        [Category("Appearance")]
        public Image InitialImage
        {
            get => initialImage;
            set
            {
                if (initialImage != value)
                {
                    initialImage = value;
                    if (!updating)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Get or set the original (unaltered) image
        /// </summary>
        [Category("Appearance")]
        public Image OriginalImage
        {
            get => originalImage;
            set
            {
                if (originalImage != value)
                {
                    originalImage = value;
                    if (!updating)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Get or set a preview of the processed image
        /// </summary>
        [Category("Appearance")]
        public Image PreviewImage
        {
            get => previewImage;
            set
            {
                if (previewImage != value)
                {
                    previewImage = value;
                    if (!updating)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Get or set the colour of the image slider
        /// </summary>
        [DefaultValue(typeof(Color), "0xFFFFF8")]
        [Category("Appearance")]
        public Color SliderColor
        {
            get => sliderColour;
            set
            {
                if (value != sliderColour)
                {
                    sliderBrush.Dispose();
                    sliderBrush = new SolidBrush(value);
                    sliderColour = value;
                    if (!updating)
                    {
                        Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Get or set the relative position of the image slider
        /// </summary>
        [DefaultValue(0.5)]
        [Category("Behavior")]
        public double SliderPosition
        {
            get => position;
            set
            {
                position = Math.Max(0, Math.Min(1, value));
                if (!updating)
                {
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Start updating properties and suppress redrawing until <see cref="EndUpdate"/> is called
        /// </summary>
        public void BeginUpdate()
        {
            updating = true;
        }

        /// <summary>
        /// End property update and apply changes immediately
        /// </summary>
        public void EndUpdate()
        {
            updating = false;
            Invalidate();
        }

        /// <summary>
        /// Fires the Paint-event and draws the control
        /// </summary>
        /// <param name="e">Event args</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Contract.Requires(e != null);
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            DrawInitialImage(e.Graphics);
            DrawOriginalImage(e.Graphics);
            DrawPreviewImage(e.Graphics);
            DrawSlider(e.Graphics);
        }

        /// <summary>
        /// Fires the MouseDown-event and starts image slider dragging if the LMB is pressed
        /// </summary>
        /// <param name="e">Event args</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Contract.Requires(e != null);
            base.OnMouseDown(e);

            dragging = canDrag && !dragging && e.Button == MouseButtons.Left;
        }

        /// <summary>
        /// Fires the MouseUp-event and stops image slider dragging if the LMB was released
        /// </summary>
        /// <param name="e">Event args</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            Contract.Requires(e != null);
            base.OnMouseUp(e);
            if (dragging && e.Button == MouseButtons.Left)
            {
                // stop dragging
                dragging = false;
            }
        }

        /// <summary>
        /// Fires the MouseMove-event and updates image slider dragging
        /// </summary>
        /// <param name="e">Event args</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Contract.Requires(e != null);
            base.OnMouseMove(e);

            if (dragging)
            {
                DragToPosition(e.Location);
            }
            else
            {
                UpdateDragState(e.Location);
            }
        }

        // Synchronise slider position to cursor X-position
        private void DragToPosition(Point pt)
        {
            var newPos = Math.Max(0, Math.Min(pt.X / (ClientSize.Width + 0.0), 1));
            if (newPos != position)
            {
                position = newPos;
                Invalidate();
            }
        }

        // Update the visual state of the slider and enable/disable drag activation via left-click
        private void UpdateDragState(Point pt)
        {
            var handlePosX = ClientSize.Width * position;
            var handlePosY = ClientSize.Height / 2;
            var couldDrag = canDrag;

            canDrag = Math.Abs(pt.X - handlePosX) < HandleRadius
                   && Math.Abs(pt.Y - handlePosY) < HandleRadius;

            if (canDrag != couldDrag)
            {
                var pos = new Point((int)(handlePosX - HandleRadius - 1), 0);
                Invalidate(new Rectangle(pos, new Size(HandleSize + 3, ClientSize.Height)));
            }
        }

        // Draw the initial image if no other image is valid
        private void DrawInitialImage(Graphics graphics)
        {
            if (initialImage != null && originalImage == null && previewImage == null)
            {
                graphics.DrawImage(InitialImage, ClientRectangle);
            }
        }

        // Draw the original image if valid
        private void DrawOriginalImage(Graphics graphics)
        {
            if (originalImage != null && position > 0)
            {
                var ofs = previewImage == null ? 1.0 : position; 
                var dstRect = new Rectangle(0, 0, (int)(ClientSize.Width * ofs), ClientSize.Height);
                var srcRect = new Rectangle(0, 0, (int)(originalImage.Width * ofs), originalImage.Height);
                graphics.DrawImage(originalImage, dstRect, srcRect, GraphicsUnit.Pixel);
            }
        }

        // Draw the preview image if valid
        private void DrawPreviewImage(Graphics graphics)
        {
            if (previewImage != null)
            {
                var ofs = originalImage == null ? 0.0 : position;
                var imSize = previewImage.Size;
                var dstOffset = (int)(ClientSize.Width * ofs);
                var srcOffset = (int)(imSize.Width * ofs);
                var dstRect = new Rectangle(dstOffset, 0, ClientSize.Width - dstOffset, ClientSize.Height);
                var srcRect = new Rectangle(srcOffset, 0, imSize.Width - srcOffset, imSize.Height);
                graphics.DrawImage(previewImage, dstRect, srcRect, GraphicsUnit.Pixel);
            }
        }

        // Draw the slider elements
        private void DrawSlider(Graphics graphics)
        {
            if (originalImage != null && previewImage != null)
            {
                var centre = ClientSize.Height / 2;
                var offset = (int)(position * ClientSize.Width);
                var slider = new Rectangle(offset - HandleRadius, centre - HandleRadius, HandleSize, HandleSize);

                var brush = canDrag ? SystemBrushes.Highlight : sliderBrush;
                using (var pen = new Pen(brush, 1))
                {
                    graphics.DrawLine(pen, offset, 0, offset, centre - HandleRadius);

                    // handle
                    graphics.FillEllipse(brush, slider);
                    // left arrow
                    graphics.DrawLine(Pens.Black, offset - HandleRadius + 4, centre, offset - HandleRadius + 8, centre - 3);
                    graphics.DrawLine(Pens.Black, offset - HandleRadius + 4, centre, offset - HandleRadius + 8, centre + 3);
                    // right arrow
                    graphics.DrawLine(Pens.Black, offset + HandleRadius - 4, centre, offset + HandleRadius - 8, centre - 3);
                    graphics.DrawLine(Pens.Black, offset + HandleRadius - 4, centre, offset + HandleRadius - 8, centre + 3);

                    graphics.DrawLine(pen, offset, centre + HandleRadius, offset, ClientSize.Height);
                }
            }
        }
    }
}
