using System.Collections.Generic;

namespace CyberBiology.Core.Serialization
{
    public class ConsciousnessDto
    {
        public ConsciousnessDto()
        {
        }

        public ConsciousnessDto(Consciousness consciousness)
        {
            Actions = new List<BotActionDto>(Consciousness.Size);

            for (int i = 0; i < Consciousness.Size; i++)
            {
                Actions.Add(new BotActionDto(consciousness.Get(i)));
            }
        }

        public List<BotActionDto> Actions { get; set; }
    }
}