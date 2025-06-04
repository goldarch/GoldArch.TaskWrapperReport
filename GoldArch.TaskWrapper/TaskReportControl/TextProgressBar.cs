using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GoldArch.TaskWrapperReport.TaskReportControl
{
    /// <summary>
    /// Defines the display modes for the <see cref="TextProgressBar"/>.
    /// </summary>
    public enum ProgressBarDisplayMode
    {
        /// <summary>
        /// Displays the progress as a percentage.
        /// </summary>
        Percentage, // Note: This mode is not explicitly implemented in current OnPaint, CustomText is used.
        /// <summary>
        /// Displays custom text set via the <see cref="TextProgressBar.CustomText"/> property.
        /// </summary>
        CustomText,
        /// <summary>
        /// Displays the current value of the progress bar.
        /// </summary>
        Value // Note: This mode is not explicitly implemented in current OnPaint, CustomText is used.
    }

    /// <summary>
    /// A ProgressBar control that can display custom text over the progress bar.
    /// </summary>
    public class TextProgressBar : ProgressBar
    {
        private string _customText = "";
        private Color _progressColor = Color.LightGreen;
        private Color _textColor = Color.Black;
        private Font _textFont; // Initialized in constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TextProgressBar"/> class.
        /// </summary>
        public TextProgressBar()
        {
            // Use a default font if not set by the designer or user.
            _textFont = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        /// <summary>
        /// Gets or sets the custom text to display on the progress bar.
        /// </summary>
        [Category("Appearance")]
        [Description("Text to display on the progress bar.")]
        [DefaultValue("")]
        public string CustomText
        {
            get { return _customText; }
            set
            {
                if (_customText != value)
                {
                    _customText = value;
                    this.Invalidate(); // Redraw the control when text changes
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the progress bar fill.
        /// </summary>
        [Category("Appearance")]
        [Description("Color of the progress bar fill.")]
        public Color ProgressColor
        {
            get { return _progressColor; }
            set
            {
                if (_progressColor != value)
                {
                    _progressColor = value;
                    this.Invalidate(); // Redraw with new color
                }
            }
        }
        // DefaultValue attribute for Color needs a string representation or typeof(Color), "ColorName"
        [DefaultValue(typeof(Color), "LightGreen")]


        /// <summary>
        /// Gets or sets the color of the text displayed on the progress bar.
        /// </summary>
        [Category("Appearance")]
        [Description("Color of the text displayed on the progress bar.")]
        public Color TextColor
        {
            get { return _textColor; }
            set
            {
                if (_textColor != value)
                {
                    _textColor = value;
                    this.Invalidate();
                }
            }
        }
        [DefaultValue(typeof(Color), "Black")]

        /// <summary>
        /// Gets or sets the font of the text displayed on the progress bar.
        /// </summary>
        [Category("Appearance")]
        [Description("Font of the text displayed on the progress bar.")]
        public Font TextFont // Note: The 'Font' property is inherited from Control. This shadows it.
                             // Consider renaming to avoid confusion or ensure proper overriding if intended.
                             // For now, keeping as is, but be mindful of this.
        {
            get { return _textFont; }
            set
            {
                if (_textFont != value)
                {
                    _textFont = value;
                    this.Invalidate();
                }
            }
        }
        // No DefaultValue for Font as it's complex. Designer will handle serialization.

        /// <summary>
        /// Gets or sets the visual mode for displaying text (currently primarily supports CustomText).
        /// </summary>
        [Category("Appearance")]
        [Description("Determines how text is displayed on the progress bar.")]
        [DefaultValue(ProgressBarDisplayMode.CustomText)]
        public ProgressBarDisplayMode VisualMode { get; set; } = ProgressBarDisplayMode.CustomText;

        /// <summary>
        /// Overrides the <see cref="Control.OnPaint"/> method to provide custom drawing.
        /// </summary>
        /// <param name="e">A <see cref="PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            Rectangle rect = this.ClientRectangle;
            Graphics g = e.Graphics;

            // Draw the standard progress bar background/border
            ProgressBarRenderer.DrawHorizontalBar(g, rect);

            // Draw the progress fill
            if (this.Value > 0 && this.Maximum > 0) // Check Maximum > 0 to prevent division by zero
            {
                // Calculate the width of the progress fill area
                float percentage = (float)this.Value / this.Maximum;
                Rectangle clip = new Rectangle(rect.X, rect.Y, (int)Math.Round(percentage * rect.Width), rect.Height);

                // Use the custom ProgressColor for the fill
                using (Brush progressBrush = new SolidBrush(this.ProgressColor))
                {
                    g.FillRectangle(progressBrush, clip);
                }
            }
            else if (this.Value == 0 && this.Maximum == 0 && this.Minimum == 0) // Marquee style or indeterminate
            {
                // If you want to support Marquee style visually when Value/Min/Max are all zero,
                // you might need to draw something indicative here, or rely on CustomText.
                // For now, it will just show an empty bar if Value is 0.
            }


            // Draw the custom text
            if (!string.IsNullOrEmpty(this.CustomText))
            {
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                using (Brush textBrush = new SolidBrush(this.TextColor))
                {
                    // Ensure TextFont is not null
                    Font fontToUse = this.TextFont ?? this.Font; // Fallback to control's default font
                    g.DrawString(this.CustomText, fontToUse, textBrush, rect, sf);
                }
            }
        }
    }
}