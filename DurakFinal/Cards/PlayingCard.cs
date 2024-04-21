using System;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Durak.Common.Cards
{
    // Представляє просту ігрову карту з багаторазовим використанням
    public class PlayingCard : ICloneable, IComparable
    {

        #region ПОЛЯ І ВЛАСТИВОСТІ

        // Запасне поле для масті
        protected CardSuit mySuit;
        // Отримує або встановлює масть цієї ігрової карти
        public CardSuit Suit
        {
            get { return mySuit; } // повертає масть
            set { mySuit = value; } // встановлює масть
        }

        // Запасне поле для рангу
        protected CardRank myRank;
        // Отримує або встановлює ранг цієї ігрової карти
        public CardRank Rank
        {
            get { return myRank; } // повертає ранг
            set { myRank = value; } // встановлює ранг
        }
        // Запасне поле для значення карти
        protected int myValue;
        // Отримує або встановлює значення цієї карти
        public int CardValue
        {
            get { return myValue; } // повертає значення
            set { myValue = value; } // встановлює значення
        }

        // Запасне поле для альтернативного значення
        protected int? altValue = null;
        // Отримує або встановлює альтернативне значення цієї карти
        public int? AlternateValue
        {
            get { return altValue; } // повертає альтернативне значення
            set { altValue = value; } // встановлює альтернативне значення
        }

        // Запасне поле для FaceUp
        protected bool faceUp = false;
        // Отримує або встановлює, чи є ця карта обличчям вверх
        public bool FaceUp
        {
            get { return faceUp; } // повертає значення faceup
            set { faceUp = value; } // встановлює значення faceup
        }

        #endregion

        #region КОНСТРУКТОРИ

        // Створює нову ігрову карту за замовчуванням, це буде Туз Піковий
        public PlayingCard() : this(CardRank.Ace, CardSuit.Spades)
        {

        }

        // Конструктор карти
        // Ініціалізує об'єкт ігрової карти. За замовчуванням карта обличчям вниз, без альтернативного значення
        public PlayingCard(CardRank rank = CardRank.Ace, CardSuit suit = CardSuit.Hearts)
        {
            // встановлює ранг, масть
            this.myRank = rank;
            this.mySuit = suit;
            // встановлює значення карти за замовчуванням
            this.myValue = (int)rank;
        }

        #endregion

        #region ПУБЛІЧНІ МЕТОДИ

        // Метод CompareTo
        // Специфічний для карти метод порівняння, використовується для сортування екземплярів Card. Порівнює цей екземпляр з об'єктом карти.
        public virtual int CompareTo(object obj)
        {
            // чи аргумент є нульовим?
            if (obj == null)
            {
                // викидає виняток про нульовий аргумент
                throw new ArgumentNullException("Неможливо порівняти карту з нульовим об'єктом");
            }
            // конвертує аргумент в Card
            PlayingCard compareCard = obj as PlayingCard;
            // якщо конвертація вдалася
            if (compareCard != null)
            {
                // порівняти спочатку за значенням, а потім за масть.
                int thisSort = this.myValue * 10 + (int)this.mySuit;
                int compareCardSort = compareCard.myValue * 10 + (int)compareCard.mySuit;
                return (thisSort.CompareTo(compareCardSort));
            }
            else // в іншому випадку конвертація не вдалася
            {
                // викинути виняток про неприпустимий аргумент
                throw new ArgumentException("Об'єкт, з яким порівнюють, не може бути перетворений у карту.");
            }
        }

        // Метод Clone
        // Для підтримки інтерфейсу ICloneable. Використовується для глибокого копіювання в класах колекцій карт.
        public object Clone()
        {
            return this.MemberwiseClone();  // повертає поверхневу копію.
        }

        // ToString: Перевизначає метод System.Object.ToString()
        public override string ToString()
        {
            string cardString; // зберігає ім'я ігрової карти.
            // якщо карта обличчям вгору
            if (faceUp)
            {
                // встановити рядок імені карти на ранг масті
                cardString = myRank.ToString() + " of " + mySuit.ToString();
            }
            // інакше карта обличчям вниз.
            else
            {
                // встановити ім'я карти на обличчя вниз
                cardString = "Face Down";
            }
            // повернути відповідний рядок імені карти
            return cardString;
        }

        // Перевизначає метод System.Object.Equals()
        // true, якщо значення карт однакові
        public override bool Equals(object obj)
        {
            return obj is PlayingCard && (this.Rank == (obj as PlayingCard).Rank && this.Suit == (obj as PlayingCard).Suit);
        }

        // Перевизначає метод System.Object.GetHashCode()
        // Значення карти * 10 + номер масти
        public override int GetHashCode()
        {
            return this.myValue * 100 + (int)this.mySuit * 10 + ((this.faceUp) ? 1 : 0);
        }

        // Отримує зображення, що відповідає ігровій карті з ресурсного файлу.
        public Image GetCardImage()
        {
            string imageName; // ім'я зображення у файлі ресурсів
            Image cardImage; // зберігає зображення
            // якщо карта не обличчям вгору
            if (!faceUp)
            {
                // встановити ім'я зображення на "Задня сторона"
                imageName = "Back"; // встановлює його як ім'я зображення для задньої сторони картки
            }
            else // інакше, якщо карта обличчям вгору
            {
                // встановити ім'я зображення на {Масть}_{Ранг}
                imageName = mySuit.ToString() + "_" + myRank.ToString(); 
            }
            // встановити зображення відповідного об'єкта з ресурсного файлу
            cardImage = Durak.Properties.Resources.ResourceManager.GetObject(imageName) as Image;
            // повернути зображення
            return cardImage;
        }

        // Генерує рядок, що відображає стан об'єкта карти
        public string DebugString()
        {
            string cardState = (string)(myRank.ToString() + " of " + mySuit.ToString()).PadLeft(20);
            cardState += (string)((FaceUp) ? "(Face Up)" : "(Face Down)").PadLeft(12);
            cardState += "Value: " + myValue.ToString().PadLeft(2);
            cardState += ((altValue != null) ? "/" + altValue.ToString() : "");
            return cardState;
        }

        #endregion

        #region ПОВ'ЯЗАНІ ОПЕРАТОРИ

        // Визначає, чи є дві карти однаковими за значенням
        public static bool operator == (PlayingCard left, PlayingCard right)
        {
            if (!object.ReferenceEquals(left, null) && !object.ReferenceEquals(right, null))
                // return the result of CardA == CardB
                return (left.Suit == right.Suit && left.Rank == right.Rank);
            else
                return object.ReferenceEquals(left, null) && object.ReferenceEquals(left, null);
        }

        // Визначає, чи не є дві карти однаковими за значенням
        public static bool operator != (PlayingCard left, PlayingCard right)
        {
            return !(left == null);
        }

        // Визначає, чи карта менше іншої карти за значенням
        public static bool operator <(PlayingCard left, PlayingCard right)
        {
            // повертає результат CardA < CardB
            return (left.CardValue < right.CardValue);
        }

        // Визначає, чи карта менше або дорівнює іншій карти за значенням
        public static bool operator <= (PlayingCard left, PlayingCard right)
        {
            // повертає результат CardA <= CardB
            return (left.CardValue <= right.CardValue);
        }
        // Визначає, чи карта більше іншої карти за значенням
        public static bool operator > (PlayingCard left, PlayingCard right)
        {
            // повертає результат CardA > CardB
            return (left.CardValue > right.CardValue);
        }

        // Визначає, чи карта більше або дорівнює іншій карти за значенням
        public static bool operator >= (PlayingCard left, PlayingCard right)
        {
            // повертає результат CardA >= CardB
            return (left.CardValue >= right.CardValue);
        }

        #endregion

    } 
} 