using System;
using CyberBiology.Core.Serialization;

namespace CyberBiology.Core
{
    public class Consciousness
    {
        public const int Size = 64;

        private int _currentActionIndex;

        private readonly BotAction[] _actions = new BotAction[Size];

        private int hash;

        private void UpdateHash()
        {
            unchecked
            {
                hash = 0;
                for (var i = 0; i < _actions.Length; i++)
                {
                    hash += (int)_actions[i].Action;
                }
            }
        }

        public Consciousness()
        {
            for (int i = 0; i < Size; i++)
            {
                _actions[i] = new BotAction(i);
            }

            UpdateHash();
        }

        public void SkipActions(int count)
        {
            _currentActionIndex = (_currentActionIndex + count) % Size;
        }
        
        public BotAction NextAction()
        {
            var action = _actions[_currentActionIndex];

            SkipActions(action.HasParam ? 2 : 1);

            return action;
        }

        public int Param(BotAction botAction)
        {
            var ind = (botAction.Index + 1) % Size;
            return (int)_actions[ind].Action;
        }

        public bool IsRelative(Consciousness other)
        {
            var abs = Math.Abs(hash - other.hash);
            if (abs < 4)
                return true;

            return false;
        }

        public void Mutate()
        {
            int index = Utils.Next(1000) % Size;

            var action = _actions[index];
            action.Mutate();

            UpdateHash();
        }

        public void TransferFrom(Consciousness otherConsciousness)
        {
            for (int i = 0; i < Size; i++)
            {
                _actions[i].TranferFrom(otherConsciousness._actions[i]);
            }

            UpdateHash();
        }

        public BotAction Get(int index)
        {
            return _actions[index];
        }

        public void Reset()
        {
            for (int i = 0; i < Size; i++)
            {
                _actions[i].Reset();
            }
        }

        public void Load(ConsciousnessDto consciousnessDto)
        {
            Reset();

            if (consciousnessDto == null)
            {
                return;
            }

            foreach (var action in consciousnessDto.Actions)
            {
                _actions[action.Index].Load(action);
            }
        }
    }
}