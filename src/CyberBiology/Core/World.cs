using System;
using System.Threading;
using System.Threading.Tasks;
using CyberBiology.Core.Enums;

namespace CyberBiology.Core
{
    public class World
    {
        public const int BlockSize = 20;

        private readonly BotСonsciousnessProcessor _botСonsciousnessProcessor = new BotСonsciousnessProcessor();

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

            BlocksX = (int)Math.Ceiling(Width / (double)BlockSize);
            BlocksY = (int)Math.Ceiling(Height / (double)BlockSize);
        }

        public int BlocksX { get; }

        public int BlocksY { get; }

        public int Generation { get; private set; }

        public int Population { get; private set; }

        public int Organic { get; private set; }

        public void NextGenerationInParallel()
        {
            int totalPopulation = 0;
            int totalOrganic = 0;

            //Block are left to right, up to down
            Parallel.For(0, BlocksX * BlocksY, i =>
            {
                int population = 0;
                int organic = 0;

                var x = i % BlocksX * BlockSize;
                var y = i / BlocksX * BlockSize;

                for (var yw = y; yw < y + BlockSize; yw++)
                {
                    for (var xw = x; xw < x + BlockSize; xw++)
                    {
                        if (xw >= Width)
                            continue;

                        if (yw >= Height)
                            continue;

                        var bot = Matrix[xw, yw];
                        if (bot == null) continue;

                        _botСonsciousnessProcessor.Process(bot);

                        if (bot.IsAlive)
                        {
                            population++;
                        }
                        else if (bot.IsOrganic)
                        {
                            organic++;
                        }
                    }
                }

                Interlocked.Add(ref totalPopulation, population);
                Interlocked.Add(ref totalOrganic, organic);
            });
            
            Population = totalPopulation;
            Organic = totalOrganic;
            Generation++;
        }

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
            var bot = BotFactory.Get(Width / 2, Height / 2);
            bot.Health = 990;

            bot.SetDirection(5);
            bot.Color.Adam();
            
            Matrix[bot.X, bot.Y] = bot; 
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

            Bot pbot = bot.PrevBot;
            Bot nbot = bot.NextBot;

            if (pbot != null) { pbot.NextBot = null; } // удаление бота из многоклеточной цепочки
            if (nbot != null) { nbot.PrevBot = null; }
            bot.PrevBot = null;
            bot.NextBot = null;

            Matrix[bot.X, bot.Y] = null; // удаление бота с карты

            BotFactory.ToCache(bot);

            return true;
        }
    }

    
}