using Newtonsoft.Json;

namespace CyberBiology.Core.Serialization
{
    public class WorldInfo
    {
        public WorldInfo()
        {
        }

        public WorldInfo(World world)
        {
            Height = world.Height;
            Width = world.Width;
            Iteration = world.Iteration;
        }

        [JsonProperty("h")]
        public int Height { get; set; }

        [JsonProperty("w")]
        public int Width { get; set; }

        [JsonProperty("i")]
        public int Iteration { get; set; }

        [JsonProperty("cn")]
        public int ChunksNumber { get; set; }
    }
}