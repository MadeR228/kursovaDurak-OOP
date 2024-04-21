using System;
using Durak.Common.Cards;
using Lidgren.Network;

namespace Durak.Common
{
    // Представляє одного гравця в грі. Зберігає всі дані про гравця
    public class Player
    {
        // Отримує ідентифікатор цього гравця
        public byte PlayerId { get; private set; }
        // Отримує з'єднання цього гравця з сервером
        public NetConnection Connection { get; private set; }
        // Отримує ім'я гравця
        public string Name { get; private set; }
        // Отримує поточну руку гравця
        public CardCollection Hand { get; private set; }
        // Отримує або задає кількість карт у руці гравця.
        public int NumCards { get; set; }
        // Отримує або задає, чи цей гравець є ботом
        public bool IsBot { get; set; }
        // Отримує або задає, чи цей екземпляр гравця є гравцем-господарем
        public bool IsHost { get; set; }

        // Викликається, коли до руки гравця було додано карту
        public event EventHandler<PlayingCard> OnCardAddedToHand;
        // Викликається, коли з руки гравця було вилучено карту
        public event EventHandler<PlayingCard> OnCardRemovedFromHand;

        // Створює новий екземпляр гравця
        public Player(byte playerId, string name, bool isBot)
        {
            PlayerId = playerId;
            Name = name;
            IsBot = isBot;
            Hand = new CardCollection();

            Hand.OnCardAdded += CardAdded;
            Hand.OnCardRemoved += CardRemoved;
        }

        // Створює новий екземпляр гравця на клієнтському боці
        public Player(ClientTag tag, byte playerId) : this(playerId, tag.Name, false)
        {
        }

        // Створює новий екземпляр гравця на клієнтському боці
        public Player(ClientTag tag, NetConnection connection, byte playerId)
            : this(tag, playerId)
        {
            Connection = connection;
        }

        // Викликається, коли з руки гравця було видалено карту
        private void CardRemoved(object sender, CardEventArgs e)
        {
            OnCardRemovedFromHand?.Invoke(this, e.Card);
        }

        // Викликається, коли до руки гравця було додано карту
        private void CardAdded(object sender, CardEventArgs e)
        {
            OnCardAddedToHand?.Invoke(this, e.Card);
        }

        // Перевизначає перевірку рівності для цього об'єкта
        public override bool Equals(object obj)
        {
            return (obj is Player && (obj as Player).PlayerId == PlayerId);
        }

        // Перевизначає хеш-код для цього об'єкта, повертаючи напівунікальне значення
        public override int GetHashCode()
        {
            return PlayerId;
        }

        // Кодує цей екземпляр в мережеве повідомлення
        public void Encode(NetOutgoingMessage msg)
        {
            msg.Write(PlayerId);
            msg.Write(Name);
            msg.Write(Hand.Count);
            msg.Write(IsBot);
            msg.Write(IsHost);
            msg.WritePadBits();
        }

        // Декодує цей екземпляр з мережевого повідомлення
        public void Decode(NetIncomingMessage msg)
        {
            PlayerId = msg.ReadByte();
            Name = msg.ReadString();
            NumCards = msg.ReadInt32();
            IsBot = msg.ReadBoolean();
            IsHost = msg.ReadBoolean();
            msg.ReadPadBits();
        }

        // Створює нову копію цього екземпляра гравця
        public Player Clone()
        {
            Player result = new Player(PlayerId, Name, true);
            return result;
        }
    }
}
