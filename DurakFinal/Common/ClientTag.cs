using Lidgren.Network;
using System.Net;
using System;

namespace Durak.Common
{
    // Представляє дані тегу клієнта, такі як ім’я гравця, IP-адреса, тип тощо...
    public struct ClientTag
    {
        // Зберігає ім'я гравця клієнта
        private string myName;
        // Зберігає IP-адресу клієнта
        private IPEndPoint myAddress;
        // Отримує ім'я цього клієнта
        public string Name
        {
            get { return myName; }
        }
        // Отримати кінцеву IP-точку
        public IPEndPoint Address
        {
            get { return myAddress; }
        }

        // Створює новий клієнтський тег із заданим іменем гравця
        public ClientTag(string name)
        {
            myName = name;
            myAddress = null;
        }

        // Записує цей тег клієнта у вихідне повідомлення
        public void WriteToPacket(NetOutgoingMessage outMessage)
        {
            outMessage.Write(myName);
        }

        // Зчитує тег клієнта з вхідного повідомлення та встановлює його адресу на адресу відправника повідомлення
        public static ClientTag ReadFromPacket(NetIncomingMessage inMessage)
        {
            ClientTag result;
            
            result.myName = inMessage.ReadString();
            result.myAddress = inMessage.SenderEndPoint;

            return result;
        }
    }
}
