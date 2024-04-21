using Lidgren.Network;
using System.Net;

namespace Durak.Common
{
    // Представляє дані тегів сервера
    public struct ServerTag
    {
        // Отримує кількість підключених гравців(ботів)
        private int myPlayerCount;
        // Зберігає кількість гравців, яку підтримує цей сервер
        private int mySupportedPlayerCount;
        // Зберігає стан сервера
        private ServerState myState;
        // IP-адреса сервера
        private IPEndPoint myAddress;


        // Отримує кількість гравців на цьому сервері
        public int PlayerCount
        {
            get { return myPlayerCount; }
            set { myPlayerCount = value; }
        }
        // Отримує або встановлює кількість гравців, яку підтримує цей сервер
        public int SupportedPlayerCount
        {
            get { return mySupportedPlayerCount; }
            set { mySupportedPlayerCount = value; }
        }
        // Отримує або встановлює стан цього сервера
        public ServerState State
        {
            get { return myState; }
            set { myState = value; }
        }
        // Отримує адресу сервера
        public IPEndPoint Address
        {
            get { return myAddress; }
        }

        // Записує цей серверний тег у вихідне повідомлення
        public void WriteToPacket(NetOutgoingMessage outMessage)
        {
            outMessage.Write(myPlayerCount);
            outMessage.Write(mySupportedPlayerCount);
            outMessage.Write((byte)myState);
            outMessage.WritePadBits();
        }

        // Зчитує тег сервера з вхідного повідомлення
        public static ServerTag ReadFromPacket(NetIncomingMessage inMessage)
        {
            ServerTag result = new ServerTag
            {
                myPlayerCount = inMessage.ReadInt32(),
                mySupportedPlayerCount = inMessage.ReadInt32(),
                myState = (ServerState)inMessage.ReadByte()
            };
            inMessage.ReadPadBits();
            result.myAddress = inMessage.SenderEndPoint;

            return result;
        }


        // Перевіряє, чи один тег сервера дорівнює іншому
        public static bool operator == (ServerTag left, ServerTag right)
        {
            return left.Address == right.Address;
        }

        // Перевіряє, чи один тег сервера не дорівнює іншому
        public static bool operator != (ServerTag left, ServerTag right)
        {
            return !(left.Address == right.Address);
        }
    }
}
