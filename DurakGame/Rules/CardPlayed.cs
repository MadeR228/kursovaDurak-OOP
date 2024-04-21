using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Durak.Server;
using Durak.Common;
using static System.Windows.Forms.AxHost;


namespace DurakGame.Rules
{
    // Клас, який викликається після успішної зіграної карти
    public class CardPlayed : IMoveSuccessRule
    {
        // Оновлює стан гри після відтворення карти
        public void UpdateState(GameServer server, GameMove move)
        {
            // Видалити карту з руки гравця
            if (move.Move != null)
                move.Player.Hand.Discard(move.Move);

            // Ми маємо перейти в залежності від того, чи ми атакуємо
            if (server.GameState.GetValueBool(Names.IS_ATTACKING))
            {
                // Якщо вони відтворили нульову карту, вони відмовилися
                if (move.Move == null)
                    server.GameState.Set(Names.ATTACKER_FORFEIT, true);
                // В іншому випадку, помістіть карту в потрібний слот
                else
                    server.GameState.Set(Names.ATTACKING_CARD, server.GameState.GetValueInt(Names.CURRENT_ROUND), move.Move);

                // Тепер ми захищаємось
                server.GameState.Set<bool>(Names.IS_ATTACKING, false);
            }
            else
            {
                // Якщо вони відтворили нульову карту, вони відмовилися
                if (move.Move == null)
                    server.GameState.Set(Names.DEFENDER_FORFEIT, true);
                // В іншому випадку, помістіть карту в потрібний слот
                else
                    server.GameState.Set(Names.DEFENDING_CARD, server.GameState.GetValueInt(Names.CURRENT_ROUND), move.Move);

                // Тепер ми атакуємо
                server.GameState.Set<bool>(Names.IS_ATTACKING, true);

                // Перехід до наступного раунду, ми перевіримо перемогу захисника в правилі перевірки стану
                server.GameState.Set<int>(Names.CURRENT_ROUND, server.GameState.GetValueInt(Names.CURRENT_ROUND) + 1);
            }
        }
    }
}