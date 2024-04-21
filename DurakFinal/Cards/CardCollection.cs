using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak.Common.Cards
{
    // Представляє колекцію гральних карт
    public class CardCollection : IEnumerable<PlayingCard>, ICloneable
    {
        // Зберігає базовий список
        private List<PlayingCard> myBackingList;

        // Викликається, коли карта додається до цієї колекції
        public event EventHandler<CardEventArgs> OnCardAdded;
        // Викликається, коли карта видаляється з цієї колекції
        public event EventHandler<CardEventArgs> OnCardRemoved;

        // Отримує кількість карт у цій колекції
        public int Count
        {
            get { return myBackingList.Count; }
        }

        // Отримує або задає карту за вказаним індексом у цій колекції
        public PlayingCard this[int index]
        {
            get { return myBackingList[index]; }
            set { myBackingList[index] = value; }
        }

        // Створює новий порожній екземпляр колекції карт
        public CardCollection()
        {
            myBackingList = new List<PlayingCard>();
        }

        // Перевіряє, чи міститься в цій колекції карта
        public bool Contains(PlayingCard move)
        {
            return myBackingList.Contains(move);
        }

        // Клонує цю колекцію
        public object Clone()
        {
            // Створюємо результат
            CardCollection newCards = new CardCollection();

            // Клонуємо кожну карту
            foreach (PlayingCard sourceCard in this)
                newCards.Add(sourceCard.Clone() as PlayingCard);

            // Повертаємо результат
            return newCards;
        }

        // Видаляє всі карти з цієї колекції
        public void Clear()
        {
            while (Count > 0)
                Discard(myBackingList[0]);
        }

        // Очищує всі карти у цій колекції без виклику подій
        public void SilentClear()
        {
            myBackingList.Clear();
        }

        // Перемішує карти у цій колекції
        public void Shuffle()
        {
            // Текстовий файл був абсолютно неадекватним, тому я зробив свій власний, що є справжньо випадковим
            Random rand = new Random();

            // Випадково вибрати кількість разів для повторення
            int loopCount = rand.Next(3, 5);

            // Перебираємо кожну карту
            for (int index = 0; index < Count * loopCount; index++)
            {
                // Знаходимо карту для обміну
                int newPos = rand.Next(0, Count);

                // Обмін картами
                PlayingCard swap = this[newPos];
                this[newPos] = this[index % Count];
                this[index % Count] = swap;
            }
        }

        // Додає карту до цієї колекції
        public void Add(PlayingCard card)
        {
            myBackingList.Add(card);

            if (OnCardAdded != null)
                OnCardAdded.Invoke(this, new CardEventArgs(card));
        }

        // Видаляє карту за вказаним індексом
        public void DiscardAt(int index)
        {
            Discard(myBackingList[index]);
        }

        // Видаляє карту з цієї колекції
        public void Discard(PlayingCard card)
        {
            myBackingList.Remove(card);

            if (OnCardRemoved != null)
                OnCardRemoved.Invoke(this, new CardEventArgs(card));
        }

        // Отримує перебірник для цієї колекції, це просто викликає перебірник базового списку
        public IEnumerator<PlayingCard> GetEnumerator()
        {
            return myBackingList.GetEnumerator();
        }

        // Отримує перебірник для цієї колекції, це просто викликає перебірник базового списку
        IEnumerator IEnumerable.GetEnumerator()
        {
            return myBackingList.GetEnumerator();
        }
    }
}
