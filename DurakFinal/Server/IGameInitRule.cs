using Durak.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak.Server
{
    // Представляє правило для застосування під час ініціалізації ігри
    public interface IGameInitRule
    {
        // Отримує або встановлює, чи ввімкнено це правило
        bool IsEnabled { get; set; }

        // Отримує зрозумілу людині назву для цього правила гри. Використовується для налагодження
        // а також логів та параметрів
        string ReadableName { get; }

        // Отримує пріоритет для виконання, вищий пріоритет виконується першим
        int Priority { get; }

        // Перевіряє стан гри
        void InitState(GameServer server);
    }
}
