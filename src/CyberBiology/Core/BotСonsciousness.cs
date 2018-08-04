using System;

namespace CyberBiology.Core
{
    public class BotСonsciousness
    {
        private static readonly Random Random = new Random();

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
                    hash += (int)_actions[0].Action;
                }
            }
        }

        public BotСonsciousness()
        {
            for (int i = 0; i < 64; i++)
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

        public bool IsRelative(BotСonsciousness other)
        {
            var abs = Math.Abs(hash - other.hash);
            if (abs < 100)
                return true;

            return false;
        }

        public void Mutate()
        {
            int index = (int)(Random.NextDouble() * Size); 

            var action = _actions[index];
            action.Mutate();

            UpdateHash();
        }

        public void TransferFrom(BotСonsciousness otherСonsciousness)
        {
            for (int i = 0; i < Size; i++)
            {
                _actions[i].TranferFrom(otherСonsciousness._actions[i]);
            }

            UpdateHash();
        }
    }
}