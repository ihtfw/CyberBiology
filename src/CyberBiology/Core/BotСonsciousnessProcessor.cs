using CyberBiology.Core.Enums;

namespace CyberBiology.Core
{
    public class BotСonsciousnessProcessor
    {
        public void Process(Bot bot)
        {
            if (bot.IsDead)
                return;

            ActionsLoop(bot);

            if (bot.IsDead)
                return;

            // распределяем энергию  минералы по многоклеточному организму
            // возможны три варианта, бот находится внутри цепочки
            // бот имеет предыдущего бота в цепочке и не имеет следующего
            // бот имеет следующего бота в цепочке и не имеет предыдущего
            if (bot.Group == Group.Both)
            {                 // бот находится внутри цепочки
                Bot pb = bot.PrevBot; // ссылка на предыдущего бота в цепочке
                Bot nb = bot.NextBot; // ссылка на следующего бота в цепочке
                                // делим минералы .................................................................
                int m = bot.Mineral + nb.Mineral + pb.Mineral; // общая сумма минералов
                                                           //распределяем минералы между всеми тремя ботами
                m = m / 3;
                bot.Mineral = m;
                nb.Mineral = m;
                pb.Mineral = m;

                // делим энергию ................................................................
                // проверим, являются ли следующий и предыдущий боты в цепочке крайними .........
                // если они не являются крайними, то распределяем энергию поровну       .........
                // связанно это с тем, что в крайних ботах в цепочке должно быть больше энергии ..
                // что бы они плодили новых ботов и удлиняли цепочку
                if ((pb.Group == Group.Both) && (nb.Group == Group.Both))
                { // если следующий и предыдущий боты не являются крайними
                  // то распределяем энергию поровну
                    int h = bot.Health + nb.Health + pb.Health;
                    h = h / 3;
                    bot.Health = h;
                    nb.Health = h;
                    pb.Health = h;
                }
            }else if (bot.Group == Group.HasPrev)
            {
                Bot pb = bot.PrevBot; // ссылка на предыдущего бота
                if (pb.Group == Group.Both)
                {   // если нет, то распределяем энергию в пользу текущего бота
                    // так как он крайний и ему нужна энергия для роста цепочки
                    int h = bot.Health + pb.Health;
                    h = h / 4;
                    bot.Health = h * 3;
                    pb.Health = h;
                }
            }else if (bot.Group == Group.HasNext)
            {
                Bot nb = bot.NextBot; // ссылка на следующего бота
                if (nb.Group == Group.Both)
                {      // если нет, то распределяем энергию в пользу текущего бота
                       // так как он крайний и ему нужна энергия для роста цепочки
                    int h = bot.Health + nb.Health;
                    h = h / 4;
                    bot.Health = h * 3;
                    nb.Health = h;
                }
            }
            //... проверим уровень энергии у бота, возможно пришла пора помереть или родить
            if (bot.Health > 999)
            {    
                bot.BotDivision();
            }           
            
            bot.Health -= 3;   // каждый ход отнимает 3 единички здоровья(энегрии)
            if (bot.TryConvertToOrganic())
                return;

            bot.TryAccumulateMinerals();
        }

        private void ActionsLoop(Bot bot)
        {
            for (int i = 0; i < 15; i++)
            {
                var action = bot.Consciousness.NextAction();
                if (!action.IsValid())
                    continue;

                switch (action.Action)
                {
                    case Actions.Skip:
                        var param = bot.Consciousness.Param(action);
                        bot.Consciousness.SkipActions(param);
                        break;
                    case Actions.Photosynthesis:
                        bot.TryPhotosynthesis();
                        break;
                    case Actions.Move:
                        if (bot.Group == Group.Alone)
                        {
                            bot.TryMove();
                        }
                        break;
                    case Actions.EatOtherBot:
                        bot.TryEatOtherBot();
                        break;
                    case Actions.EatOrganic:
                        bot.TryEatOrganic();
                        break;
                    case Actions.Share:
                        bot.TryShare();
                        break;
                    case Actions.Give:
                        bot.TryGive();
                        break;
                    case Actions.CheckHealth:
                        CheckHealth(bot, action);
                        break;
                    case Actions.CheckMinerals:
                        CheckMinerals(bot, action);
                        break;
                    case Actions.BotDivision:
                        bot.BotDivision();
                        break;
                    case Actions.SkipNextIfSurrounded:
                        SkipNextIfSurrounded(bot, action);
                        break;
                    case Actions.HasEnergyIncome:
                        if (!bot.IsHealthGrow())
                        {
                            bot.Consciousness.SkipActions(1);
                        }
                        break;
                    case Actions.AreMineralGrow:
                        if (bot.Y < World.Instance.Height / 2)
                        {
                            bot.Consciousness.SkipActions(1);
                        }

                        break;
                    case Actions.ConvertMineralToEnergy:
                        bot.ConvertMineralToEnergy();
                        break;
                    case Actions.Mutate:
                        bot.Consciousness.Mutate();
                        bot.Consciousness.Mutate();
                        break;
                    case Actions.GeneAttack:
                        bot.TryGeneAttack();
                        break;
                }

                if (action.IsStopAction)
                    break;
            }
        }
        
        private void CheckHealth(Bot bot, BotAction action)
        {
            int param = bot.Consciousness.Param(action);

            var magicHealth = param * Bot.MaxHealth / BotСonsciousness.Size;
            if (bot.Health > magicHealth)
            {
                bot.Consciousness.SkipActions(1);
            }
        }

        private void CheckMinerals(Bot bot, BotAction action)
        {
            int param = bot.Consciousness.Param(action);

            var magicMinerals = param * Bot.MaxMinerals / BotСonsciousness.Size;
            if (bot.Mineral > magicMinerals)
            {
                bot.Consciousness.SkipActions(1);
            }
        }
        
        private void SkipNextIfSurrounded(Bot bot, BotAction action)
        {
            if (bot.TryLook(CheckResult.Empty))
            {
                return;
            }

            bot.Consciousness.SkipActions(1);
        }
    }
}