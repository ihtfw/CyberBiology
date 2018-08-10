using System.Collections.Generic;
using Newtonsoft.Json;

namespace CyberBiology.Core.Serialization
{
    public class WorldChunk
    {
        [JsonProperty("n")]
        public int Number { get; set; }

        [JsonProperty("b")]
        public List<BotDto> Bots { get; set; }
    }
}