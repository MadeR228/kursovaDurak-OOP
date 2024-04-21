using Durak.Server;
using System;
using Durak.Common;

namespace DurakTester.Rules
{
    /// <summary>
    /// This class is used to check the state of each bot after each message pump to determine the state, wheather they should be attacking, defending, asking for help 
    /// </summary>
    public class VerifyBotState : IBotInvokeStateChecker
    {
        public bool ShouldInvoke(GameState state, BotPlayer player)
        {
            return 
                //Check if the attacking players id is equal to the bots id and it is currently attacking
                (state.GetValueByte("attacking_player_id") == player.Player.PlayerId & state.GetValueBool("IsAttacking")) | 
                
                //Check if the defending players id is equal to the bots id and if it is currently defending 
                (state.GetValueByte("defending_player_id") == player.Player.PlayerId & !state.GetValueBool("IsAttacking")) | 

                //Checks the games state, if a player requires help and the bot players has the same value of the attacking player, this bot is asking for help
                (state.GetValueBool("player_req_help") == true && state.GetValueByte("defending_player_id") != player.Player.PlayerId);
        }
    }
}
