using Durak.Common;
using Durak.Common.Cards;
using Durak.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurakGame.Rules
{
    // Клас, який відповідає за роздачу карт гравцям на початку гри
    class InitCardsRule : IGameInitRule
    {
        public bool IsEnabled
        {
            get;
            set;
        }

        public int Priority
        {
            get
            {
                return 50;
            }
        }

        public string ReadableName
        {
            get { return "Роздача карт"; }
        }

        public void InitState(GameServer server)
        {
            // Отримати колоду
            Deck toDrawFrom = new Deck(server.GameState.GetValueCardCollection(Names.DECK));

            // Пройтися по всім гравцям
            for (byte index = 0; index < server.Players.Count; index++)
            {
                // Якщо гравець існує
                if (server.Players[index] != null)
                {
                    // Додати 6 карт до його руки
                    for (int cIndex = 0; cIndex < 6; cIndex++)
                    {
                        if (toDrawFrom.CardsInDeck > 0)
                            server.Players[index].Hand.Add(toDrawFrom.Draw());
                        else if (!server.GameState.GetValueBool(Names.TRUMP_CARD_USED))
                        {
                            server.Players[index].Hand.Add(server.GameState.GetValueCard(Names.TRUMP_CARD));
                            server.GameState.Set(Names.TRUMP_CARD_USED, true);
                        }
                    }
                }
            }
            // Оновити колоду та кількість карт у стані
            server.GameState.Set<CardCollection>(Names.DECK, toDrawFrom.Cards);
            server.GameState.Set<int>(Names.DECK_COUNT, toDrawFrom.CardsInDeck);
        }
    }
}
