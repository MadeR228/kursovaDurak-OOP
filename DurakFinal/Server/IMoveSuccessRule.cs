using Durak.Common;

namespace Durak.Server
{
    // Правило, яке має бути виконано після вдалого ходу
    public interface IMoveSuccessRule
    {
        // Оновлює стан щодо ходу
        void UpdateState(GameServer server, GameMove move);
    }
}
