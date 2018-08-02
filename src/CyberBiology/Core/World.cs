using CyberBiology.Core.Enums;

namespace CyberBiology.Core
{
    public class World
    {
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

                    bot.Step(); //Выполняем ход бота

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
                adr = 0,
                x = Width / 2,
                y = Height / 2,
                health = 990,
                mineral = 0,
                direction = 5,
                mprev = null,
                mnext = null
            };

            bot.Color.Adam();

            for (var i = 0; i < 64; i++)
            {
                bot.mind[i] = Gene.Photosynthesis;
            }

            Matrix[bot.x, bot.y] = bot; 
        }
    }
}