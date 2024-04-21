using Durak.Common.Cards;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Durak.Common
{
    // Представляє аргументи події для події зміни стану
    public class StateParameter : EventArgs
    {
        // Отримує підтримувані типи
        public static readonly Dictionary<System.Type, Type> SUPPORTED_TYPES = new Dictionary<System.Type, Type>()
        {
            { typeof(byte), Type.Byte },
            { typeof(char), Type.Char },
            { typeof(short), Type.Short },
            { typeof(int), Type.Int },
            { typeof(bool), Type.Bool },
            { typeof(CardSuit), Type.CardSuit },
            { typeof(CardRank), Type.CardRank },
            { typeof(string), Type.String },
            { typeof(PlayingCard), Type.PlayingCard },
            { typeof(CardCollection), Type.CardCollection }
        };

        // Зберігає назву параметра
        private string myName;
        // Зберігає значення параметра
        private object myValue;
        // Зберігає тип параметра
        private Type myType;
        // Зберігає, чи цей параметр синхронізується
        private bool isSynced;

        // Отримує назву цього параметра
        public string Name
        {
            get { return myName; }
        }
        // Отримує необроблене значення цього параметра
        public object RawValue
        {
            get { return myValue; }
        }
        // Отримує або встановлює, чи цей параметр синхронізовано
        public bool IsSynced
        {
            get { return isSynced; }
            set { isSynced = value; }
        }
        // Отримує тип цього параметра стану
        public Type ParameterType
        {
            get { return myType; }
        }

        // Створює новий екземпляр параметра стану
        private StateParameter()
        {
            isSynced = false;
        }

        // Створює порожній параметр стану
        public static StateParameter CreateEmpty(bool sync = false)
        {
            return new StateParameter() { IsSynced = sync };
        }

        // Створює новий параметр стану заданого типу
        public static StateParameter Construct<T>(string name, T value, bool syncronize)
        {
            StateParameter result = new StateParameter
            {
                myName = name,
                myType = SUPPORTED_TYPES[typeof(T)],
                myValue = value,
                IsSynced = syncronize
            };

            return result;
        }

        // Внутрішній метод отримання значення як зазначеного типу
        internal T GetValueInternal<T>()
        {
            if (myValue == null)
                return default;
            else if (myValue is T t)
                return t;
            else
                throw new InvalidCastException(string.Format("Cannot cast {0} to {1}", myValue.GetType().Name, typeof(T).Name));
        }

        // Отримати значення цього параметра як байт
        public byte GetValueByte()
        {
            return GetValueInternal<byte>();
        }
        // Отримати значення цього параметра як символ
        public int GetValueChar()
        {
            return GetValueInternal<char>();
        }
        // Отримати значення цього параметра як 16-розрядне ціле число зі знаком
        public int GetValueShort()
        {
            return GetValueInternal<short>();
        }
        // Отримати значення цього параметра як 32-розрядне ціле число зі знаком
        public int GetValueInt()
        {
            return GetValueInternal<int>();
        }
        // Отримайте значення цього параметра boolean
        public bool GetValueBool()
        {
            return GetValueInternal<bool>();
        }
        // Отримати значення цього параметра як масть карти
        public CardSuit GetValueCardSuit()
        {
            return GetValueInternal<CardSuit>();
        }
        // Отримати значення цього параметра як ранг карти
        public CardRank GetValueCardRank()
        {
            return GetValueInternal<CardRank>();
        }
        // Отримати значення цього параметра у вигляді рядка
        public string GetValueString()
        {
            return GetValueInternal<string>();
        }
        // Отримати значення цього параметра як ігрову карту
        public PlayingCard GetValuePlayingCard()
        {
            return GetValueInternal<PlayingCard>();
        }
        public CardCollection GetValueCardCollection()
        {
            return GetValueInternal<CardCollection>();
        }

        // Внутрішній метод, який використовується для встановлення значення
        internal void SetValueInternal<T>(T value)
        {
            System.Type t = typeof(T);
            System.Type valueType = SUPPORTED_TYPES.FirstOrDefault(X => X.Value == myType).Key;


            if (SUPPORTED_TYPES.ContainsKey(t))
            {
                if (t == valueType)
                    myValue = value;
                else if (Utils.CanChangeType(value, valueType))
                    myValue = Convert.ChangeType(value, valueType);
            }
            else
                throw new InvalidCastException("Type " + t + " is not supported");
        }

        // Встановлює для цього параметра байт
        public void SetValue(byte value)
        {
            SetValueInternal(value);
        }
        // Встановлює для цього параметра символ
        public void SetValue(char value)
        {
            SetValueInternal(value);
        }
        // Встановлює для цього параметра 16-бітове ціле число зі знаком
        public void SetValue(short value)
        {
            SetValueInternal(value);
        }
        // Встановлює для цього параметра 32-розрядне ціле число зі знаком
        public void SetValue(int value)
        {
            SetValueInternal(value);
        }
        // Установіть для цього параметра boolean
        public void SetValue(bool value)
        {
            SetValueInternal(value);
        }
        // Встановлює для цього параметра масть карти
        public void SetValue(CardSuit value)
        {
            SetValueInternal(value);
        }
        // Встановлює для цього параметра ранг карти
        public void SetValue(CardRank value)
        {
            SetValueInternal(value);
        }
        // Встановлює для цього параметра рядок
        public void SetValue(string value)
        {
            SetValueInternal(value);
        }
        // Встановлює цей параметр для гральної карти
        public void SetValue(PlayingCard value)
        {
            value.FaceUp = true;
            SetValueInternal(value);
        }
        // Встановлює для цього параметра колекцію карт
        public void SetValue(CardCollection value)
        {
            SetValueInternal(value);
        }

        // Кодує цей параметр стану в мережеве повідомлення
        public void Encode(NetOutgoingMessage msg)
        {
            if (isSynced)
            {
                msg.Write(Name);
                msg.Write((byte)myType);

                switch (myType)
                {
                    case Type.Byte:
                        msg.Write((byte)myValue);
                        break;
                    case Type.Char:
                        msg.Write((char)myValue);
                        break;
                    case Type.Short:
                        msg.Write((short)myValue);
                        break;
                    case Type.Int:
                        msg.Write((int)myValue);
                        break;
                    case Type.Bool:
                        msg.Write((bool)myValue);
                        break;
                    case Type.CardSuit:
                        msg.Write((byte)((CardSuit)myValue));
                        break;
                    case Type.CardRank:
                        msg.Write((byte)((CardSuit)myValue));
                        break;
                    case Type.String:
                        msg.Write((string)myValue);
                        break;
                    case Type.PlayingCard:
                        msg.Write(myValue != null);

                        if (myValue != null)
                        {
                            msg.Write((byte)(myValue as PlayingCard).Rank);
                            msg.Write((byte)(myValue as PlayingCard).Suit);
                        }
                        break;
                    case Type.CardCollection:
                        msg.Write((myValue as CardCollection).Count);

                        foreach (PlayingCard card in (myValue as CardCollection))
                        {
                            msg.Write(card != null);

                            if (card != null)
                            {
                                msg.Write((byte)card.Rank);
                                msg.Write((byte)card.Suit);
                            }
                        }
                        break;
                }

                msg.WritePadBits();
            }
        }

        // Декодує значення з повідомлення
        private void DecodeInternal(NetIncomingMessage msg)
        {
            // Зчитує значення 
            switch (myType)
            {
                case Type.Byte:
                    myValue = msg.ReadByte();
                    break;
                case Type.Char:
                    myValue = (char)msg.ReadByte();
                    break;
                case Type.Short:
                    myValue = msg.ReadInt16();
                    break;
                case Type.Int:
                    myValue = msg.ReadInt32();
                    break;
                case Type.Bool:
                    myValue = msg.ReadBoolean();
                    break;
                case Type.CardSuit:
                    myValue = (CardSuit)msg.ReadByte();
                    break;
                case Type.CardRank:
                    myValue = (CardRank)msg.ReadByte();
                    break;
                case Type.String:
                    myValue = msg.ReadString();
                    break;
                case Type.PlayingCard:
                    if (msg.ReadBoolean())
                    {
                        myValue = new PlayingCard((CardRank)msg.ReadByte(), (CardSuit)msg.ReadByte()) { FaceUp = true };
                    }
                    else
                    {
                        myValue = null;
                    }
                    break;
                case Type.CardCollection:
                    CardCollection resultCollection = new CardCollection();

                    int numCards = msg.ReadInt32();

                    for (int index = 0; index < numCards; index++)
                    {
                        bool hasValue = msg.ReadBoolean();

                        if (hasValue)
                        {
                            resultCollection.Add(new PlayingCard((CardRank)msg.ReadByte(), (CardSuit)msg.ReadByte()) { FaceUp = true });
                        }
                    }

                    myValue = resultCollection;
                    break;

            }

            msg.ReadPadBits();
        }

        // Декодує параметр стану з повідомлення
        public void Decode(NetIncomingMessage msg)
        {
            // Read the name and type
            myName = msg.ReadString();
            myType = (Type)msg.ReadByte();
            
            // Decode the value
            DecodeInternal(msg);
        }

        // Декодує параметр стану з повідомлення, використовуючи стан гри для оновлення
        public static StateParameter Decode(NetIncomingMessage msg, GameState state)
        {
            string name = msg.ReadString();
            Type type = (Type)msg.ReadByte();

            StateParameter result = null;

            switch (type)
            {
                case Type.Byte:
                    result = state.GetParameter<byte>(name);
                    break;
                case Type.Char:
                    result = state.GetParameter<char>(name);
                    break;
                case Type.Short:
                    result = state.GetParameter<short>(name);
                    break;
                case Type.Int:
                    result = state.GetParameter<int>(name);
                    break;
                case Type.Bool:
                    result = state.GetParameter<bool>(name);
                    break;
                case Type.CardSuit:
                    result = state.GetParameter<CardSuit>(name);
                    break;
                case Type.CardRank:
                    result = state.GetParameter<CardRank>(name);
                    break;
                case Type.String:
                    result = state.GetParameter<String>(name);
                    break;
                case Type.PlayingCard:
                    result = state.GetParameter<PlayingCard>(name);
                    break;
                case Type.CardCollection:
                    result = state.GetParameter<CardCollection>(name);
                    break;
            }
            // Розшифровує значення
            result.DecodeInternal(msg);

            state.InvokeUpdated(result);
            
            return result;
        }

        // Отримує основні значення ToString
        public override string ToString()
        {
            return myValue?.ToString();
        }

        // Представляє тип, який може охоплювати параметр
        public enum Type
        {
            // Це параметр стану System.Byte
            Byte,
            // Це параметр стану System.Char
            Char,
            // Це параметр стану System.Int16
            Short,
            // Це параметр стану System.Int32
            Int,
            // Це параметр стану System.Boolean
            Bool,
            // Це параметр стану DurakCommom.Cards.CardSuit
            CardSuit,
            // Це параметр стану DurakCommom.Cards.CardRank
            CardRank,
            // Це параметр стану System.String
            String,
            // Це параметр стану DurakCommom.Cards.PlayingCard
            PlayingCard,
            // Це параметр стану DurakCommom.Cards.CardCollection
            CardCollection
        }
    }
}