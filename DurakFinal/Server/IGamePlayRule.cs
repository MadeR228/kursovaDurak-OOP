using Durak.Common;

namespace Durak.Server
{
    // Представляє інтерфейс для правил гри, яке використовується логікою гри
    public interface IGamePlayRule
    {
        // Отримує або встановлює, чи ввімкнено це правило
        bool IsEnabled { get; set; }

        // Отримує зрозумілу людині назву для цього правила гри. Використовується для налагодження
        // а також логів та параметрів
        string ReadableName { get; }

        // Перевіряє запропонований ігровий хід на поточний стан гри та повертає, чи він дійсний
        bool IsValidMove(GameServer server, GameMove move, ref string reason);
    }
}
