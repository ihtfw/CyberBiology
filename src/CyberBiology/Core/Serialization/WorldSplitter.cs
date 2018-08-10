using System.Collections.Generic;
using System.Linq;

namespace CyberBiology.Core.Serialization
{
    public class WorldSplitter
    {
        public IEnumerable<WorldChunk> Split(World world)
        {
            var chunk = new WorldChunk
            {
                Bots = new List<BotDto>()
            };

            for (var yw = 0; yw < world.Height; yw++)
            {
                for (var xw = 0; xw < world.Width; xw++)
                {
                    var bot = world.Matrix[xw, yw];
                    if (bot == null)
                    {
                        continue;
                    }

                    if (chunk.Bots.Count > 100)
                    {
                        yield return chunk;

                        chunk = new WorldChunk
                        {
                            Number = chunk.Number + 1,
                            Bots = new List<BotDto>()
                        };
                    }
                    chunk.Bots.Add(new BotDto(bot));
                }
            }

            if (chunk.Bots.Any())
            {
                yield return chunk;
            }
        }
    }
}