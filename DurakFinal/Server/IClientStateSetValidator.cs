using Durak.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak.Server
{
    // Представляє інтерфейс, який використовує сервер, щоб визначити, чи може клієнт установити параметр стану
    public interface IClientStateSetValidator
    {
        // Намагається встановити запитаний параметр стану
    }
}
