using System;
using CyberBiology.Core.Enums;
using CyberBiology.Core.Serialization;

namespace CyberBiology.Core
{
    public class BotAction
    {
        private static readonly int MinActionValue = int.MaxValue;
        private static readonly int MaxActionValue = int.MinValue;
        private Actions _action = Actions.Photosynthesis;

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

        public Actions Action
        {
            get => _action;
            private set
            {
                _action = value;

                switch (Action)
                {
                    case Actions.Skip:
                    case Actions.Rotate:
                    case Actions.Look:
                    case Actions.Eat:
                        HasParam = true;
                        break;
                }
            }
        }

        public bool HasParam { get; private set; }
        
        public void Mutate()
        {
            Action = (Actions)(Utils.Next(1000) % (MaxActionValue + MinActionValue));
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

        public void TranferFrom(BotAction action)
        {
            Action = action.Action;
        }

        public void Reset()
        {
            Action = Actions.Photosynthesis;
        }

        public void Load(BotActionDto botActionDto)
        {
            Action = (Actions)botActionDto.Action;
        }
    }
}