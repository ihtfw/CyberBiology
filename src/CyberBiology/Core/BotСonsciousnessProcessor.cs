using System;
using CyberBiology.Core.Enums;

namespace CyberBiology.Core
{
    public class BotСonsciousnessProcessor
    {
        public void Process(Bot bot)
        {
            bot.Health -= 0.01f;

            ActionsLoop(bot);

            if (bot.IsDead)
                return;
            
            if (bot.Health > Bot.MaxHealth)
            {
                bot.BotDivision();
            }
        }

        private void ActionsLoop(Bot bot)
        {
            /*
            BotAction action = null;
            
            for (int i = 0; i < 15; i++)
            {
                var nextAction = bot.Consciousness.NextAction();
                if (!nextAction.IsValid())
                    continue;

                action = nextAction;
                break;
            }

            if (action == null)
                return;
                */
            TryNextAction(bot, out var _);
            /*
            for (int i = 0; i < 10; i++)
            {
                if (TryNextAction(bot, out var botAction))
                {
                    if (botAction.IsStopAction)
                        break;
                }
            }*/
        }

        private bool TryNextAction(Bot bot, out BotAction action)
        {
            action = bot.Consciousness.NextAction();
            if (!action.IsValid())
                return false;

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

            return true;
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