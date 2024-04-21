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
    // Клас-утиліта, що використовується правилами Дурень для виконання дій, загальних для декількох правил
    static class Utils
    {
        // Обробляє перехід до наступного поєдинку, це також перевіряє кінець гри і встановлює параметр GAME_OVER в true, якщо потрібно
        public static void MoveNextDuel(GameServer server)
        {
            GameState state = server.GameState;
            PlayerCollection players = server.Players;

            if (!state.GetValueBool(Names.GAME_OVER))
            {
                // Спочатку визначте кількість активних гравців
                int activePlayers = players.Where(X => X.Hand.Count > 0).Count();

                // Якщо залишилося більше 1 гравця, продовжуйте грати
                if (activePlayers > 1)
                {
                    // Очистити стани, специфічні для бою
                    state.Set(Names.IS_ATTACKING, true);
                    state.Set(Names.ATTACKER_FORFEIT, false);
                    state.Set(Names.DEFENDER_FORFEIT, false);

                    // Очистити стани, специфічні для матчу
                    state.Set(Names.CURRENT_ROUND, 0);

                    // Отримайте ідентифікатор нападаючого гравця, це перша особа, яка отримує нові карти
                    byte id = state.GetValueByte(Names.ATTACKING_PLAYER);
                    byte startId = id;

                    // Отримайте колоду зі стану
                    Deck deck = new Deck(state.GetValueCardCollection(Names.DECK));

                    bool go = true;

                    while (go)
                    {
                        Player player = players[id];

                        if (player != null)
                        {
                            while (player.Hand.Count < 6)
                            {
                                if (deck.Cards.Count > 0)
                                {
                                    player.Hand.Add(deck.Draw());
                                }
                                else if (!state.GetValueBool(Names.TRUMP_CARD_USED))
                                {
                                    player.Hand.Add(state.GetValueCard(Names.TRUMP_CARD));
                                    state.Set<bool>(Names.TRUMP_CARD_USED, true);
                                    go = false;
                                    break;
                                }
                                else
                                    break;
                            }
                        }

                        id++;

                        if (id >= players.Count)
                            id = 0;
                        if (id == startId)
                            break;
                    }

                    // Отримати інформацію про нових нападаючих та захищаючих
                    byte attackingPlayerId = (byte)(state.GetValueByte(Names.ATTACKING_PLAYER));
                    attackingPlayerId++;
                    // Ітерації, поки не знайдете дійсного нападаючого                    
                    for (byte index = attackingPlayerId, iterations = 0; iterations < players.Count; index++, iterations++)
                    {
                        index = (byte)(index >= players.Count ? players.Count - index : index);

                        if (players[index] != null && players[index].Hand.Count > 0)
                        {
                            attackingPlayerId = index;
                            break;
                        }
                    }

                    byte defendingPlayerId = (byte)(attackingPlayerId + 1 >= players.Count ? players.Count - (attackingPlayerId + 1) : attackingPlayerId + 1);

                    // Ітерації, поки не знайдете дійсного захищаючогося       
                    for (byte index = defendingPlayerId, iterations = 0; iterations < players.Count; index++, iterations++)
                    {
                        index = (byte)(index >= players.Count ? players.Count - index : index);

                        if (players[index] != null && players[index].Hand.Count > 0)
                        {
                            defendingPlayerId = index;
                            break;
                        }
                    }

                    // Оновлює нападаючого та захищаючогося
                    state.Set(Names.ATTACKING_PLAYER, attackingPlayerId);
                    state.Set(Names.DEFENDING_PLAYER, defendingPlayerId);

                    state.Set(Names.DECK, deck.Cards);
                    state.Set(Names.DECK_COUNT, deck.CardsInDeck);
                }
                else
                {
                    if (activePlayers == 1)
                    {
                        state.Set<byte>(Names.LOSER_ID, players.Where(X => X.Hand.Count > 0).ElementAt(0).PlayerId);
                        state.Set<bool>(Names.IS_TIE, false);
                        state.Set<bool>(Names.GAME_OVER, true);
                    }
                    else if (activePlayers == 0)
                    {
                        state.Set<bool>(Names.IS_TIE, true);
                        state.Set<bool>(Names.GAME_OVER, true);
                    }
                }
            }
        }
    }
}
