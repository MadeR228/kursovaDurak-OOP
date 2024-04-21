using Durak.Server;
using System;
using Durak.Common;
using Durak.Common.Cards;

namespace DurakGame.Rules
{
    // Цей клас використовується для перевірки стану кожного бота після кожного запиту для визначення стану: чи вони мають атакувати, захищатися, чи просити допомоги
    public class VerifyBotState : IBotInvokeStateChecker
    {
        // Реалізує інтерфейс перевірки стану бота
        public bool ShouldInvoke(GameServer server, BotPlayer player)
        {
            GameState state = server.GameState;

            if (state.GetValueBool(Names.GAME_OVER))
                return false;

            byte attackingPlayerID = state.GetValueByte(Names.ATTACKING_PLAYER);
            byte defendingPlayerId = state.GetValueByte(Names.DEFENDING_PLAYER);
            bool isAttacking = state.GetValueBool(Names.IS_ATTACKING);
            int currentRound = state.GetValueInt(Names.CURRENT_ROUND);
            PlayingCard attackingCard = state.GetValueCard(Names.ATTACKING_CARD, currentRound);
            PlayingCard defendingCard = state.GetValueCard(Names.DEFENDING_CARD, currentRound);

            if (isAttacking)
            {
                if (attackingPlayerID == player.Player.PlayerId)
                    return true;
            }
            else
            {
                if (defendingPlayerId == player.Player.PlayerId && attackingCard != null)
                    return true;
            }

            return false;
        }
    }
}
