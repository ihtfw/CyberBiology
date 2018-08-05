using System;
using System.Runtime.CompilerServices;
using CyberBiology.Core.Enums;

namespace CyberBiology.Core
{
    public class Bot
    {
        private float _mineral;
        private float _health;

        public const float MinHealth = 0.01f;
        public const float MaxHealth = 100f;
        public const float MaxMinerals = 100f;

        private static readonly Random Random = new Random();

        public int X;
        public int Y;

        public float Health
        {
            get => _health;
            set
            {
                _health = value;
                if (_health < 0.01)
                {
                    TryConvertToOrganic();
                }
            }
        }

        public float Mineral
        {
            get => _mineral;
            set
            {
                _mineral = value;
                if (_mineral < 0)
                {
                    _mineral = 0;
                }
                else if (_mineral > MaxMinerals)
                {
                    _mineral = MaxMinerals;
                }
            }
        }

        public Direction Direction { get; set; }
        
        public BotСonsciousness Consciousness { get; } = new BotСonsciousness();

        public BotColor Color { get; } = new BotColor();
        public BotState State { get; private set; }
        
        public Bot(int x, int y)
        {
            X = x;
            Y = y;

            Direction = Direction.NORTHEAST;
            Reset();
        }

        public void Reset()
        {
            Color.Reset();
            Mineral = 0;
            Health = 35;
            State = BotState.Alive;
        }

        public bool IsAlive => State == BotState.Alive;

        public bool IsDead => !IsAlive;

        public bool IsOrganic => State == BotState.Organic;
        
        public void ConvertMineralToHealth(float value = 10f)
        {
            if (value > 10)
            {
                value = 10;
            }

            if (value > Mineral)
            {
                value = Mineral;
            }

            Color.GoBlue(value);

            Health += 4 * value;
            Mineral -= value;
        }

        public bool TryMove()
        {
            World.Instance.CountAroundFor(this, out int relative, out _, out int empty);
            if (relative > 1 || empty == 0)
                return false;

            if (!CheckDirectionFor(CheckResult.Empty, out _))
            {
                return false;
            }

            World.Instance.Move(this, Direction);

            return true;
        }


        public void Photosynthesis()
        {
            World.Instance.CountAroundFor(this, out int relative, out _, out _);

            float mineralK = 2 / (Mineral + 1);

            float groupK = relative * 4f;

            var val = (groupK +  8 + mineralK ) / (Y + 40);
            Health += val;   

            Color.GoGreen(val); 
        }

        public void AccumulateMinerals()
        {
            Mineral += (float) Y / World.Instance.Height / 2f;
        }

        public bool TryEatOrganic()
        {
            if (!CheckDirectionFor(CheckResult.Organic, out var otherBot))
            {
                return false;
            }

            World.Instance.Delete(otherBot);

            Health += 5f;
            Color.GoRed(5f);

            return true;
        }

        public bool TryEatOtherBot()
        {
            if (!CheckDirectionFor(CheckResult.OtherBot, out var otherBot))
            {
                return false;
            }

            TryEat(otherBot);

            return true;
        }

        private bool TryEat(Bot victim)
        {
            if (Mineral >= victim.Mineral)
            {
                Mineral -= victim.Mineral; 

                World.Instance.Delete(victim);         

                var cl = victim.Health / 4f;           
                Health += cl;
                Color.GoRed(cl);                  

                return true;                              
            }

            victim.Mineral -= Mineral;
            Mineral = 0; 

            if (Health >= 2 * victim.Mineral)
            {
                World.Instance.Delete(victim);

                var cl = victim.Health / 4f - 2f * victim.Mineral; 
                if (cl < 0) { cl = 0; }

                Health += cl;
                Color.GoRed(cl);                 

                return true;
            }

            victim.Mineral -= Health / 2f; 
            Health = 0;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CheckDirectionFor(CheckResult lookFor, out Bot otherBot)
        {
            //if (World.Instance.TryFindDirection(this, lookFor, out var direction, out otherBot))
                if (World.Instance.CheckDirectionFor(this, lookFor, Direction, out otherBot))
            {
                //Direction = direction;
                return true;
            }

            return false;
        }

        public bool TryLook(CheckResult lookFor)
        {
            if (World.Instance.TryFindDirection(this, lookFor, out var direction, out _))
            {
                Direction = direction;
                return true;
            }

            return false;
        }
        /*
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryLook(CheckResult lookFor, out Bot otherBot)
        {

            if (World.Instance.TryFindDirection(this, lookFor, out var direction, out otherBot))
            {
                Direction = direction;
                return true;
            }

            return false;
        }*/
        
        public bool TryShare()
        {
            if (!CheckDirectionFor(CheckResult.RelativeBot, out var otherBot))
                return false;
            
            if (Health > otherBot.Health)
            {              
                var newHealth = (Health + otherBot.Health) / 2f;   
                Health = newHealth;
                otherBot.Health = newHealth;
            }

            if (Mineral > otherBot.Mineral)
            {             
                var newMineral = (Mineral + otherBot.Mineral) / 2f;   
                Mineral =  newMineral;
                otherBot.Mineral = newMineral;
            }

            return true;
        }

        public bool TryGive()
        {
            if (!CheckDirectionFor(CheckResult.RelativeBot, out var otherBot))
                return false;
            
            var giveHealth = Health / 4f;
            Health -= giveHealth;
            otherBot.Health += giveHealth;

            var giveMineral = Mineral / 4f;
            Mineral -= giveMineral;
            otherBot.Mineral += giveMineral;

            return true;
        }
        
        public bool TryGeneAttack()
        {
            if (!CheckDirectionFor(CheckResult.OtherBot, out var otherBot))
                return false;

            Health -= 1f;
            if (IsDead)
                return false;
            
            otherBot.Consciousness.Mutate();

            return true;
        }
        
        public void BotDivision()
        {
            Health -= 15;

            if (IsDead)
                return;

            if (!World.Instance.TryFindDirection(this, CheckResult.Empty, out var direction, out _))
            {
                //Health = Health / 2f;
                Health = 0;
                return;
            }
            
            Bot newbot = BotFactory.Get(World.Instance.LimitX(X + direction.Dx), World.Instance.LimitY(Y + direction.Dy));
            newbot.Reset();
            newbot.Consciousness.TransferFrom(Consciousness);

            if (Random.NextDouble() < 0.25)
            {
                newbot.Consciousness.Mutate();
            }

            Health = Health / 2f;
            newbot.Health = Health;

            Mineral = Mineral / 2f;
            newbot.Mineral = Mineral;

            newbot.Color.CopyFrom(Color);

            newbot.Direction = Direction.Random();

            World.Instance.Matrix[newbot.X, newbot.Y] = newbot;
        }
        
        public bool TryConvertToOrganic()
        {
            if (Health >= MinHealth)
            {
                return false;
            }

            State = BotState.Organic;
            
            return true;
        }
        
        public override string ToString()
        {
            return $"({X},{Y}) {State} H:{Health} M:{Mineral}";
        }

        public bool IsSurrounded()
        {
            if (World.Instance.TryFindDirection(this, CheckResult.Empty, out _, out _))
            {
                return false;
            }

            return true;
        }
    }
}
