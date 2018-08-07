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
            Health = bot.Health;
            Mineral = bot.Mineral;
            Color = bot.Color;
            DirectionIndex = bot.Direction.Index;

            Consciousness = new ConsciousnessDto(bot.Consciousness);
        }

        public int X { get; set; }

        public int Y { get; set; }

        public float Health { get; set; }

        public float Mineral { get; set; }

        public Color Color { get; set; } 
        
        public int DirectionIndex { get; set; }
        
        public ConsciousnessDto Consciousness { get; set; }
    }
}