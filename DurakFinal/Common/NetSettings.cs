using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak.Common
{
    // Зберігає порт і назву програми
    public static class NetSettings
    {
        // Отримує стандартний порт сервера
        public const int DEFAULT_SERVER_PORT = 55440;
        // Отримує час очікування сервера за замовчуванням
        public const float DEFAULT_SERVER_TIMEOUT = 30.0f;
        // Отримує ідентифікатор програми
        public const string APP_IDENTIFIER = "ДУРЕНЬ";
        // Отримує повідомлення про завершення роботи серверів
        public const string DEFAULT_SERVER_SHUTDOWN_MESSAGE = "";
    }
}
