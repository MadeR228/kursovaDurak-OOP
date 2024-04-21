using System;
using Durak.Common;
using Durak.Common.Cards;
using System.Collections.Generic;

namespace Durak.Server
{
    // Представляє правило, яке бот використовує для визначення карти для розігрування
    public interface IAIRule
    {
        // Визначає запропонований хід із заданого стану та поточної руки
        void Propose(Dictionary<PlayingCard, float> proposalTable, GameServer server, CardCollection hand);
    }
}