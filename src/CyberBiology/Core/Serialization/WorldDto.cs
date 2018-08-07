using System.Collections.Generic;

namespace CyberBiology.Core.Serialization
{
    public class WorldDto
    {
        public WorldDto()
        {
        }

        public WorldDto(World world)
        {
            Height = world.Height;
            Width = world.Width;
            Iteration = world.Iteration;
            Bots = new List<BotDto>();

            for (var yw = 0; yw < Height; yw++)
            {
                for (var xw = 0; xw < Width; xw++)
                {
                    var bot = world.Matrix[xw, yw];
                    if (bot == null)
                    {
                        continue;
                    }

                    Bots.Add(new BotDto(bot));
                }
            }
        }

        public int Height { get; set; }
        public int Width { get; set; }
        public int Iteration { get; set; }

        public List<BotDto> Bots { get; set; }
    }
}