using Durak.Common.Cards;
using System;
using System.Collections.Generic;
using Lidgren.Network;
using System.Linq;

namespace Durak.Common
{
    // Представляє стан гри 
    public class GameState
    {
        // Зберігає формат рядка для іменування елементів масиву
        private const string ARRAY_FORMAT = "@{0}[{1}]";

        // Зберігає параметри
        private Dictionary<string, StateParameter> myParameters;

        // Зберігає колекцію подій зі зміною стану
        private Dictionary<string, StateChangedEvent> myChangedEvents;

        // Зберігає колекцію подій станів
        private Dictionary<Tuple<string, object>, StateChangedEvent> myStateEqualsEvents;

        // Викликається, коли змінюється один стан у цьому стані гри
        public event EventHandler<StateParameter> OnStateChanged;
        // Викликається, коли змінюється один стан у цьому стані гри, ця подія не вимикається та використовується
        // переважно для цілей інтерфейсу користувача
        public event EventHandler<StateParameter> OnStateChangedUnSilenceable;
        // Викликається, коли стан очищено
        public event EventHandler OnCleared;

        // Отримує або встановлює, чи стан не повинен викликати події, коли встановлено параметри.
        // Корисно для ініціалізації
        public bool SilentSets
        {
            get;
            set;
        }

        // Створює новий екземпляр стану гри
        public GameState()
        {
            myParameters = new Dictionary<string, StateParameter>();
            myChangedEvents = new Dictionary<string, StateChangedEvent>();
            myStateEqualsEvents = new Dictionary<Tuple<string, object>, StateChangedEvent>();
        }

        // Очищає цей стан гри для повторного використання
        public void Clear()
        {
            myParameters.Clear();

            if (OnCleared != null)
                OnCleared.Invoke(this, EventArgs.Empty);
        }

        // Додає прослуховувач зміни стану до вказаного параметра стану
        public void AddStateChangedEvent(string name, StateChangedEvent eventListener)
        {
            if (!myChangedEvents.ContainsKey(name))
                myChangedEvents.Add(name, eventListener);
            else
                myChangedEvents[name] += eventListener;
        }

        // Видалено прослуховувач зміни стану для заданого параметра стану
        public void RemoveStateChangedEvent(string name, StateChangedEvent eventListener)
        {
            if (!myChangedEvents.ContainsKey(name))
                myChangedEvents.Add(name, eventListener);
            else
                myChangedEvents[name] -= eventListener;
        }

        // До заданого параметра стану додається стан, який дорівнює слухачу
        public void AddStateEqualsEvent(string name, object value, StateChangedEvent eventListener)
        {
            Tuple<string, object> key = new Tuple<string, object>(name, value);

            if (!myStateEqualsEvents.ContainsKey(key))
                myStateEqualsEvents.Add(key, eventListener);
            else
                myStateEqualsEvents[key] += eventListener;
        }

        // Видалений стан дорівнює слухачу заданого параметра стану
        public void RemoveStateEqualsEvent(string name, object value, StateChangedEvent eventListener)
        {
            Tuple<string, object> key = new Tuple<string, object>(name, value);

            if (!myStateEqualsEvents.ContainsKey(key))
                myStateEqualsEvents.Add(key, eventListener);
            else
                myStateEqualsEvents[key] -= eventListener;
        }

        // Додає прослуховувач зміни стану до вказаного параметра стану
        public void AddStateChangedEvent(string name, int index, StateChangedEvent eventListener)
        {
            name = string.Format(ARRAY_FORMAT, name, index);

            if (!myChangedEvents.ContainsKey(name))
                myChangedEvents.Add(name, eventListener);
            else
                myChangedEvents[name] += eventListener;
        }

        // Видаляє прослуховувач зміни стану для заданого параметра стану
        public void RemoveStateChangedEvent(string name, int index, StateChangedEvent eventListener)
        {
            name = string.Format(ARRAY_FORMAT, name, index);

            if (!myChangedEvents.ContainsKey(name))
                myChangedEvents.Add(name, eventListener);
            else
                myChangedEvents[name] -= eventListener;
        }

        // До заданого параметра стану додається стан, який дорівнює слухачу
        public void AddStateEqualsEvent(string name, int index, object value, StateChangedEvent eventListener)
        {
            name = string.Format(ARRAY_FORMAT, name, index);

            Tuple<string, object> key = new Tuple<string, object>(name, value);

            if (!myStateEqualsEvents.ContainsKey(key))
                myStateEqualsEvents.Add(key, eventListener);
            else
                myStateEqualsEvents[key] += eventListener;
        }

