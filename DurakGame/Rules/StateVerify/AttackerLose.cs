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
    // Клас, який обробляє перевірку того, чи нападаючий програв, і оновлює стан гри відповідно
    public class AttackerLose : IGameStateRule
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
                return "Перевірка, чи нападаючий гравець взяв карти";
            }
        }

        // Обробляє перевірку стану сервера. Якщо нападаючий програв, це відповідає за перехід до наступного раунду
        public void ValidateState(GameServer server)
        {
            // Виконувати це оновлення стану, лише якщо нападаючий відмовився
            if (server.GameState.GetValueBool(Names.ATTACKER_FORFEIT))
            {
                // Отримати поточний раунд та колоди відкидних карт із стану
                int round = server.GameState.GetValueInt(Names.CURRENT_ROUND);
                CardCollection discard = server.GameState.GetValueCardCollection(Names.DISCARD);

                // Пройтися по всіх попередніх раундах, оскільки у цьому раунді немає бойових карт
                for (int index = 0; index < round; index++)
                {
                    // Додати карти до відкидного столу
                    discard.Add(server.GameState.GetValueCard(Names.ATTACKING_CARD, index));
                    discard.Add(server.GameState.GetValueCard(Names.DEFENDING_CARD, index));

                    // Видалити карти зі стану гри
                    server.GameState.Set<PlayingCard>(Names.ATTACKING_CARD, index, null);
                    server.GameState.Set<PlayingCard>(Names.DEFENDING_CARD, index, null);
                }

                // Надіслати повідомлення
                server.SendServerMessage(server.Players[server.GameState.GetValueByte(Names.ATTACKING_PLAYER)].Name + " бито");

                // Оновити стіл
                server.GameState.Set(Names.DISCARD, discard);

                // Перехід до наступного поєдинку
                Utils.MoveNextDuel(server);
            }
        }
    }
}
