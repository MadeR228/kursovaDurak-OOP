using Lidgren.Network;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Durak.Common
{
    // Зберігає список усіх гравців у грі, використовується для пошуку
    // гравці за їхніми ID
    public sealed class PlayerCollection : IEnumerable<Player>
    {
        // Визначає кількість гравців за умовчанням для нових колекцій гравців
        public const int DEFAULT_PLAYER_COUNT = 2;

        // Внутрішній масив для цієї колекції
        private Player[] myPlayers;

        // Отримує або встановлює гравця з заданим ID
        public Player this[byte playerId]
        {
            get { return myPlayers[playerId]; }
            set { myPlayers[playerId] = value; }
        }

        // Отримує програвач із заданим мережевим підключенням
        public Player this[NetConnection connection]
        {
            get
            {
                // Повторює і порівнює з’єднання
                for (int index = 0; index < myPlayers.Length; index ++)
                    if (myPlayers[index] != null && myPlayers[index].Connection == connection)
                        return myPlayers[index];

                return null;
            }
        }

        // Отримує кількість слотів для гравців у цій колекції
        public int Count
        {
            get { return myPlayers.Length; }
        }

        // Отримує кількість ненульових гравців
        public byte PlayerCount
        {
            get { return (byte)myPlayers.Where(X => X != null).Count(); }
        }

        // Створює нову колекцію гравців із стандартною кількістю гравців
        public PlayerCollection() : this(DEFAULT_PLAYER_COUNT)
        { }

        // Створює нову колекцію гравців із заданою кількістю гравців
        public PlayerCollection(int numPlayers)
        {
            myPlayers = new Player[numPlayers];
        }

        // Змінює розмір цієї колекції програвачів до нового розміру
        public void Resize(int newSize)
        {
            if (newSize > myPlayers.Length)
                Array.Resize(ref myPlayers, newSize);
            else if (newSize < myPlayers.Length)
                throw new InvalidOperationException("Неможливо зменшити колекцію гравців");
        }

        // Отримує перший доступний ідентифікатор для цієї колекції гравців або -1, якщо немає відкритих слотів
        public int GetNextAvailableId()
        {
            // Отримує наступний доступний ідентифікатор
            for (int index = 0; index < myPlayers.Length; index++)
                if (myPlayers[index] == null)
                    return index;

            return -1;
        }

        // Видалити даного гравця з цієї колекції гравців
        public void Remove(Player player)
        {
            // Переконається,що гравець існує, а потім видаляє його
            if (player != null)
                for (int index = 0; index < myPlayers.Length; index++)
                    if (myPlayers[index] != null && myPlayers[index].Equals(player))
                    {
                        myPlayers[index] = null;
                        return;
                    }
        }

        // Очищає колекцію гравців
        internal void Clear()
        {
            for (int index = 0; index < myPlayers.Length; index++)
                myPlayers[index] = null;
        }

        // Отримує екземпляр ітератора для перерахування всіх гравців у цій колекції
        public IEnumerator<Player> GetEnumerator()
        {
            return myPlayers.Where(x => x != null).GetEnumerator();
        }

        // Отримує екземпляр ітератора для перерахування всіх гравців у цій колекції
        IEnumerator IEnumerable.GetEnumerator()
        {
            return myPlayers.Where(x => x != null).GetEnumerator();
        }
    }
}