        // Видаляє стан дорівнює слухачу заданого параметра стану
        public void RemoveStateEqualsEvent(string name, int index, object value, StateChangedEvent eventListener)
        {
            name = string.Format(ARRAY_FORMAT, name, index);

            Tuple<string, object> key = new Tuple<string, object>(name, value);

            if (!myStateEqualsEvents.ContainsKey(key))
                myStateEqualsEvents.Add(key, eventListener);
            else
                myStateEqualsEvents[key] -= eventListener;
        }

        // Отримує параметр стану з заданим іменем
        public StateParameter GetParameter<T>(string name, bool serverSide = false)
        {
            if (!myParameters.ContainsKey(name))
                myParameters.Add(name, StateParameter.Construct<T>(name, (T)Activator.CreateInstance(typeof(T)), serverSide));

            // Get the parameter
            return myParameters[name];
        }

        // Приватний метод, який використовується всіма наборами, які обробляють налаштування параметра
        private void InternalSet<T>(string name, T value, bool serverSide)
        {
            if (!myParameters.ContainsKey(name))
                myParameters.Add(name, StateParameter.Construct(name, value, !serverSide));
            else
            {
                myParameters[name].SetValueInternal(value);
            }

            InvokeUpdated(myParameters[name]);
        }

        // Оновлює параметр стану в цьому стані гри
        public void UpdateParam(StateParameter parameter)
        {
            if (myParameters.ContainsKey(parameter.Name))
                InternalSet(parameter.Name, parameter.RawValue, !parameter.IsSynced);
            else
                myParameters.Add(parameter.Name, parameter);
        }

        // Встановлює заданому параметру значення
        public void Set<T>(string name, T value, bool serverSide = false)
        {
            if (string.IsNullOrWhiteSpace(name) || name[0] == '@')
                throw new ArgumentException("Invalid name, cannot be empty or start with @");

            if (!StateParameter.SUPPORTED_TYPES.ContainsKey(typeof(T)))
                throw new ArgumentException("Type " + typeof(T) + " is not a supported type");

            InternalSet(name, value, serverSide);
        }

        // Встановлює для заданого слота масиву параметрів значення
        public void Set<T>(string name, int index, T value, bool serverSide = false)
        {
            if (!StateParameter.SUPPORTED_TYPES.ContainsKey(typeof(T)))
                throw new ArgumentException("Type " + typeof(T) + " is not a supported type");

            InternalSet(string.Format(ARRAY_FORMAT, name, index), value, serverSide);
        }

        // Приватний метод, який використовується всіма getами, які обробляють отримання параметра.
        // Зауважте, що якщо параметр не визначено, повертається значення за замовчуванням для цього типу
        private T GetValueInternal<T>(string name, bool serverSide = false)
        {
            if (myParameters.ContainsKey(name))
            {
                object value = myParameters[name].RawValue;

                if (value == null)
                    return default(T);
                else if (typeof(T).IsAssignableFrom(value.GetType()))
                    return (T)value;
                else if (Utils.CanChangeType(value, typeof(T)))
                    return (T)Convert.ChangeType(value, typeof(T));
                else
                    throw new InvalidCastException(string.Format("Cannot cast {0} to {1}", value.GetType().Name, typeof(T).Name));
            }
            else
            {
                myParameters.Add(name, StateParameter.Construct(name, default(T), !serverSide));
                return myParameters[name].GetValueInternal<T>();
            }
        }

