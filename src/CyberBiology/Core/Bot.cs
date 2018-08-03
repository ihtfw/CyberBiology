using System;
using CyberBiology.Core.Enums;

namespace CyberBiology.Core
{
    public class Bot
    {
        public const int MaxHealth = 1000;
        public const int MaxMinerals = 1000;

        private static readonly Random Random = new Random();

        public int x;
        public int y;
        public int health;
        public int mineral;
        
        public int Direction { get; private set; }
        
        public Bot mprev;
        public Bot mnext;

        public BotСonsciousness Consciousness { get; } = new BotСonsciousness();

        public BotColor Color { get; } = new BotColor();
        public BotState State { get; private set; }

        public Bot()
        {
            Direction = 2;
            health = 5;
            State = BotState.Alive;
        }

        private Bot(int x, int y) : this()
        {
            this.x = x;
            this.y = y;
        }

        public bool IsAlive => State == BotState.Alive;

        public bool IsDead => !IsAlive;

        public bool IsOrganic => State == BotState.Organic;

        public void SetDirection(int value)
        {
            Direction = value % 8;
        }

        public void Rotate(int value)
        {
            int newdrct = Direction + value;
            if (newdrct >= 8)
            {
                newdrct = newdrct - 8;
            }
            Direction = newdrct;
        }
        
        public int CountY(int direction)
        {
            direction = direction % 8;
            if (direction == 0 || direction == 1 || direction == 2)
            {
                return World.Instance.LimitY(y - 1);
            }

            if (direction == 4 || direction == 5 || direction == 6)
            {
                return World.Instance.LimitY(y + 1);
            }

            return y;
        }

        public int CountX(int direction)
        {
            direction = direction % 8;
            if (direction == 0 || direction == 6 || direction == 7)
            {
                return World.Instance.LimitX(x - 1);
            }

            if (direction == 2 || direction == 3 || direction == 4)
            {
                return World.Instance.LimitX(x + 1);
            }

            return x;
        }
        
        public bool TryFindDirection(CheckResult lookFor, out int direction)
        {
            var directionOffset = Random.Next(100);
            for (int i = 0; i < 8; i++)
            {
                var xt = CountX(i + directionOffset);
                var yt = CountY(i + directionOffset);

                var checkResult = World.Instance.Check(this, xt, yt);
                if (checkResult == lookFor)
                {
                    direction = (i + directionOffset) % 8;
                    return true;
                }
            }

            direction = -1;
            return false;
        }

        public Group Group()
        {
            Group g = 0;
            if (mprev != null)
            {
                g |= Core.Group.HasPrev;
            }
            if (mnext != null)
            {
                g |= Core.Group.HasNext;
            }

            if (g == 0)
            {
                return Core.Group.Alone;
            }
            return g;
        }
        
        public void ConvertMineralToEnergy(int value = 100)
        {
            if (value > 100)
            {
                value = 100;
            }

            if (value > mineral)
            {
                value = mineral;
            }

            Color.GoBlue(value);
            health += 4 * value;
            mineral -= value;
        }

        public bool TryMove()
        {
            if (!TryLook(CheckResult.Empty))
            {
                return false;
            }

            int xt = CountX(Direction);
            int yt = CountY(Direction);
            
            var checkResult = World.Instance.Check(this, xt, yt);
            if (checkResult == CheckResult.Empty)
            {
                World.Instance.Matrix[xt, yt] = this;
                World.Instance.Matrix[x, y] = null;
                x = xt;
                y = yt;
            }

            return true;
        }

        public bool TryPhotosynthesis()
        {
            int mineralK;
            if (mineral < 100)
            {
                mineralK = 0;
            }
            else if (mineral < 400)
            {
                mineralK = 1;
            }
            else
            {
                mineralK = 2;
            }

            int groupK = 0;
            if (mprev != null)
            {
                groupK = groupK + 4;
            }
            if (mnext != null)
            {
                groupK = groupK + 4;
            }

            int hlt = groupK + 1 * (11 - (15 * y / World.Instance.Height) + mineralK); // формула вычисления энергии 
            if (hlt > 0)
            {
                health = health + hlt;   // прибавляем полученную энергия к энергии бота
                Color.GoGreen(hlt); // бот от этого зеленеет

                return true;
            }

            return false;
        }

        public bool TryEat()
        {
            health = health - 4; // бот теряет на этом 4 энергии в независимости от результата

            if (!TryLook(CheckResult.Organic))
            {
                if (!TryLook(CheckResult.OtherBot))
                {
                    return false;
                }
            }

            int xt = CountX(Direction);
            int yt = CountY(Direction);
            
            var checkResult = World.Instance.Check(this, xt, yt);
            switch (checkResult)
            {
                case CheckResult.Organic:
                    World.Instance.Delete(xt, yt);
                    health = health + 100; 
                    Color.GoRed(100);
                    return true;

                case CheckResult.OtherBot:
                    TryEat(World.Instance.Matrix[xt, yt]);
                    return true;
            }

            return false;
        }

