using System.Collections.Generic;
using Newtonsoft.Json;

namespace CyberBiology.Core.Serialization
{
    public class ConsciousnessDto
    {
        public ConsciousnessDto()
        {
        }

        [JsonProperty("c")]
        public int CurrentActionIndex { get; set; }

        public ConsciousnessDto(Consciousness consciousness)
        {
            CurrentActionIndex = consciousness.CurrentActionIndex;

            Actions = new List<int>(Consciousness.Size);

            for (int i = 0; i < Consciousness.Size; i++)
            {
                Actions.Add((int)consciousness.Get(i).Action);
            }
        }

        [JsonProperty("a")]
        public List<int> Actions { get; set; }
    }
}