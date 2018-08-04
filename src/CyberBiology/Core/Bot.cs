using System;
using System.Runtime.CompilerServices;
using CyberBiology.Core.Enums;

namespace CyberBiology.Core
{
    public class Bot
    {
        public const int MaxHealth = 1000;
        public const int MaxMinerals = 1000;

        private static readonly Random Random = new Random();

        public int X;
        public int Y;
        public int Health;
        public int Mineral;
        private Bot _prevBot;
        private Bot _nextBot;

        public Direction Direction { get; set; }

        public Bot PrevBot
        {
            get => _prevBot;
            set
            {
                _prevBot = value;
                UpdateGroup();
            }
        }

        public Bot NextBot
        {
            get => _nextBot;
            set
            {
                _nextBot = value;
                UpdateGroup();
            }
        }

        public BotСonsciousness Consciousness { get; } = new BotСonsciousness();

        public BotColor Color { get; } = new BotColor();
        public BotState State { get; private set; }
        
        public Bot(int x, int y)
        {
            X = x;
            Y = y;

            Direction = Direction.NORTHEAST;
            Health = 5;
            State = BotState.Alive;
        }

        public bool IsAlive => State == BotState.Alive;

        public bool IsDead => !IsAlive;

        public bool IsOrganic => State == BotState.Organic;
        
        public Group Group { get; private set; } = Group.Alone;

        private void UpdateGroup()
        {
            Group g = 0;
            if (PrevBot != null)
            {
                g |= Group.HasPrev;
            }
            if (NextBot != null)
            {
                g |= Group.HasNext;
            }

            if (g == 0)
            {
                Group = Group.Alone;
                return;
            }

            Group = g;
        }
        
