using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DurakGame
{
    /// <summary>
    /// Represents a panel that has a customizable border
    /// </summary>
    public class BorderPanel : Panel
    {
        /// <summary>
        /// The backing field to the border width
        /// </summary>
        private float myBorderWidth;
        /// <summary>
        /// The backing field to the border color
        /// </summary>
        private Color myBorderColor;
        /// <summary>
        /// The backing field for whether to show the border
        /// </summary>
        private bool showBorder;

        /// <summary>
        /// Gets or sets the width of the border in pixels
        /// </summary>
        public float BorderWidth
        {
            get { return myBorderWidth; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Border width cannot be less than 0");

                myBorderWidth = value;
                Invalidate();
            }
        }
        /// <summary>
        /// Gets or sets the Color of the border
        /// </summary>
        public Color BorderColor
        {
            get { return myBorderColor; }
            set
            {
                myBorderColor = value;
                Invalidate();
            }
        }
        /// <summary>
        /// Gets or sets whether the border should be shown on this panel
        /// </summary>
        public bool ShowBorder
        {
            get { return showBorder; }
            set
            {
                showBorder = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Creates a new default border panel
        /// </summary>
        public BorderPanel()
        {
            myBorderColor = Color.Black;
            BorderWidth = 2;
            ShowBorder = true;
        }

        /// <summary>
        /// Overrides the OnPaint event, this is where the boder is drawn
        /// </summary>
        /// <param name="e">The event arguments for this event, with the graphics to draw with</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            if (ShowBorder)
            {
                Pen pen = new Pen(BorderColor, BorderWidth);
                e.Graphics.DrawRectangle(pen, BorderWidth / 2, BorderWidth / 2, Width - BorderWidth, Height - BorderWidth);
            }
        }
    }
}
