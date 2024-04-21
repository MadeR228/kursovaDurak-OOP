using Durak.Server;
using System.Linq;
using Durak.Common;
using Durak.Common.Cards;
using System.Collections.Generic;

namespace DurakGame.Rules
{
    // Представляє основне правило штучного інтелекту, що визначає найкращу карту для гри
    public class AIRules : IAIRule
    {
        // Метод, який визначає, які дії повинен виконати гравець-бот
        public void Propose(Dictionary<PlayingCard, float> proposals, GameServer server, CardCollection hand)
        {
            GameState state = server.GameState;

            // Отримати масть козирної карти
            CardSuit trumpSuit = state.GetValueCard(Names.TRUMP_CARD).Suit;

            // Визначити карти, з якими працюємо
            PlayingCard[] keys = proposals.Keys.ToArray();

            // Гравець-бот атакує
            if (state.GetValueBool(Names.IS_ATTACKING))
            {
                foreach (PlayingCard key in keys)
                {
                    // Автоматично встановлюємо вагу всіх карт на 1.0
                    proposals[key] += .50f;
                    if (key.Suit == trumpSuit)
                    {
                        proposals[key] -= .25f;
                    }
                    // Якщо це не перший раунд, робимо логіку, щоб використовувати лише картки, які можна грати на попередні карти
                    if (state.GetValueInt(Names.CURRENT_ROUND) != 0)
                    {
                        for (int i = 0; i >= state.GetValueInt(Names.CURRENT_ROUND); i++)
                        {
                            // Якщо карта не має того ж рангу, що й атакуюча або захищаюча карта, або не має тієї самої масті, що і козирна
                            if (state.GetValueCard(Names.ATTACKING_CARD, i).Rank != key.Rank || state.GetValueCard(Names.DEFENDING_CARD, i).Rank == key.Rank || key.Suit == trumpSuit)
                            {
                                proposals[key] = 0.0f;
                            }
                        }
                    }
                }
            }

            // Гравець-бот захищається
            else
            {
                PlayingCard attackingCard = state.GetValueCard(Names.ATTACKING_CARD, state.GetValueInt(Names.CURRENT_ROUND));

                foreach (PlayingCard key in keys)
                {
                    // Додаємо вагу до всіх карток з масті атакуючої карти та масті козирної
                    if (key.Suit == trumpSuit | key.Suit == attackingCard.Suit)
                    {
                        proposals[key] += .25f;
                        // Додаємо трохи більше ваги, якщо карта не є козирною. Таким чином, бот має менше шансів витратити свої козирні карти
                        if (key.Suit != trumpSuit)
                        {
                            proposals[key] += .25f;
                        }
                        // Додаємо вагу, якщо ранг карти вище, ніж ранг атакуючої карти
                        if (key.Rank > attackingCard.Rank)
                        {
                            proposals[key] += .25f - ((int)key.Rank / 100.0f);
                        }
                        // Якщо ранг картки менше, ніж ранг атакуючої карти, і масті співпадають, видаляємо вагу (означає, що козирні карти з меншим рангом все ще мають вагу)
                        else if (key.Rank < attackingCard.Rank && key.Suit == attackingCard.Suit)
                        {
                            proposals[key] = 0.0f;
                        }
                    }
                }
            }
        }
    }
}
