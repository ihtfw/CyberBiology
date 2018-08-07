using CyberBiology.Core.Enums;

namespace CyberBiology.Core
{
    public class СonsciousnessProcessor
    {
        public void Process(Bot bot)
        {
            bot.Health -= 0.01f;

            NextAction(bot);

            if (bot.IsDead)
                return;
            
            if (bot.Health > Bot.MaxHealth)
            {
                bot.BotDivision();
            }
        }

        private void NextAction(Bot bot)
        {
            var action = bot.Consciousness.NextAction();
            if (!action.IsValid())
                return;

            switch (action.Action)
            {
                case Actions.Rotate:
                    bot.Direction = Direction.Offset(bot.Direction, bot.Consciousness.Param(action));
                    break;
                case Actions.Look:
                    bot.TryLook(ToLookCheckResult(bot.Consciousness.Param(action)));
                    break;
                case Actions.Skip:
                    var param = bot.Consciousness.Param(action);
                    bot.Consciousness.SkipActions(param);
                    break;
                case Actions.Photosynthesis:
                    bot.Photosynthesis();
                    break;
                case Actions.AccumulateMinerals:
                    bot.AccumulateMinerals();
                    break;
                case Actions.Move:
                    bot.TryMove();
                    break;
                case Actions.Eat:
                    bot.TryEatBot(ToEatCheckResult(bot.Consciousness.Param(action)));
                    break;
                case Actions.Share:
                    bot.TryShare();
                    break;
                case Actions.Give:
                    bot.TryGive();
                    break;
                case Actions.BotDivision:
                    bot.BotDivision();
                    break;
                case Actions.SkipNextIfSurrounded:
                    SkipNextIfSurrounded(bot, action);
                    break;
                case Actions.ConvertMineralToHealth:
                    bot.ConvertMineralToHealth();
                    break;
                case Actions.Mutate:
                    bot.Consciousness.Mutate();
                    break;
                case Actions.GeneAttack:
                    bot.TryGeneAttack();
                    break;
            }
        }

        private CheckResult ToLookCheckResult(int param)
        {
            switch (param % 4)
            {
                case 0:
                    return CheckResult.Empty;
                case 1:
                    return CheckResult.Organic;
                case 2:
                    return CheckResult.OtherBot;
                default:
                    return CheckResult.RelativeBot;
            }
        }
        private CheckResult ToEatCheckResult(int param)
        {
            switch (param % 4)
            {
                case 0:
                    return CheckResult.Organic;
                case 1:
                    return CheckResult.RelativeBot;
                case 2:
                    return CheckResult.OtherBot;
                default:
                    return CheckResult.AnyBot;
            }
        }

        private void SkipNextIfSurrounded(Bot bot, BotAction action)
        {
            if (bot.IsSurrounded())
            {
                return;
            }

            bot.Consciousness.SkipActions(1);
        }
    }
}