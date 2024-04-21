using Durak.Common;
using Durak.Server;

namespace DurakGame.Rules
{
    // Клас для перевірки того, чи настала черга гравця грати карту
    public class VerifyTurnRule : IGamePlayRule
    {
        // Отримує або задає, чи увімкнене це правило
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
                return "Перевірка черги гравця";
            }
        }

        // Визначає, чи є хід допустимим
        public bool IsValidMove(GameServer server, GameMove move, ref string reason)
        {
            if (server.GameState.GetValueBool(Names.IS_ATTACKING))
            {
                if (move.Player.PlayerId == server.GameState.GetValueByte(Names.ATTACKING_PLAYER))

                    return true;

                if (move.Player.PlayerId != server.GameState.GetValueByte(Names.DEFENDING_PLAYER))
                    return true;
            }
            else
            {
                if (move.Player.PlayerId == server.GameState.GetValueByte(Names.DEFENDING_PLAYER))
                    return true;

            }

            reason = "Зараз не ваша черга " + (server.GameState.GetValueBool(Names.IS_ATTACKING) ? "атакувати." : "захищатися.");
            return false;
        }
    }
}
