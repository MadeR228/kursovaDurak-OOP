using Durak.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak.Server
{
    // Правило для застосування до станів гри
    public interface IGameStateRule
    {
        // Отримує або встановлює, чи ввімкнено це правило
        bool IsEnabled { get; set; }

        // Отримує зрозумілу людині назву для цього правила гри. Використовується для налагодження
        // а також логів та параметрів
        string ReadableName { get; }

        // Перевіряє стан гри
        void ValidateState(GameServer server);
    }
}
