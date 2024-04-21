using Durak.Common.Cards;
using Lidgren.Network;
using System.Linq;
using System;

namespace Durak.Common
{
    // Представляє один рух, зроблений гравцем
    public struct GameMove
    {
        // Зберігає гравця, який зробив рух
        private Player myPlayer;
        // Зберігає карту для гри
        private PlayingCard myMove;

        // Створює новий рух гри з вказаним гравцем
        public GameMove(Player player, PlayingCard move) : this()
        {
            myPlayer = player;
            myMove = move;
        }

        // Отримує гравця, який виконав цей рух
        public Player Player
        {
            get { return myPlayer; }
        }
        // Отримує карту, яку гравець хоче зіграти
        public PlayingCard Move
        {
            get { return myMove; }
        }

        // Записує цей рух гри до заданого мережного пакету
        public void Encode(NetOutgoingMessage outMessage)
        {
            // Просто передати цей ідентифікатор
            outMessage.Write(myPlayer.PlayerId);

            outMessage.Write(myMove != null);
            outMessage.WritePadBits();

            // Записати інформацію про карту
            if (myMove != null)
            {
                outMessage.Write((byte)myMove.Rank);
                outMessage.Write((byte)myMove.Suit);
            }
        }

        // Зчитує рух гри з заданого мережного пакету, вважаючи, що він від клієнта
        public static GameMove DecodeFromClient(NetIncomingMessage inMessage, PlayerCollection players)
        {
            GameMove result = new GameMove();

            // Побудувати результат
            result.myPlayer = players[inMessage.ReadByte()];

            bool hasValue = inMessage.ReadBoolean();
            inMessage.ReadPadBits();

            // Прочитати, якщо не є нульовим
            if (hasValue)
            {
                // Отримати ігральну карту
                int moveRank = inMessage.ReadByte();
                int moveSuit = inMessage.ReadByte();

                result.myMove = new PlayingCard((CardRank)moveRank, (CardSuit)moveSuit);
                result.myMove.FaceUp = true;
            }

            return result;
        }

        // Зчитує рух гри з заданого мережного пакету
        public static GameMove Decode(NetIncomingMessage inMessage, PlayerCollection players)
        {
            GameMove result = new GameMove();

            // Отримати ідентифікатор гравця
            byte playerId = inMessage.ReadByte();

            // Побудувати результат
            result.myPlayer = players[playerId];

            bool hasValue = inMessage.ReadBoolean();
            inMessage.ReadPadBits();

            // Прочитати, якщо не є нульовим
            if (hasValue)
            {
                // Отримати ігральну карту
                int moveRank = inMessage.ReadByte();
                int moveSuit = inMessage.ReadByte();

                result.myMove = new PlayingCard((CardRank)moveRank, (CardSuit)moveSuit);
                result.myMove.FaceUp = true;
            }

            return result;
        }
    }
}
