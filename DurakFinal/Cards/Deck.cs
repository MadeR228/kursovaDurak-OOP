using System;
using System.Collections.Generic;

namespace Durak.Common.Cards
{
    // Делегат, який використовується для обробки події, коли остання карта витягнута з вказаної колоди
    public delegate void LastCardDrawnHandler(Deck currentDeck);

    // Представляє колоду карт, яка містить всі можливі карти при створенні
    public class Deck : ICloneable
    {
        // Викликається, коли остання карта витягнута з цієї колоди
        public event LastCardDrawnHandler OnLastCardDrawn;

        // Отримує карти в цій колоді
        public CardCollection GetCards()
        {
            return cards.Clone() as CardCollection;
        }

        // Внутрішня колекція карт
        private CardCollection cards = new CardCollection();

        // Конструктор за замовчуванням - створює нову колоду карт
        public Deck()
        {
            InsertCards(3, 14);
        }


        // Параметризований конструктор - створює колоду з заданою колекцією
        public Deck(CardCollection newCards)
        {
            cards = newCards;
        }

        // Параметризований конструктор - створює нову колоду карт
        public Deck(CardRank minRank, CardRank maxRank)
        {
            InsertCards((int)minRank, (int)maxRank + 1);
        }

        // Вставляє кожну карту зі звичайної колоди в цю колоду
        private void InsertCards(int minRankIndex, int maxRankIndex)
        {
            for (int suitVal = 0; suitVal < 4; suitVal++)
            {
                for (int rankVal = minRankIndex; rankVal < maxRankIndex; rankVal++)
                {
                    cards.Add(new PlayingCard((CardRank)rankVal, (CardSuit)suitVal));
                }
            }
        }

        // Отримує кількість карт, що залишилися в цій колоді
        public int CardsInDeck
        {
            get { return cards.Count; }
        }

        // Отримує колекцію карт, навколо якої обертається ця колода
        public CardCollection Cards
        {
            get { return cards; }
        }

        // Перемішує карти в цій колоді
        public void Shuffle()
        {
            cards.Shuffle();
        }

        // Витягує наступну карту з колоди
        public PlayingCard Draw()
        {
            // Якщо карт немає, нічого не повертайте
            if (cards.Count == 0)
                return null;
            else
            {
                // Отримуємо першу карту, а потім видаляємо її
                var card = cards[0];
                cards.DiscardAt(0);

                // Якщо це була остання карта, викличемо нашу подію
                if (cards.Count == 0 && OnLastCardDrawn != null)
                    OnLastCardDrawn(this);

                // Повертаємо результат
                return card;
            }
        }

        // Спроба вибрати карту певної масті з колоди
        // Найкраща карта, що відповідає масти, це поверне випадкову карту, якщо карт такої масті немає
        public PlayingCard SelectCardOfSpecificSuit(CardSuit suit)
        {
            // Отримуємо результат
            PlayingCard selectedCard = null;

            // Спробуємо отримати карту відповідної масті
            foreach (PlayingCard card in cards)
            {
                if (card.Suit == suit)
                {
                    selectedCard = card;
                    break;
                }
            }

            // Якщо карта є нульовою, витягнемо випадкову
            if (selectedCard == null)
                return Draw();
            // Якщо ми знайшли карту, переконайтесь, що її видалили з колоди
            else
                cards.Discard(selectedCard);

            // Повертаємо результат
            return selectedCard;
        }

        // Клонує колоду карт
        public object Clone()
        {
            Deck newDeck = new Deck(cards.Clone() as CardCollection);
            return newDeck;
        }
    }
}
