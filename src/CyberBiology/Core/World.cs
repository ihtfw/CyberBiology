using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CyberBiology.Core.Enums;
using CyberBiology.Core.Serialization;

namespace CyberBiology.Core
{
    public class World
    {
        public const int BlockSize = 20;

        private readonly СonsciousnessProcessor _сonsciousnessProcessor = new СonsciousnessProcessor();

        public static World Instance;
        public int Height { get; private set; }
        public int Width { get; private set; }

        public readonly Bot[,] Matrix; //Матрица мира

        public void LoadWorld(WorldInfo worldInfo, IEnumerable<WorldChunk> worldChunks)
        {
            Clear();   

            Width = worldInfo.Width;
            Height = worldInfo.Height;
            Iteration = worldInfo.Iteration;

            foreach (var worldChunk in worldChunks)
            {
                foreach (var botDto in worldChunk.Bots)
                {
                    var bot = BotFactory.Get(botDto.X, botDto.Y);
                    bot.Load(botDto);

                    Matrix[bot.X, bot.Y] = bot;
                }
            }
        }

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

        public int Iteration { get; private set; }

        public int Population { get; private set; }

        public int Organic { get; private set; }
        public int Empty { get; private set; }

        public void NextIterationInParallel()
        {
            int totalPopulation = 0;
            int totalOrganic = 0;
            int totalEmpty = 0;

            //Block are left to right, up to down
            Parallel.For(0, BlocksX * BlocksY, i =>
            {
                int population = 0;
                int organic = 0;
                int empty = 0;

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
                        if (bot == null)
                        {
                            empty++;
                            continue;
                        }

                        if (bot.IsAlive)
                        {
                            _сonsciousnessProcessor.Process(bot);
                        }

                        if (bot.IsOrganic)
                        {
                            organic++;
                        }else if (bot.IsAlive)
                        {
                            population++;
                        }
                    }
                }

                Interlocked.Add(ref totalPopulation, population);
                Interlocked.Add(ref totalOrganic, organic);
                Interlocked.Add(ref totalEmpty, empty);
            });
            
            Population = totalPopulation;
            Organic = totalOrganic;
            Empty = totalEmpty;
            Iteration++;
        }

        public void NextIteration()
        {
            int population = 0;
            int organic = 0;
            int empty = 0;

            for (var yw = 0; yw < Height; yw++)
            {
                for (var xw = 0; xw < Width; xw++)
                {
                    var bot = Matrix[xw, yw];
                    if (bot == null)
                    {
                        empty++;
                        continue;
                    }

                    if (bot.IsAlive)
                    {
                        _сonsciousnessProcessor.Process(bot);
                    }

                    if (bot.IsOrganic)
                    {
                        organic++;
                    }
                    else if (bot.IsAlive)
                    {
                        population++;
                    }
                }
            }

            Population = population;
            Organic = organic;
            Empty = empty;

            Iteration++;
        }
        
        public void CreateAdam()
        {
            var bot = BotFactory.Get(Width / 2, Height / 2);
            bot.Health = Bot.MaxHealth + 1;

            bot.Direction = Direction.Random();
            bot.Color.Reset();
            
            Matrix[bot.X, bot.Y] = bot; 
        }

        public void CountAroundFor(Bot checkForBot, out int relative, out int other, out int empty)
        {
            relative = 0;
            other = 0;
            empty = 0;

            foreach (var direction in Direction.All)
            {
                var xt = checkForBot.X + direction.Dx;
                var yt = checkForBot.Y + direction.Dy;

                if (yt < 0 || yt >= Height)
                {
                    continue;
                }

                xt = LimitX(xt);

                var directionBot = Matrix[xt, yt];
                if (directionBot == null)
                {
                    empty++;
                    continue;
                }

                if (!directionBot.IsAlive)
                {
                    continue;
                }

                if (directionBot.Consciousness.IsRelative(checkForBot.Consciousness))
                {
                    relative++;
                }
                else
                {
                    other++;
                }
            }
        }

        public bool CheckDirectionFor(Bot checkForBot, CheckResult lookFor, Direction direction, out Bot directionBot)
        {
            var xt = checkForBot.X + direction.Dx;
            var yt = checkForBot.Y + direction.Dy;

            directionBot = null;

            if (yt < 0 || yt >= Height)
            {
                return lookFor == CheckResult.Wall;
            }

            xt = LimitX(xt);

            directionBot = Matrix[xt, yt];
            if (directionBot == null)
            {
                return lookFor == CheckResult.Empty;
            }

            if (directionBot.IsOrganic)
            {
                return lookFor == CheckResult.Organic;
            }

            if (lookFor == CheckResult.AnyBot)
            {
                return true;
            }

            if (directionBot.Consciousness.IsRelative(checkForBot.Consciousness))
            {
                return lookFor == CheckResult.RelativeBot;
            }

            if (lookFor == CheckResult.OtherBot)
            {
                return lookFor == CheckResult.OtherBot;
            }

            return false;
        }
        
        public bool TryFindDirection(Bot checkForBot, CheckResult lookFor, out Direction foundDirection, out Bot directionBot)
        {
            foreach (var direction in Direction.From(checkForBot.Direction))
            {
                if (CheckDirectionFor(checkForBot, lookFor, direction, out directionBot))
                {
                    foundDirection = direction;
                    return true;
                }
            }

            foundDirection = null;
            directionBot = null;

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LimitX(int x)
        {
            if (x > Width - 1)
                return 0;

            if (x < 0)
                return Width - 1;

            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LimitY(int y)
        {
            if (y > Width - 1)
                return Width - 1;

            if (y < 0)
                return 0;

            return y;
        }

        public bool Delete(Bot bot)
        {
            if (bot == null)
                return false;
            
            Matrix[bot.X, bot.Y] = null; // удаление бота с карты

            BotFactory.ToCache(bot);

            return true;
        }

        public void Move(Bot bot, Direction direction)
        {
            int xt = LimitX(bot.X + direction.Dx);
            int yt = LimitY(bot.Y + direction.Dy);

            Matrix[xt, yt] = bot;
            Matrix[bot.X, bot.Y] = null;

            bot.X = xt;
            bot.Y = yt;
        }

        public void AddRandomBot()
        {
            var x = Utils.Next(Width);
            var y = Utils.Next(Height);

            var bot = Matrix[x, y];
            if (bot == null)
            {
                bot = BotFactory.Get(x, y);
                Matrix[bot.X, bot.Y] = bot;
            }

            bot.Reset();
            bot.Health = Bot.MaxHealth + 1;

            for (int i = 0; i < Consciousness.Size; i++)
            {
                bot.Consciousness.Mutate();
            }
        }

        public void RandomMutations()
        {
            for (var yw = 0; yw < Height; yw++)
            {
                for (var xw = 0; xw < Width; xw++)
                {
                    var bot = Matrix[xw, yw];
                    if (bot == null)
                    {
                        continue;
                    }

                    if (bot.IsAlive)
                    {
                        for (int i = 0; i < Consciousness.Size / 10; i++)
                        {
                            bot.Consciousness.Mutate();
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            for (var yw = 0; yw < Height; yw++)
            {
                for (var xw = 0; xw < Width; xw++)
                {
                    var bot = Matrix[xw, yw];
                    if (bot == null)
                    {
                        continue;
                    }

                    Delete(bot);
                }
            }
        }
    }
}