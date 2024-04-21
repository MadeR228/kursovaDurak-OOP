using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durak.Common
{
    // Зберігає корисні методи для різних цілей
    public static class Utils
    {
        // Заповнює список списком екземплярів усіх типів, які успадковують тип списку
        public static void FillTypeList<T>(AppDomain domain, List<T> result)
        {
            Type[] types = (
                from domainAssembly in domain.GetAssemblies()                   // Отримайте посилання на збірки
                from assemblyType in domainAssembly.GetTypes()                  // Отримати всі типи в зборі
                where typeof(T).IsAssignableFrom(assemblyType)                  // Перевірте, чи тип є правилом гри
                where assemblyType.GetConstructor(Type.EmptyTypes) != null      // Переконайтеся, що є порожній конструктор
                select assemblyType).ToArray();                                 // Перетворення IEnumerable на масив

            // Ітерації між ними
            for (int index = 0; index < types.Length; index++)
            {
                // Створіть екземпляр
                result.Add((T)Activator.CreateInstance(types[index]));
            }
        }

        // Отримує інформацію про те, чи можна перетворити об’єкт на заданий тип
        public static bool CanChangeType(object value, Type conversionType)
        {
            if (conversionType == null)
                return false;

            if (value == null)
                return false;
            if (!(value is IConvertible))
                return false;

            return true;
        }
    }
}