        // Отримує параметр із заданим іменем як байт
        public byte GetValueByte(string name)
        {
            return GetValueInternal<byte>(name);
        }
        // Отримує параметр у масиві з заданим іменем у вигляді байта
        public byte GetValueByte(string name, int index)
        {
            return GetValueInternal<byte>(string.Format(ARRAY_FORMAT, name, index));
        }
        // Отримує параметр із заданим іменем як символ
        public char GetValueChar(string name)
        {
            return GetValueInternal<char>(name);
        }
        // Отримує параметр у масиві з заданим іменем у вигляді символу
        public char GetValueChar(string name, int index)
        {
            return GetValueInternal<char>(string.Format(ARRAY_FORMAT, name, index));
        }
        // Отримує параметр із вказаною назвою як 16-бітне ціле число
        public short GetValueShort(string name)
        {
            return GetValueInternal<short>(name);
        }
        // Отримує параметр у масиві з заданим іменем у вигляді 16-бітного цілого числа
        public short GetValueShort(string name, int index)
        {
            return GetValueInternal<short>(string.Format(ARRAY_FORMAT, name, index));
        }
        // Отримує параметр із заданим іменем як 32-розрядне ціле число
        public int GetValueInt(string name)
        {
            return GetValueInternal<int>(name);
        }
        // Отримує параметр у масиві з заданим іменем у вигляді 32-розрядного цілого числа
        public int GetValueInt(string name, int index)
        {
            return GetValueInternal<int>(string.Format(ARRAY_FORMAT, name, index));
        }
        // Отримує параметр із вказаною назвою як boolean
        public bool GetValueBool(string name)
        {
            return GetValueInternal<bool>(name);
        }
        // Отримує параметр у масиві з заданим іменем як boolean
        public bool GetValueBool(string name, int index)
        {
            return GetValueInternal<bool>(string.Format(ARRAY_FORMAT, name, index));
        }
        // Отримує параметр із вказаною назвою як ранг картки
        public CardRank GetValueCardRank(string name)
        {
            return GetValueInternal<CardRank>(name);
        }
        // Отримує параметр у масиві з заданим іменем як ранг карти
        public CardRank GetValueCardRank(string name, int index)
        {
            return GetValueInternal<CardRank>(string.Format(ARRAY_FORMAT, name, index));
        }
        // Отримує параметр із вказаною назвою як масть карти
        public CardSuit GetValueCardSuit(string name)
        {
            return GetValueInternal<CardSuit>(name);
        }
        // Отримує параметр у масиві з вказаною назвою як масть карти
        public CardSuit GetValueCardSuit(string name, int index)
        {
            return GetValueInternal<CardSuit>(string.Format(ARRAY_FORMAT, name, index));
        }
        // Отримує параметр із заданим іменем у вигляді рядка
        public string GetValueString(string name)
        {
            return GetValueInternal<string>(name);
        }
        // Отримує параметр у масиві з заданим іменем у вигляді рядка
        public string GetValueString(string name, int index)
        {
            return GetValueInternal<string>(string.Format(ARRAY_FORMAT, name, index));
        }
        // Отримує параметр із вказаною назвою як ігрову карту
        public PlayingCard GetValueCard(string name)
        {
            return GetValueInternal<PlayingCard>(name);
        }
        // Отримує параметр у масиві з заданим іменем як гральної карти
        public PlayingCard GetValueCard(string name, int index)
        {
            return GetValueInternal<PlayingCard>(string.Format(ARRAY_FORMAT, name, index));
        }
        // Отримує параметр із вказаною назвою як колекція гральних карт
        public CardCollection GetValueCardCollection(string name)
        {
            return GetValueInternal<CardCollection>(name);
        }

        // Отримує збірку внутрішніх параметрів для цього стану гри
        public StateParameter[] GetParameterCollection()
        {
            return myParameters.Values.ToArray();
        }

        // Перевіряє, чи параметр стану дорівнює значенню
        public bool Equals(string name, object value)
        {
            if (myParameters.ContainsKey(name))
                if (myParameters[name].RawValue == null)
                    return value == null;
                else
                    return myParameters[name].RawValue.Equals(value);
            else
                return value == null;
        }

        // Кодує цей стан гри в мережеве повідомлення
        public void Encode(NetOutgoingMessage msg)
        {
            StateParameter[] toTransfer = myParameters.Values.Where(x => x.IsSynced).ToArray();

            msg.Write((int)toTransfer.Length);
            
            for(int index = 0; index < toTransfer.Length; index ++)
                toTransfer[index].Encode(msg);
        }

        // Декодує стан гри з даного повідомлення
        public void Decode(NetIncomingMessage msg)
        {
            int numParams = msg.ReadInt32();

            for (int index = 0; index < numParams; index++)
                StateParameter.Decode(msg, this);
        }

        // Декодує стан гри з даного повідомлення
        public static GameState CreateDecode(NetIncomingMessage msg)
        {
            GameState result = new GameState();
            result.Decode(msg);
            return result;
        }

        // Викликає подію оновленого параметра з заданим параметром
        internal void InvokeUpdated(StateParameter stateParameter)
        {
            if (stateParameter != null)
            {
                if (!SilentSets && OnStateChanged != null)
                    OnStateChanged(this, stateParameter);

                if (OnStateChangedUnSilenceable != null)
                    OnStateChangedUnSilenceable(this, stateParameter);

                if (myChangedEvents.ContainsKey(stateParameter.Name))
                    myChangedEvents[stateParameter.Name](this, stateParameter);
                else
                {
                    string name = myChangedEvents.Keys.FirstOrDefault(X => stateParameter.Name.Substring(1, stateParameter.Name.Length - 1) == X);
                    if (name != null) { myChangedEvents[name](this, stateParameter); }
                }
                
                Tuple<string, object> key = myStateEqualsEvents.Keys.FirstOrDefault(X => X.Item1 == stateParameter.Name);

                if (key != null && stateParameter.RawValue.Equals(key.Item2))
                {
                    myStateEqualsEvents[key](this, stateParameter);
                }
            }

        }
    }
}
