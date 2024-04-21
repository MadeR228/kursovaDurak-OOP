using System;

namespace Durak.Common
{
    // Представляє стан ігрового сервера
    public enum ServerState
    {
        // Сервер знаходиться у лобі та приймає підключення
        InLobby,
        // Сервер у грі
        InGame,
        // Сервер не працює
        NotRunning
    }
}
