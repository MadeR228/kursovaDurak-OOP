using Durak.Common;
using Durak.Server;

namespace DurakTester.Rules
{
    // Клас, який перевіряє, чи карта знаходиться в руці гравця
    public class VerifyCardInHand : IGamePlayRule
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
                return "Перевірка карти в руці";
            }
        }

        // Визначає, чи є даний хід допустимим
        public bool IsValidMove(GameServer server, GameMove move, ref string reason)
        {
            if (move.Move == null)
                return true;

            if (!move.Player.Hand.Contains(move.Move))
            {
                reason = "Карта не знаходиться в руці гравця";
                return false;
            }

            return true;
        }
    }
}
