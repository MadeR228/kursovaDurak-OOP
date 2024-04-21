using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durak.Common.Cards
{
    // Представляє аргумент події для карток
    public class CardEventArgs : EventArgs
    {
        // Отримує або задає карту для цієї події
        public PlayingCard Card { get; set; }

        // Створює новий екземпляр аргументу події для карток
        public CardEventArgs(PlayingCard card)
        {
            Card = card;
        }
    }
}
