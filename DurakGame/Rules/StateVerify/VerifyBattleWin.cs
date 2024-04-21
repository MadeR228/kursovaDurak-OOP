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
    // Клас, який обробляє перевірку, чи завершився бій (тобто раунд = 6), і відповідно оновлює стан гри
    public class VerifyBattleWin : IGameStateRule
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
                return "Перевірка виграного бою";
            }
        }

        // Обробляє перевірку стану сервера. Якщо раунд = 6, це обробляє перехід до наступного раунду та присудження перемоги захиснику
        public void ValidateState(GameServer server)
        {
            // Перевірити виграну битву
            if (server.GameState.GetValueInt(Names.CURRENT_ROUND) == 6)
            {
                // Отримати поточний раунд та колоду відкиданих карт зі стану
                int round = server.GameState.GetValueInt(Names.CURRENT_ROUND);
                CardCollection discard = server.GameState.GetValueCardCollection(Names.DISCARD);

                // Пройтися по всім попереднім раундам, оскільки в цьому раунді немає атакуючих або захисних карт
                for (int index = 0; index < round; index++)
                {
                    // Додати карти до колоди відкиданих карт
                    discard.Add(server.GameState.GetValueCard(Names.ATTACKING_CARD, index));
                    discard.Add(server.GameState.GetValueCard(Names.DEFENDING_CARD, index));

                    // Видалити карти зі стану
                    server.GameState.Set<PlayingCard>(Names.ATTACKING_CARD, index, null);
                    server.GameState.Set<PlayingCard>(Names.DEFENDING_CARD, index, null);
                }

                // Оновити колоду відкиданих карт
                server.GameState.Set(Names.DISCARD, discard);

                // Перейти до наступного поєдинку
                Utils.MoveNextDuel(server);
            }
        }
    }
}
