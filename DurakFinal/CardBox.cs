using System;
using System.Drawing;
using System.Windows.Forms;
using Durak.Common.Cards;
using System.Drawing.Imaging;

namespace Durak
{
    // Елемент управління, що використовується у додатках Windows Forms для відображення гральної карти.
    public partial class CardBox : UserControl
    {

        #region ПОЛЯ І ВЛАСТИВОСТІ

        // Зберігає об'єкт карти
        private PlayingCard myCard;
        // Зберігає зображення відображеної карти
        private Image myImage;
        // Властивість картки:
        // Встановлює / отримує базовий об'єкт картки
        public PlayingCard Card
        {
            get { return myCard; }
            set
            {
                myCard = value;

                if (myCard != null)
                    myImage = myCard.GetCardImage();

                Invalidate();
            }
        }

        // Властивість масті:
        // Встановлює / отримує масть базового об'єкта картки
        public CardSuit Suit
        {
            get { return Card.Suit; }
            set
            {
                Card.Suit = value;
                Invalidate();
            }
        }

        // Властивість рангу:
        // Встановлює / отримує ранг базового об'єкта картки
        public CardRank Rank
        {
            get { return Card.Rank; }
            set
            {
                Card.Rank = value;
                Invalidate();
            }
        }

        // Властивість FaceUp:
        // Встановлює / отримує властивість FaceUp базового об'єкта картки
        public bool FaceUp
        {
            get { return Card.FaceUp; }
            set
            {
                // якщо значення відрізняється від властивості FaceUp базової картки
                if (myCard.FaceUp != value) // тоді картка перевертається
                {
                    myCard.FaceUp = value; // змінити властивість FaceUp картки
                    Invalidate();

                    // якщо існує обробник події для CardFlipped у клієнтській програмі
                    CardFlipped?.Invoke(this, new EventArgs()); // викликати його
                }
            }
        }

        // Властивість CardOrientation:
        // Встановлює / отримує орієнтацію картки.
        // якщо встановлення цієї властивості змінює стан елементу керування, то
        // обмінює висоту і ширину елемента керування та оновлює зображення.
        private Orientation myOrientation;
        // Отримує або задає орієнтацію картки
        public Orientation CardOrientation
        {
            get { return myOrientation; }
            set
            {
                // якщо значення відрізняється від myOrientation
                if (myOrientation != value)
                {
                    myOrientation = value; // змінити орієнтацію
                    // обміняти висоту і ширину елемента керування
                    this.Size = new Size(Size.Height, Size.Width);
                    Invalidate();
                }
            }
        }

        // Отримує параметри створення WinForms нижнього рівня, додаючи прозорість
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20; 
                return cp;
            }
        }

        #endregion

        #region КОНСТРУКТОРИ

        // Конструктор (За замовчуванням):
        // Конструює елемент управління з новою карткою, орієнтованою вертикально
        public CardBox()
        {
            InitializeComponent(); // обов'язковий метод для підтримки дизайнера
            myOrientation = Orientation.Vertical; // встановити орієнтацію на вертикальну
            myCard = new PlayingCard(); // створити нову базову карту
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        // Конструктор (PlayingCard, Orientation): Конструює елемент управління за допомогою параметрів
        public CardBox(PlayingCard card, Orientation orientation = Orientation.Vertical) : this()
        {
            myOrientation = orientation; // встановити орієнтацію
            myCard = card; // встановити базову картку
        }
        #endregion

        #region ПОДІЇ ТА ОБРОБНИКИ ПОДІЙ

        // Подія, яку може обробити клієнтська програма, коли карта перевертається вверх / вниз
        public event EventHandler CardFlipped;

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        // Перевизначає подію OnPaint, відображаючи цю карту
        // Аргументи події з графічним пристроєм для відображення
        protected override void OnPaint(PaintEventArgs e)
        {
            if (myCard != null && myCard.GetCardImage() != null)
            {
                Color disabledColor = Color.FromArgb(Enabled ? 0 : 128, 128, 128, 128);

                using (Brush brush = new SolidBrush(disabledColor))
                {
                    e.Graphics.DrawImage(myCard.GetCardImage(), 0, 0, Width, Height);
                    e.Graphics.FillRectangle(brush, 0, 0, Width, Height);
                }
            }
        }

        #endregion

        #region ІНШІ МЕТОДИ

        // ToString: Перевизначає метод System.Object.ToString()
        public override string ToString()
        {
            return myCard.ToString();
        }


        #endregion

    }
}