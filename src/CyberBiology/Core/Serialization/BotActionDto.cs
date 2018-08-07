namespace CyberBiology.Core.Serialization
{
    public class BotActionDto
    {
        public BotActionDto()
        {
        }

        public BotActionDto(BotAction botAction)
        {
            Index = botAction.Index;
            Action = (int)botAction.Action;
        }

        public int Index { get; set; }

        public int Action { get; set; }
    }
}