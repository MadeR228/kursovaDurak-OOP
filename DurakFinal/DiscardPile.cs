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
using System.Drawing.Imaging;

namespace Durak
{
    /// <summary>
    /// Represents a pile of discarded cards
    /// </summary>
    public partial class DiscardPile : UserControl
    {
        /// <summary>
        /// Represents a single instance of a card in this discard pile
        /// </summary>
        private struct CardInstance
        {
            /// <summary>
            /// The playing card to represent
            /// </summary>
            public PlayingCard Card;
            /// <summary>
            /// The scale of the image
            /// </summary>
            public float Scale;
            /// <summary>
            /// The image's center
            /// </summary>
            public PointF Center;
            /// <summary>
            /// The rotation of the image, in degrees
            /// </summary>
            public float Rotation;

            /// <summary>
            /// Creates a new card instance
            /// </summary>
            /// <param name="card">The card to represent</param>
            /// <param name="rotation">The rotation of the card in degrees</param>
            /// <param name="center">The centerpoint of the card</param>
            public CardInstance(PlayingCard card, float rotation, PointF center)
            {
                Card = card;
                Rotation = rotation;
                Center = center;
                Scale = 1;
            }
        }

        /// <summary>
        /// The list of card instances
        /// </summary>
        private List<CardInstance> myInstances;
        /// <summary>
        /// The random number generator to use
        /// </summary>
        private Random myRandom;

        /// <summary>
        /// The width of card scaled to 1.0
        /// </summary>
        private const float CARD_WIDTH = 48;
        /// <summary>
        /// The height of card scaled to 1.0
        /// </summary>
        private const float CARD_HEIGHT = 64;
        
        /// <summary>
        /// Creates a new card discard pile
        /// </summary>
        public DiscardPile()
        {
            InitializeComponent();

            myInstances = new List<CardInstance>();
            myRandom = new Random();
        }

        /// <summary>
        /// Clears this discard pile of all cards
        /// </summary>
        public void Clear()
        {
            myInstances.Clear();
            Invalidate();
        }

        /// <summary>
        /// Adds the given card to this discard pile
        /// </summary>
        /// <param name="card">The card to add</param>
        public void AddCard(PlayingCard card)
        {
            float offset = (CARD_WIDTH + CARD_HEIGHT) / 2.0f;
            myInstances.Add(new CardInstance(card, myRandom.Next(0, 360), new PointF(myRandom.Next((int)offset, Width - (int)offset), myRandom.Next((int)(offset), Height - (int)offset))));
            Invalidate();
        }

        /// <summary>
        /// Overrides the OnPaint event for this control
        /// </summary>
        /// <param name="e">The event arguments, with the graphics device to render with</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            foreach(CardInstance instance in myInstances)
            {
                e.Graphics.TranslateTransform(instance.Center.X, instance.Center.Y);
                e.Graphics.RotateTransform(instance.Rotation);

                e.Graphics.DrawImage(instance.Card.GetCardImage(), 0, 0, CARD_WIDTH, CARD_HEIGHT);

                e.Graphics.ResetTransform();
            }
        }
    }
}
