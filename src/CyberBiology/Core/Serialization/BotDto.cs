using Newtonsoft.Json;

namespace CyberBiology.Core.Serialization
{
    public class BotDto
    {
        public BotDto()
        {
        }

        public BotDto(Bot bot)
        {
            X = bot.X;
            Y = bot.Y;

            if (bot.IsAlive)
            {
                Health = bot.Health;
                Mineral = bot.Mineral;
                DirectionIndex = bot.Direction.Index;
                Color = bot.Color;
                Consciousness = new ConsciousnessDto(bot.Consciousness);
            }
            else
            {
                Health = -1;
            }
        }

        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("h")]
        public float Health { get; set; }

        [JsonProperty("m")]
        public float Mineral { get; set; }

        [JsonProperty("c")]
        public Color Color { get; set; }

        [JsonProperty("d")]
        public int DirectionIndex { get; set; }

        [JsonProperty("con")]
        public ConsciousnessDto Consciousness { get; set; }
    }
}