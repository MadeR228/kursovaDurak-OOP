using Durak.Common.Cards;
using Lidgren.Network;

namespace Durak.Common
{
    // Делегат для обробки, коли виявлено новий сервер
    public delegate void ServerDiscoveredEvent(object sender, ServerTag tag);

    // Делегат для обробки, коли гравець залишив гру
    public delegate void PlayerLeftEvent(object sender, Player player, string reason);

    // Делегувати для обробки, коли було зіграно недійсний хід
    public delegate void InvalidMoveEvent(object sender, PlayingCard card, string reason);

    // Делегувати для обробки, коли кількість карток гравця оновлено
    public delegate void PlayerCardCountChangedEvent(Player player, int newCardCount);

    // Делегат для обробки, коли параметр стану змінено
    public delegate void StateChangedEvent(GameState sender, StateParameter parameter);

    // Делегат для обробки, коли надіслано повідомлення чату
    public delegate void PlayerChatEvent(object sender, Player player, string message);

    // Представляє метод або подію, яка обробляє вхідні мережеві пакети
    public delegate void PacketHandler(NetIncomingMessage msg);
}
