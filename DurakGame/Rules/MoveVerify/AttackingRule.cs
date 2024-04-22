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
    // Клас, який перевіряє, чи може атакуючий гравець зіграти карту
    public class AttackingRule : IGamePlayRule
    {
        // Отримує або задає, чи це правило ввімкнене
        public bool IsEnabled
        {
            get;
            set;
        }

        // Отримує читабельне ім'я для цього правила
        public string ReadableName
        {
            get
            {
                return "правило для атакуючого";
            }
        }

        // Перевіряє, чи хід є дійсним
        public bool IsValidMove(GameServer server, GameMove move, ref string reason)
        {
            if (server.GameState.GetValueBool(Names.IS_ATTACKING) && (move.Player.PlayerId == server.GameState.GetValueByte(Names.ATTACKING_PLAYER)))
            {
                if (move.Move == null)
                {
                    if (server.GameState.GetValueByte(Names.ATTACKING_PLAYER) == move.Player.PlayerId)
                        return true;
                    else
                    {
                        reason = "Ви не можете здатися від імені іншого гравця";
                        return false;
                    }
                }

                // Додаємо перевірку на кількість карт, які може зіграти атакуючий
                int defenderCardCount = server.Players[server.GameState.GetValueByte(Names.DEFENDING_PLAYER)].Hand.Count;
                int currentAttacks = server.GameState.GetValueInt(Names.CURRENT_ROUND);

                if (currentAttacks >= defenderCardCount)
                {
                    reason = "Захисник не має достатньо карт, щоб захиститися від додаткових атак";
                    return false;
                }

                int round = server.GameState.GetValueInt(Names.CURRENT_ROUND);

                if (round == 0)
                {
                    return true;
                }
                else
                {
                    bool canPlay = false;

                    for (int index = 0; index < round; index++)
                    {
                        if (
                            move.Move.Rank == server.GameState.GetValueCard(Names.ATTACKING_CARD, index).Rank ||
                            move.Move.Rank == server.GameState.GetValueCard(Names.DEFENDING_CARD, index).Rank)
                            canPlay = true;
                    }

                    if (!canPlay)
                        reason = "Ви повинні зіграти карту з рангом, який вже був зіграний";

                    return canPlay;
                }
            }
            else if (!server.GameState.GetValueBool(Names.IS_ATTACKING))
                return true;
            else
                return false;
        }
    }
}
