using System;
using Durak.Common;
using Durak.Server;
using Durak.Common.Cards;

namespace DurakGame.Rules
{
    // Клас для налаштування початкового стану гри
    public class InitRoundStates : IGameInitRule
    {
        // Отримує або встановлює, чи це правило увімкнене
        public bool IsEnabled
        {
            get;
            set;
        }
        // Отримує пріоритет для цього правила, вищі пріоритети виконуються першими
        public int Priority
        {
            get
            {
                return 100;
            }
        }
        // Отримує зрозумілу назву для цього правила стану
        public string ReadableName
        {
            get
            {
                return "Ініціалізація стартових станів гри";
            }
        }

        // Ініціалізує стан гри, включаючи створення колоди та роздачу початкових карт
        public void InitState(GameServer server)
        {
            server.GameState.Set(Names.IS_ATTACKING, true);
            server.GameState.Set(Names.ATTACKER_FORFEIT, false);
            server.GameState.Set(Names.DEFENDER_FORFEIT, false);

            // Визначити початкових нападаючого та захищаючого гравців
            byte attackingPlayerId = 0;
            byte defendingPlayerId = 1;

            while (server.Players[attackingPlayerId] == null)
                attackingPlayerId = (byte)(attackingPlayerId + 1 >= server.Players.Count ? 0 : attackingPlayerId + 1);

            while (server.Players[defendingPlayerId] == null)
                defendingPlayerId = (byte)(defendingPlayerId + 1 >= server.Players.Count ? 0 : defendingPlayerId + 1);

            server.GameState.Set(Names.ATTACKING_PLAYER, attackingPlayerId);
            server.GameState.Set(Names.DEFENDING_PLAYER, defendingPlayerId);

            // Побудувати колоду
            Deck deck;

            int numInitCards = server.GameState.GetValueInt(Names.NUM_INIT_CARDS);

            numInitCards = 36;
            deck = new Deck(CardRank.Six, CardRank.Ace);

            deck.Shuffle();

            // Витягти козирну карту
            server.GameState.Set<PlayingCard>(Names.TRUMP_CARD, deck.Draw());

            // Встановити колоду на стані серверу
            server.GameState.Set<int>(Names.DECK_COUNT, deck.CardsInDeck);
            server.GameState.Set<CardCollection>(Names.DECK, deck.GetCards(), true);

            // Створити стопку відкинутих карт
            server.GameState.Set(Names.DISCARD, new CardCollection());

            // Ініціалізувати бойові слоти
            for (int index = 0; index < 6; index++)
            {
                server.GameState.Set<PlayingCard>(Names.DEFENDING_CARD, index, null);
                server.GameState.Set<PlayingCard>(Names.ATTACKING_CARD, index, null);
            }
        }
    }
}
