using Durak.Server;
using System;
using Durak.Common;

namespace DurakTester.Rules
{
    public class BotRule : IBotInvokeStateChecker
    {
        public bool ShouldInvoke(GameState state, BotPlayer player)
        {
            return 
                (state.GetValueByte("attacking_player_id") == player.Player.PlayerId & state.GetValueBool("IsAttacking")) | 
                (state.GetValueByte("defending_player_id") == player.Player.PlayerId & !state.GetValueBool("IsAttacking"));
        }
    }
}