        private bool TryEat(Bot victim)
        {
            if (mineral >= victim.mineral)
            {
                mineral -= victim.mineral; // количество минералов у бота уменьшается на количество минералов у жертвы
                // типа, стесал свои зубы о панцирь жертвы
                World.Instance.Delete(victim);          // удаляем жертву из списков

                int cl = 100 + (victim.health / 2);           // количество энергии у бота прибавляется на 100+(половина от энергии жертвы)
                health += cl;
                Color.GoRed(cl);                    // бот краснеет

                return true;                              // возвращаем 5
            }

            victim.mineral -= mineral;
            mineral = 0; // бот израсходовал все свои минералы на преодоление защиты
            //------ если здоровья в 2 раза больше, чем минералов у жертвы  ------
            //------ то здоровьем проламываем минералы ---------------------------
            if (health >= 2 * victim.mineral)
            {
                World.Instance.Delete(victim);

                int cl = 100 + (victim.health / 2) - 2 * victim.mineral; // вычисляем, сколько энергии смог получить бот
                health += cl;
                if (cl < 0) { cl = 0; } //========================================================================================ЗАПЛАТКА!!!!!!!!!!! - энергия не должна быть отрицательной

                Color.GoRed(cl);                    // бот краснеет

                return true;
            }

            //--- если здоровья меньше, чем (минералов у жертвы)*2, то бот погибает от жертвы
            victim.mineral -= health / 2;  // у жертвы минералы истраченны
            health = 0;  // здоровье уходит в ноль

            return false;
        }

        public bool TryLook(CheckResult lookFor)
        {
            if (TryFindDirection(lookFor, out var direction))
            {
                SetDirection(direction);

                return true;
            }

            return false;
        }
        
        public CheckResult Share()
        {
            int xt = CountX(Direction);
            int yt = CountY(Direction);

            var checkResult = World.Instance.Check(this, xt, yt);
            if (checkResult != CheckResult.RelativeBot)
            {
                return checkResult;
            }

            //------- если мы здесь, то в данном направлении живой ----------
            var otherBot = World.Instance.Matrix[xt, yt];
            if (health > otherBot.health)
            {              // если у бота больше энергии, чем у соседа
                int hlt = (health - otherBot.health) / 2;   // то распределяем энергию поровну
                health -= hlt;
                otherBot.health += hlt;
            }

            if (mineral > otherBot.mineral)
            {              // если у бота больше минералов, чем у соседа
                int min = (mineral - otherBot.mineral) / 2;   // то распределяем их поровну
                mineral -=  min;
                otherBot.mineral +=  min;
            }

            return checkResult;
        }

        public CheckResult Give()
        {
            int xt = CountX(Direction);
            int yt = CountY(Direction);

            var checkResult = World.Instance.Check(this, xt, yt);
            if (checkResult != CheckResult.RelativeBot)
            {
                return checkResult;
            }

            int giveHealth = health / 4;
            health -= giveHealth;

            var otherBot = World.Instance.Matrix[xt, yt];
            otherBot.health += giveHealth;

            var giveMineral = mineral / 4;
            if (giveMineral > 0)
            {
                mineral -= giveMineral;
                otherBot.mineral += giveMineral;
            }

            return checkResult;
        }
        
        public bool TryGeneAttack()
        {
            if (!TryLook(CheckResult.OtherBot))
                return false;

            int xt = CountX(Direction);
            int yt = CountY(Direction);

            var checkResult = World.Instance.Check(this, xt, yt);
            if (checkResult != CheckResult.OtherBot)
            {
                return false;
            }

            health -= 10; 

            if (health > 0)
            {
                var otherBot = World.Instance.Matrix[xt, yt];
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
            if (mineral < 100)
            {
                t = 0;
            }
            else
            {
                if (mineral < 400)
                {
                    t = 1;
                }
                else
                {
                    t = 2;
                }
            }
            int hlt = 10 - (15 * y / World.Instance.Height) + t; 
            if (hlt >= 3)
            {
                return true;
            }

            return false;
        }

        public void BotDivision()
        {
            var @group = Group();
            if (@group == Core.Group.Both || @group == Core.Group.Alone)
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
            if (mprev != null && mnext != null)
            {
                return false;
            }

            var newBot = CreateFreeChild();
            if (newBot == null)
                return false;

            if (mnext == null)
            {                   
                mnext = newBot; 
                newBot.mprev = this;  
            }
            else
            {                             
                mprev = newBot; 
                newBot.mnext = this; 
            }

            return true;
        }

        private Bot CreateFreeChild()
        {
            health -= 150;      // бот затрачивает 150 единиц энергии на создание копии
            if (health <= 0)
            {
                return null;
            }

            if (!TryFindDirection(CheckResult.Empty, out int emptyDirection))
            {
                // если бот окружен, то он в муках погибает
                health = 0;
                return null;
            }

            int xt = CountX(emptyDirection);
            int yt = CountY(emptyDirection);

            Bot newbot = new Bot(xt, yt);
            newbot.Consciousness.TransferFrom(Consciousness);

            if (Random.NextDouble() < 0.25)
            {
                newbot.Consciousness.Mutate();
            }

            health = health / 2;
            newbot.health = health;

            mineral = mineral / 2;
            newbot.mineral = mineral;

            newbot.Color.CopyFrom(Color);

            newbot.SetDirection((int)(Random.NextDouble() * 8));

            World.Instance.Matrix[xt, yt] = newbot;

            return newbot;
        }

        
        public bool TryConvertToOrganic()
        {
            if (health >= 1)
            {
                return false;
            }

            State = BotState.Organic;

            if (mprev != null)
            {
                mprev.mnext = null;
            } 

            if (mnext != null)
            {
                mnext.mprev = null;
            }

            mprev = null;
            mnext = null;

            return true;
        }

        public bool TryAccumulateMinerals()
        {
            // если бот находится на глубине ниже 48 уровня
            // то он автоматом накапливает минералы, но не более 999
            if (y <= World.Instance.Height / 2) return false;

            mineral++;

            if (y > World.Instance.Height / 6 * 4)
            {
                mineral++;
            }

            if (y > World.Instance.Height / 6 * 5)
            {
                mineral++;
            }

            if (mineral > 999) { mineral = 999; }

            return true;
        }

        public override string ToString()
        {
            return $"({x},{y}) {State} H:{health} M:{mineral}";
        }
    }
}
