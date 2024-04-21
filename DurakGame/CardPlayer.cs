using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Durak.Common.Cards;

namespace DurakGame
{
    /// <summary>
    /// Represents a Control that represents a collection of playable cards
    /// </summary>
    public partial class CardPlayer : UserControl
    {
        /// <summary>
        /// Represents the aspect ratio for the playing cards
        /// </summary>
        private const float CARD_ASPECT_RATIO = 1.466666666666f;
        /// <summary>
        /// Represents the change in width and height for cards being hovered over
        /// </summary>
        private const float EXPAND_SIZE = 16.0f;

        private float myCardWidth;
        private float myCardHeight;
        private float myCardOffsetX;
        private float myCardOffsetY;
        /// <summary>
        /// Stores the index of the card being hovered over
        /// </summary>
        private int myHoverIndex = -1;        
        /// <summary>
        /// Represents the card collection that this Card Player is representing
        /// </summary>
        private CardCollection myCards;

        /// <summary>
        /// Invoked when a card has been selected
        /// </summary>
        public event EventHandler<CardEventArgs> OnCardSelected;

        /// <summary>
        /// Gets or sets the card collection that this card player is representing
        /// </summary>
        public CardCollection Cards
        {
            get { return myCards; }
            set
            {
                if (myCards != null)
                {
                    myCards.OnCardAdded -= OnCardsChanged;
                    myCards.OnCardRemoved -= OnCardsChanged;
                }

                myCards = value;

                if (myCards != null)
                {
                    myCards.OnCardAdded += OnCardsChanged;
                    myCards.OnCardRemoved += OnCardsChanged;
                }
            }
        }
        
        /// <summary>
        /// Creates a new card player control
        /// </summary>
        public CardPlayer()
        {
            InitializeComponent();

            // We need to use double buffering as we redraw rapidly on mouse hover
            // This lets winforms create another buffer to store the previous image
            // while rendering the new one, reducing flickering
            DoubleBuffered = true;
        }

        /// <summary>
        /// Invoked when the Card collection's number of cards has changed
        /// </summary>
        /// <param name="sender">The object that invoked the event (the card collection)</param>
        /// <param name="e">The card that has been added/removed</param>
        private void OnCardsChanged(object sender, CardEventArgs e)
        {
            Invalidate();
            CalculateSize();
        }

        /// <summary>
        /// Overrides the OnResize event, invoked when this control's size has changed
        /// </summary>
        /// <param name="e">An empty event arguments</param>
        protected override void OnResize(EventArgs e)
        {
            CalculateSize();

            base.OnResize(e);
        }

        /// <summary>
        /// Calculates the size of each individual playing card, as well as the offsets 
        /// </summary>
        private void CalculateSize()
        {
            // We only calculate if we have at least 1 card
            if (myCards != null && myCards.Count > 0)
            {
                // Calculate the available area to render in
                float availableWidth = Width - EXPAND_SIZE;
                float availableHeight = Height - EXPAND_SIZE;

                // Calculate the card width and height
                myCardWidth = availableWidth / (float)myCards.Count;
                myCardHeight = myCardWidth * CARD_ASPECT_RATIO;

                // If the height is larger than the available height, we need to clamp it
                if (myCardHeight > availableHeight)
                {
                    myCardHeight = availableHeight;
                    // Calculate width from height and inverse of card aspect ratio
                    myCardWidth = myCardHeight * (1 / CARD_ASPECT_RATIO);
                }

                // Determine the X and Y offsets
                myCardOffsetX = EXPAND_SIZE / 2 + (availableWidth - (myCardWidth * myCards.Count)) / 2.0f;
                myCardOffsetY = EXPAND_SIZE / 2 + (availableHeight - myCardHeight) / 2.0f;

            }
            else
            {
                // Reset all sizes and offsets to 0
                myCardHeight = myCardWidth = myCardOffsetX = myCardOffsetY = 0;
            }
        }

        /// <summary>
        /// Overrides the mouse left event, invoked when the mouse has left the control's bounds
        /// </summary>
        /// <param name="e">The empty event arguments for the event</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            myHoverIndex = -1;

            Invalidate();
        }

        /// <summary>
        /// Overrides the on mouse move event, invoked when the mouse has moved in this control's bounds
        /// </summary>
        /// <param name="e">The event arguments containing the mouse's position</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            myHoverIndex = (int)((e.X - myCardOffsetX) / myCardWidth);

            Invalidate();
        }

        /// <summary>
        /// Overrides the mouse clicked event, this is where cards get actually played
        /// </summary>
        /// <param name="e">The mouse event arguments, with the mouse's information</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (myCards != null && myHoverIndex > -1 && myHoverIndex < myCards.Count)
            {
                OnCardSelected?.Invoke(this, new CardEventArgs(myCards[myHoverIndex]));
            }
        }

        /// <summary>
        /// Overrides the OnPaint event, invoked when this control has been invalidated
        /// </summary>
        /// <param name="e">The paint event arguments with the graphics device to use for rendering</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Only draw cards if the collection exists and we have cards
            if (myCards != null && myCards.Count > 0)
            {
                // Iterate over all cards
                for (int index = 0; index < myCards.Count; index++)
                {
                    // Only render the card if we are not hovering on it
                    if (index != myHoverIndex)
                    {
                        // Get the image for the card
                        Image i = myCards[index].GetCardImage();

                        // Draw the card's image
                        e.Graphics.DrawImage(i, CalculateCardBounds(index));
                    }
                }

                // After rendering all normal cards, we need to render the hover card
                if (myHoverIndex > -1 && myHoverIndex < myCards.Count)
                {
                    // Get the card's image
                    Image i = myCards[myHoverIndex].GetCardImage();

                    // Get the bounds of the unexpanded card
                    RectangleF bounds = CalculateCardBounds(myHoverIndex);

                    // Expand the bounds
                    bounds.Location = new PointF(bounds.Left - EXPAND_SIZE / 2, bounds.Top - EXPAND_SIZE / 2);
                    bounds.Width += EXPAND_SIZE;
                    bounds.Height += EXPAND_SIZE;

                    // Draw the expanded image
                    e.Graphics.DrawImage(i, bounds);

                    // Draw a black outline
                    Pen p = Pens.Black;
                    e.Graphics.DrawRoundedRectangle(p, bounds, 4.0f);
                }
            }

            // Call the base OnPaint
            base.OnPaint(e);
        }

        /// <summary>
        /// Calculates the bounds of a single card
        /// </summary>
        /// <param name="index">The index of the card</param>
        /// <returns>The bounds of the card with the given index</returns>
        private RectangleF CalculateCardBounds(int index)
        {
            return new RectangleF(
                            myCardOffsetX + myCardWidth * index,
                            myCardOffsetY,
                            myCardWidth,
                            myCardHeight);
        }
    }
}
