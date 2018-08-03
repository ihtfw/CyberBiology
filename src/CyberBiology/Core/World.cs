using System;
using CyberBiology.Core.Enums;

namespace CyberBiology.Core
{
    public class World
    {
        private BotСonsciousnessProcessor _botСonsciousnessProcessor = new BotСonsciousnessProcessor();

        public static World Instance;
        public readonly int Height;
        public readonly int Width;

        public readonly Bot[,] Matrix; //Матрица мира
        
        public World(int width, int height)
        {
            Width = width;
            Height = height;
            Matrix = new Bot[width, height];

            Instance = this;
        }


        public int Generation { get; private set; }

        public int Population { get; private set; }

        public int Organic { get; private set; }


        public void NextGeneration()
        {
            int population = 0;
            int organic = 0;

            for (var yw = 0; yw < Height; yw++)
            {
                for (var xw = 0; xw < Width; xw++)
                {
                    var bot = Matrix[xw, yw];
                    if (bot == null) continue;

                    _botСonsciousnessProcessor.Process(bot);

                    if (bot.IsAlive)
                    {
                        population++;
                    }else if (bot.IsOrganic)
                    {
                        organic++;
                    }
                }
            }

            Population = population;
            Organic = organic;
            Generation++;
        }
        
        public void CreateAdam()
        {
            var bot = new Bot
            {
                x = Width / 2,
                y = Height / 2,
                health = 990
            };

            bot.SetDirection(5);
            bot.Color.Adam();
            
            Matrix[bot.x, bot.y] = bot; 
        }

        public CheckResult Check(Bot checkForBot, int x, int y)
        {
            if (y < 0 || y >= Height)
            { 
                return CheckResult.Wall;                    
            }

            var bot = Matrix[x, y];
            if (bot == null)
            {
                return CheckResult.Empty;
            }

            if (bot.IsOrganic)
            { 
                return CheckResult.Organic;
            }

            if (bot.Consciousness.IsRelative(checkForBot.Consciousness))
            { 
                return CheckResult.RelativeBot;                     
            }

            return CheckResult.OtherBot;                         
        }

        public int LimitX(int x)
        {
            if (x > Width - 1)
                return 0;

            if (x < 0)
                return Width - 1;

            return x;
        }

        public int LimitY(int y)
        {
            if (y > Width - 1)
                return Width - 1;

            if (y < 0)
                return 0;

            return y;
        }

        public bool Delete(int x, int y)
        {
            return Delete(Matrix[x, y]);
        }

        public bool Delete(Bot bot)
        {
            if (bot == null)
                return false;

            Bot pbot = bot.mprev;
            Bot nbot = bot.mnext;

            if (pbot != null) { pbot.mnext = null; } // удаление бота из многоклеточной цепочки
            if (nbot != null) { nbot.mprev = null; }
            bot.mprev = null;
            bot.mnext = null;

            Matrix[bot.x, bot.y] = null; // удаление бота с карты

            return true;
        }
    }

    
}