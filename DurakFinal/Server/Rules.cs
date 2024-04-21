using Durak.Common;
using Durak.Server;
using System;
using System.Collections.Generic;

namespace Durak.Server
{
    // Клас-утиліта, який створює екземпляри всіх типів правил
    public static class Rules
    {
        // Отримує список всіх правил перевірки стану гри
        public static readonly List<IGameStateRule> STATE_RULES;
        // Отримує список всіх правил ініціалізації гри
        public static readonly List<IGameInitRule> INIT_RULES;
        // Отримує список всіх правил ходу гри
        public static readonly List<IGamePlayRule> PLAY_RULES;
        // Отримує список всіх правил, які виконуються після успішного ходу
        public static readonly List<IMoveSuccessRule> MOVE_SUCCESS_RULES;
        // Отримує список всіх правил ботів, що складають штучний інтелект ботів
        public static readonly List<IAIRule> AI_RULES;
        // Отримує список всіх правил ботів, які визначають, коли потрібно викликати бота
        public static readonly List<IBotInvokeStateChecker> BOT_INVOKE_RULES;
        // Отримує список всіх валідаторів серверної сторони, які перевіряють, які стани може встановлювати клієнт
        public static readonly List<IClientStateSetValidator> CLIENT_STATE_REQ_VALIDATORS;

        // Статичний ініціалізатор, завантажує всі типи
        static Rules()
        {
            STATE_RULES = new List<IGameStateRule>();
            Utils.FillTypeList(AppDomain.CurrentDomain, STATE_RULES);

            INIT_RULES = new List<IGameInitRule>();
            Utils.FillTypeList(AppDomain.CurrentDomain, INIT_RULES);
            INIT_RULES.Sort((X, Y) => { return Y.Priority.CompareTo(X.Priority); });

            PLAY_RULES = new List<IGamePlayRule>();
            Utils.FillTypeList(AppDomain.CurrentDomain, PLAY_RULES);

            AI_RULES = new List<IAIRule>();
            Utils.FillTypeList(AppDomain.CurrentDomain, AI_RULES);

            CLIENT_STATE_REQ_VALIDATORS = new List<IClientStateSetValidator>();
            Utils.FillTypeList(AppDomain.CurrentDomain, CLIENT_STATE_REQ_VALIDATORS);

            BOT_INVOKE_RULES = new List<IBotInvokeStateChecker>();
            Utils.FillTypeList(AppDomain.CurrentDomain, BOT_INVOKE_RULES);

            MOVE_SUCCESS_RULES = new List<IMoveSuccessRule>();
            Utils.FillTypeList(AppDomain.CurrentDomain, MOVE_SUCCESS_RULES);
        }
    }
}
