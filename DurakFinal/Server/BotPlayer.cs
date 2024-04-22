using Durak.Common;
using Durak.Common.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Durak.Server
{
    // Представляє AI-бота, який виконує дії від імені гравця
    public class BotPlayer
    {
        // Отримує або задає, чи ботам потрібно затрачати час на прийняття рішення
        public static bool SimulateThinkTime = true;
        // Мінімальний час для прийняття ботом рішення
        public static int ThinkSleepMinTime = 1000;
        // Максимальний час для прийняття ботом рішення
        public static int ThinkSleepMaxTime = 4000;
        // Стандартна складність бота від 0 до 1
        public static float DefaultDifficulty = 1;

        // Зберігає, чи цей бот має викликати на кінці поточного циклу повідомлень
        private bool shouldInvoke;
        // Зберігає гравця, якого представляє цей бот
        private Player myPlayer;
        // Зберігає сервер, на якому працює бот
        private GameServer myServer;
        // Зберігає складність бота в діапазоні від 0 до 1
        private float myDifficulty;
        // Зберігає генератор випадкових чисел для цього бота
        private Random myRandom;
        // Зберігає екземпляр таймера для очікування результату зворотного виклику. Ми чекаємо на таймер перед оновленням нашого shouldInvoke
        private Timer myTimer;
        // Зберігає словник запропонованих ходів, очищений під час визначення нового ходу
        private Dictionary<PlayingCard, float> myProposedMoves;

        // Отримує або задає складність для цього бота в діапазоні від 0 до 1
        public float Difficulty
        {
            get { return myDifficulty; }
            set
            {
                // Затискаємо складність між 0 і 1
                myDifficulty = value < 0 ? 0 : value > 1 ? 1 : value;
            }
        }
        // Отримує або задає, чи цей бот має оновлювати свою логіку
        public bool ShouldInvoke
        {
            get { return shouldInvoke; }
            set { shouldInvoke = value; }
        }
        // Отримує гравця, якого представляє цей бот
        public Player Player
        {
            get { return myPlayer; }
        }

        // Створює нового бота для вказаного ігрового сервера та гравця
        public BotPlayer(GameServer server, Player player, float difficulty)
        {
            myServer = server;
            myPlayer = player;
            myDifficulty = difficulty;

            myRandom = new Random(player.PlayerId + DateTime.Now.Millisecond);
            myTimer = new Timer();
            myTimer.Elapsed += TimerElapsed;

            myProposedMoves = new Dictionary<PlayingCard, float>();
            player.Hand.OnCardRemoved += CardRemoved;
            player.Hand.OnCardAdded += CardAdded;
        }

        // Викликається, коли карта була видалена з руки гравця
        private void CardRemoved(object sender, CardEventArgs e)
        {
            if (myProposedMoves.ContainsKey(e.Card))
                myProposedMoves.Remove(e.Card);
        }

        // Викликається, коли карта була додана до руки гравця
        private void CardAdded(object sender, CardEventArgs e)
        {
            e.Card.FaceUp = true;

            if (!myProposedMoves.ContainsKey(e.Card))
                myProposedMoves.Add(e.Card, 0.0f);
        }

        // Викликається, коли таймер спрацював, це оновлює наш shouldInvoke після заданого "часу обдумування" для бота
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            shouldInvoke = true;
            myTimer.Stop();
            myServer.PumpMessages();
        }

        // Викликається, коли стан гри було оновлено
        public void StateUpdated()
        {
            // Перевизначте shouldInvoke, щоб ми не могли випадково його встановити
            bool shouldInvoke = InstantValidateCheck();

            if (shouldInvoke & !myTimer.Enabled)
            {
                // Якщо ми симулюємо час розміркування, тоді підготуйте таймер і запустіть його
                if (SimulateThinkTime)
                {
                    // Розрахуйте випадковий інтервал на основі мінімального та максимального часу та "вміння" бота
                    myTimer.Interval = ThinkSleepMinTime + (myRandom.Next(0, ThinkSleepMaxTime - ThinkSleepMinTime) * (1 - myRandom.NextDouble() * (1 - myDifficulty)));
                    // Запустити таймер
                    myTimer.Start();
                }
                else
                    // Скопіювати локальний метод у екземплярний член
                    this.shouldInvoke = shouldInvoke;
            }
        }

        // Миттєво перевіряє, чи цей бот може грати, це використовується з таймерами обдумування, щоб переконатися, що все ще хід цього бота
        public bool InstantValidateCheck()
        {
            // Перевизначте shouldInvoke, щоб ми не могли випадково встановити його
            bool shouldInvoke = Rules.BOT_INVOKE_RULES.Count > 0;

            // Перебирайте кожне правило стану і встановлюйте false, якщо одне з них не вдалося
            foreach (IBotInvokeStateChecker stateChecker in Rules.BOT_INVOKE_RULES)
            {
                // Якщо одна перевірка стану не вдається, всі вони не вдаються
                if (!stateChecker.ShouldInvoke(myServer, this))
                {
                    shouldInvoke = false;
                    break;
                }
            }

            return shouldInvoke;
        }

        // Визначає хід, який повинен зробити бот
        public PlayingCard DetermineMove()
        {
            // Створюємо копію ключів, щоб безпечно можна було модифікувати словник
            var keys = myProposedMoves.Keys.ToList();

            foreach (var key in keys)
            {
                myProposedMoves[key] = 0;
            }

            // Впевненість у тому, що не потрібно робити жодного ходу
            float noMoveConfidence = 0;

            // Перебір усіх правил
            foreach (IAIRule rule in Rules.AI_RULES)
            {
                // Отримати запропонований хід
                rule.Propose(myProposedMoves, myServer, myPlayer.Hand);
            }

            foreach (var key in myProposedMoves.Keys.ToList())
            {
                myProposedMoves[key] *= (float)((myRandom.NextDouble() - 0.5d) * (1 - myDifficulty / 4));
            }

            // Сортування списку
            List<KeyValuePair<PlayingCard, float>> sortedList = myProposedMoves.ToList();
            sortedList.Sort((X, Y) => -(X.Value.CompareTo(Y.Value)));

            // Скидання цього, щоб уникнути безкінечних циклів, повинно бути встановлене як false при оновленні стану
            shouldInvoke = false;

            // Перебір списку та спроба кожного ходу, якщо він дійсний, тоді поверніть цей хід
            for (int index = 0; index < sortedList.Count; index++)
                if (sortedList[index].Value > noMoveConfidence && myServer.CanPlayMove(myPlayer, sortedList[index].Key))
                    return sortedList[index].Key;

            // Якщо не було хорошого ходу, повернути null
            return null;
        }


        // Створює бота гравця
        public static BotPlayer CreateBot(Player player, GameServer gameServer)
        {
            Player clone = player.Clone();
            return new BotPlayer(gameServer, player, DefaultDifficulty);
        }
    }
}
