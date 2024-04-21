using Durak.Common.Cards;

namespace Durak.Server
{
    // Представляє хід, запропонований штучним інтелектом
    public struct AIMoveProposal
    {
        private float myConfidence;

        // Отримує або встановлює впевненість для цього результату (в діапазоні 0-1)
        public float Confidence
        {
            get { return myConfidence; }
            set
            {
                myConfidence = value < 0 ? 0 : value > 1 ? 1 : 0;
            }
        }
        // Отримує або встановлює карту, яку пропонується розіграти
        public PlayingCard Move { get; set; }

        // Створює нову пропозицію переміщення штучного інтелекту
        public AIMoveProposal(PlayingCard card, float confidence)
        {
            myConfidence = confidence;
            Move = card;
            Confidence = confidence;
        }

        // Замінює логіку виявлення впевненості, щоб вручну встановити впевненість
        public void SetConfidenceOverride(float confidence)
        {
            myConfidence = confidence;
        }
    }
}