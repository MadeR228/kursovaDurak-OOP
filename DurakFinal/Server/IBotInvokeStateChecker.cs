using Durak.Common;

namespace Durak.Server
{
    // Представляє інтерфейс, який використовується для перевірки того, чи слід викликати бота в кінці поточного повідомлення
    public interface IBotInvokeStateChecker
    {
        bool ShouldInvoke(GameServer server, BotPlayer player);
    }
}
