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
    // Клас, що визначає, які карти можна використовувати для захисту
    public class DefendingRule : IGamePlayRule
    {
        // Отримує або встановлює, чи ввімкнено це правило
        public bool IsEnabled
        {
            get;
            set;
        }

        // Повертає читабельну назву цього правила
        public string ReadableName
        {
            get
            {
                return "правило захисника";
            }
        }

        // Перевіряє, чи є даний хід допустимим
        public bool IsValidMove(GameServer server, GameMove move, ref string reason)
        {
            if (move.Move == null)
                return true;

            if (!server.GameState.GetValueBool(Names.IS_ATTACKING))
            {
                if (server.GameState.GetValueByte(Names.DEFENDING_PLAYER) == move.Player.PlayerId)
                {
                    PlayingCard attacking = server.GameState.GetValueCard(Names.ATTACKING_CARD, server.GameState.GetValueInt(Names.CURRENT_ROUND));
                    PlayingCard trump = server.GameState.GetValueCard(Names.TRUMP_CARD);

                    if (move.Move.Rank > attacking.Rank)
                    {
                        if (move.Move.Suit == attacking.Suit || move.Move.Suit == trump.Suit)
                        {
                            return true;
                        }
                        else
                        {
                            reason = "Ви повинні зіграти карту вищого рангу тієї самої масті або козирну карту";
                            return false;
                        }
                    }
                    else if (move.Move.Suit == trump.Suit && attacking.Suit != trump.Suit)
                        return true;
                    else
                    {
                        reason = "Ви повинні зіграти карту вищого рангу";
                        return false;
                    }
                }
                else
                {
                    reason = "Зараз не ваша черга захищатися";
                    return false;
                }
            }
            else
                return true;
        }
    }
}
