using System;
using CyberBiology.Core.Enums;

namespace CyberBiology.Core
{
    public class BotAction
    {
        private static readonly Random Random = new Random();

        private static readonly int MinActionValue = int.MaxValue;
        private static readonly int MaxActionValue = int.MinValue;

        static BotAction()
        {
            foreach (int value in Enum.GetValues(typeof(Actions)))
            {
                MinActionValue = MinActionValue > value ?  value : MinActionValue;
                MaxActionValue = MaxActionValue < value ?  value : MaxActionValue;
            }
        }

        public BotAction(int index)
        {
            Index = index;
        }

        public int Index { get; }

        public Actions Action { get; private set; } = Actions.Photosynthesis;
        
        public bool HasParam()
        {
            switch (Action)
            {
                case Actions.CheckHealth:
                case Actions.CheckMinerals:
                case Actions.Skip:
                    return true;
                default:
                    return false;
            }
        }
        
        public void Mutate()
        {
            Action = (Actions)(Random.NextDouble() * (MaxActionValue + MinActionValue));
        }

        public bool IsStopAction()
        {
            switch (Action)
            {
                case Actions.Move:
                case Actions.BotDivision:
                case Actions.Skip:
                    return true;
            }

            return false;
        }

        public BotAction Clone()
        {
            return new BotAction(Index)
            {
                Action = Action
            };
        }

        public bool IsValid()
        {
            if ((int)Action < MinActionValue)
                return false;

            if ((int)Action > MaxActionValue)
                return false;

            return true;
        }

        public override string ToString()
        {
            return $"{Index}: {Action}";
        }
    }
}