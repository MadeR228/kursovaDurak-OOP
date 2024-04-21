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
    // Перевіряє, чи захищаючий гравець програв, і оновлює стан гри відповідно
    public class DefenderLose : IGameStateRule
    {
        // Отримує або встановлює, чи це правило увімкнене
        public bool IsEnabled
        {
            get;
            set;
        }

        // Отримує зрозумілу для людини назву для цього правила
        public string ReadableName
        {
            get
            {
                return "Перевірка, чи захисник програв, і перехід до наступного раунду";
            }
        }

        // Обробляє перевірку стану сервера. Якщо захисник програв, це відповідає за перехід до наступного раунду
        public void ValidateState(GameServer server)
        {

            PlayerCollection players = server.Players;

            // Перевірити, чи захисник відмовився від гри
            if (server.GameState.GetValueBool(Names.DEFENDER_FORFEIT))
            {
                // Отримати поточний раунд та захищаючогося гравця
                int round = server.GameState.GetValueInt(Names.CURRENT_ROUND);
                Player defender = server.Players[server.GameState.GetValueByte(Names.DEFENDING_PLAYER)];
                int activePlayers = players.Where(X => X.Hand.Count > 0).Count();

                // Перехід до наступного нападаючого гравця
                byte attackingPlayerId = server.GameState.GetValueByte(Names.ATTACKING_PLAYER);
                byte nextAttackingPlayerId = (byte)((attackingPlayerId + 1) % activePlayers);

                // Пройтися по всіх попередніх раундах
                for (int index = 0; index < round; index++)
                {
                    // Додати карту нападаючого та захисника назад до руки захисника
                    PlayingCard card1 = server.GameState.GetValueCard(Names.ATTACKING_CARD, index);
                    if (card1 != null)
                        defender.Hand.Add(card1);

                    PlayingCard card2 = server.GameState.GetValueCard(Names.DEFENDING_CARD, index);
                    if (card2 != null)
                        defender.Hand.Add(card2);

                    // Видалити обидві карти зі стану гри
                    server.GameState.Set<PlayingCard>(Names.ATTACKING_CARD, index, null);
                    server.GameState.Set<PlayingCard>(Names.DEFENDING_CARD, index, null);
                }

                // Встановити нового нападаючого гравця
                server.GameState.Set(Names.ATTACKING_PLAYER, nextAttackingPlayerId);

                // Відправити повідомлення
                server.SendServerMessage(defender.Name + " взяв карти");

                // Перехід до наступного поєдинку
                Utils.MoveNextDuel(server);
            }
        }
    }
}
