using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurakGame
{
    // Клас, що містить імена станів у вигляді констант. Це дозволяє нам завжди використовувати одне й те саме ім'я
    // для посилання на важливі стани.
    public static class Names
    {
        // Назва колекції карт, що захищаються
        // тип PlayingCard
        public const string DEFENDING_CARD = "defending_card";
        // Назва колекції карт, що атакуються
        // тип PlayingCard
        public const string ATTACKING_CARD = "attacking_card";
        // Назва лічильника поточного раунду
        // тип byte
        public const string CURRENT_ROUND = "current_round";
        // Назва ID атакуючого гравця
        // тип byte
        public const string ATTACKING_PLAYER = "attacking_player_id";
        // Назва ID захищаючого гравця
        // тип byte
        public const string DEFENDING_PLAYER = "defending_player_id";
        // Назва параметру "Чи атакує"
        // тип boolean
        public const string IS_ATTACKING = "is_attacking";
        // Назва поля, що вказує на здатність атакувальника здатність відмовитися від ходу
        // тип boolean
        public const string ATTACKER_FORFEIT = "attacker_forfeit";
        // Назва поля, що вказує на здатність захисника здатність відмовитися від ходу
        // тип boolean
        public const string DEFENDER_FORFEIT = "defender_forfeit";
        // Назва колоди відкинутих карт
        // тип CardCollection
        public const string DISCARD = "discard_pile";
        // Назва козирної карти
        // тип PlayingCard
        public const string TRUMP_CARD = "trump_card";
        // Назва поля кількості карт в колоді
        // тип integer
        public const string DECK_COUNT = "cards_in_deck";
        // Назва поля вихідної колоди
        // тип CardCollection
        public const string DECK = "source_deck";
        // Назва поля закінчення гри
        // тип boolean
        public const string GAME_OVER = "game_over";
        // Назва ID гравця-програвшого
        // типу byte
        public const string LOSER_ID = "loser_id";
        // Назва поля нічиєї гри
        // тип Boolean
        public const string IS_TIE = "is_tie";
        // Назва поля, що вказує на використання козирної карти
        // Це типу Boolean
        public const string TRUMP_CARD_USED = "trump_card_used";
        // Назва поля початкової кількості карт
        // тип int
        public const string NUM_INIT_CARDS = "number_init_cards";
    }
}