        public void ConvertMineralToEnergy(int value = 100)
        {
            if (value > 100)
            {
                value = 100;
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
            if (!TryLook(CheckResult.Empty, out _))
            {
                return false;
            }

            World.Instance.Move(this, Direction);

            return true;
        }

        public bool TryPhotosynthesis()
        {
            int mineralK;
            if (Mineral < 100)
            {
                mineralK = 0;
            }
            else if (Mineral < 400)
            {
                mineralK = 1;
            }
            else
            {
                mineralK = 2;
            }

            int groupK = 0;
            if (PrevBot != null)
            {
                groupK = groupK + 4;
            }
            if (NextBot != null)
            {
                groupK = groupK + 4;
            }

            int hlt = groupK + 1 * (11 - (15 * Y / World.Instance.Height) + mineralK); // формула вычисления энергии 
            if (hlt > 0)
            {
                Health = Health + hlt;   // прибавляем полученную энергия к энергии бота
                Color.GoGreen(hlt); // бот от этого зеленеет

                return true;
            }

            return false;
        }

        public bool TryEatOrganic()
        {
            Health = Health - 4; // бот теряет на этом 4 энергии в независимости от результата

            if (!TryLook(CheckResult.Organic, out var otherBot))
            {
                return false;
            }

            World.Instance.Delete(otherBot);

            Health = Health + 100;
            Color.GoRed(100);

            return true;
        }

        public bool TryEatOtherBot()
        {
            Health = Health - 4; // бот теряет на этом 4 энергии в независимости от результата

            if (!TryLook(CheckResult.OtherBot, out var otherBot))
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
                Mineral -= victim.Mineral; // количество минералов у бота уменьшается на количество минералов у жертвы
                // типа, стесал свои зубы о панцирь жертвы
                World.Instance.Delete(victim);          // удаляем жертву из списков

                int cl = 100 + (victim.Health / 2);           // количество энергии у бота прибавляется на 100+(половина от энергии жертвы)
                Health += cl;
                Color.GoRed(cl);                    // бот краснеет

                return true;                              // возвращаем 5
            }

            victim.Mineral -= Mineral;
            Mineral = 0; // бот израсходовал все свои минералы на преодоление защиты
            //------ если здоровья в 2 раза больше, чем минералов у жертвы  ------
            //------ то здоровьем проламываем минералы ---------------------------
            if (Health >= 2 * victim.Mineral)
            {
                World.Instance.Delete(victim);

                int cl = 100 + (victim.Health / 2) - 2 * victim.Mineral; // вычисляем, сколько энергии смог получить бот
                Health += cl;
                if (cl < 0) { cl = 0; } //========================================================================================ЗАПЛАТКА!!!!!!!!!!! - энергия не должна быть отрицательной

                Color.GoRed(cl);                    // бот краснеет

                return true;
            }

            //--- если здоровья меньше, чем (минералов у жертвы)*2, то бот погибает от жертвы
            victim.Mineral -= Health / 2;  // у жертвы минералы истраченны
            Health = 0;  // здоровье уходит в ноль

            return false;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryLook(CheckResult lookFor, out Bot otherBot)
        {
            if (World.Instance.TryFindDirection(this, lookFor, out var direction, out otherBot))
            {
                Direction = direction;
                return true;
            }

            return false;
        }
        
        public bool TryShare()
        {
            if (!TryLook(CheckResult.RelativeBot, out var otherBot))
                return false;
            
            if (Health > otherBot.Health)
            {              // если у бота больше энергии, чем у соседа
                int hlt = (Health - otherBot.Health) / 2;   // то распределяем энергию поровну
                Health -= hlt;
                otherBot.Health += hlt;
            }

            if (Mineral > otherBot.Mineral)
            {              // если у бота больше минералов, чем у соседа
                int min = (Mineral - otherBot.Mineral) / 2;   // то распределяем их поровну
                Mineral -=  min;
                otherBot.Mineral +=  min;
            }

            return true;
        }

        public bool TryGive()
        {
            if (!TryLook(CheckResult.RelativeBot, out var otherBot))
                return false;
            
            int giveHealth = Health / 4;
            Health -= giveHealth;

            var giveMineral = Mineral / 4;
            if (giveMineral > 0)
            {
                Mineral -= giveMineral;
                otherBot.Mineral += giveMineral;
            }

            return true;
        }
        
        public bool TryGeneAttack()
        {
            if (!TryLook(CheckResult.OtherBot, out var otherBot))
                return false;

            Health -= 10; 

            if (Health > 0)
            {
                otherBot.Consciousness.Mutate();
            }

            return true;
        }
        
        /// <summary>
        /// Some kind of magic. 
        /// </summary>
        /// <returns></returns>
        public bool IsHealthGrow()
        {
            int t;
            if (Mineral < 100)
            {
                t = 0;
            }
            else
            {
                t = Mineral < 400 ? 1 : 2;
            }

            int hlt = 10 - (15 * Y / World.Instance.Height) + t; 
            if (hlt >= 3)
            {
                return true;
            }

            return false;
        }

        public void BotDivision()
        {
            if (Group == Group.Both || Group == Group.Alone)
            {
                CreateFreeChild();
            }
            else
            {
                TryCreateChildAsPart();
            }
        }
        
        private bool TryCreateChildAsPart()
        {
            if (Group == Group.Both)
            {
                return false;
            }

            var newBot = CreateFreeChild();
            if (newBot == null)
                return false;

            if (Group == Group.HasPrev)
            {                   
                NextBot = newBot; 
                newBot.PrevBot = this;  
            }
            else
            {                             
                PrevBot = newBot; 
                newBot.NextBot = this; 
            }

            return true;
        }

        private Bot CreateFreeChild()
        {
            Health -= 150;      // бот затрачивает 150 единиц энергии на создание копии
            if (Health <= 0)
            {
                return null;
            }

            if (!TryLook(CheckResult.Empty, out _))
            {
                // если бот окружен, то он в муках погибает
                Health = 0;
                return null;
            }

            Bot newbot = BotFactory.Get(World.Instance.LimitX(X + Direction.Dx), World.Instance.LimitY(Y + Direction.Dy));
            newbot.Consciousness.TransferFrom(Consciousness);

            if (Random.NextDouble() < 0.25)
            {
                newbot.Consciousness.Mutate();
            }

            Health = Health / 2;
            newbot.Health = Health;

            Mineral = Mineral / 2;
            newbot.Mineral = Mineral;

            newbot.Color.CopyFrom(Color);

            newbot.Direction = Direction.Random();

            World.Instance.Matrix[newbot.X, newbot.Y] = newbot;

            return newbot;
        }

        
        public bool TryConvertToOrganic()
        {
            if (Health >= 1)
            {
                return false;
            }

            State = BotState.Organic;

            if (PrevBot != null)
            {
                PrevBot.NextBot = null;
            } 

            if (NextBot != null)
            {
                NextBot.PrevBot = null;
            }

            PrevBot = null;
            NextBot = null;

            return true;
        }

        public bool TryAccumulateMinerals()
        {
            // если бот находится на глубине ниже 48 уровня
            // то он автоматом накапливает минералы, но не более 999
            if (Y <= World.Instance.Height / 2) return false;

            Mineral++;

            if (Y > World.Instance.Height / 6 * 4)
            {
                Mineral++;
            }

            if (Y > World.Instance.Height / 6 * 5)
            {
                Mineral++;
            }

            if (Mineral > 999) { Mineral = 999; }

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
