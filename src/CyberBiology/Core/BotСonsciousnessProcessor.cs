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

            var group = bot.Group();
            // распределяем энергию  минералы по многоклеточному организму
            // возможны три варианта, бот находится внутри цепочки
            // бот имеет предыдущего бота в цепочке и не имеет следующего
            // бот имеет следующего бота в цепочке и не имеет предыдущего
            if (group == Group.Both)
            {                 // бот находится внутри цепочки
                Bot pb = bot.mprev; // ссылка на предыдущего бота в цепочке
                Bot nb = bot.mnext; // ссылка на следующего бота в цепочке
                                // делим минералы .................................................................
                int m = bot.mineral + nb.mineral + pb.mineral; // общая сумма минералов
                                                           //распределяем минералы между всеми тремя ботами
                m = m / 3;
                bot.mineral = m;
                nb.mineral = m;
                pb.mineral = m;

                // делим энергию ................................................................
                // проверим, являются ли следующий и предыдущий боты в цепочке крайними .........
                // если они не являются крайними, то распределяем энергию поровну       .........
                // связанно это с тем, что в крайних ботах в цепочке должно быть больше энергии ..
                // что бы они плодили новых ботов и удлиняли цепочку
                if ((pb.Group() == Group.Both) && (nb.Group() == Group.Both))
                { // если следующий и предыдущий боты не являются крайними
                  // то распределяем энергию поровну
                    int h = bot.health + nb.health + pb.health;
                    h = h / 3;
                    bot.health = h;
                    nb.health = h;
                    pb.health = h;
                }
            }else if (group == Group.HasPrev)
            {
                Bot pb = bot.mprev; // ссылка на предыдущего бота
                if (pb.Group() == Group.Both)
                {   // если нет, то распределяем энергию в пользу текущего бота
                    // так как он крайний и ему нужна энергия для роста цепочки
                    int h = bot.health + pb.health;
                    h = h / 4;
                    bot.health = h * 3;
                    pb.health = h;
                }
            }else if (group == Group.HasNext)
            {
                Bot nb = bot.mnext; // ссылка на следующего бота
                if (nb.Group() == Group.Both)
                {      // если нет, то распределяем энергию в пользу текущего бота
                       // так как он крайний и ему нужна энергия для роста цепочки
                    int h = bot.health + nb.health;
                    h = h / 4;
                    bot.health = h * 3;
                    nb.health = h;
                }
            }
            //... проверим уровень энергии у бота, возможно пришла пора помереть или родить
            if (bot.health > 999)
            {    
                bot.BotDivision();
            }           
            
            bot.health -= 3;   // каждый ход отнимает 3 единички здоровья(энегрии)
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
                        if (bot.Group() == Group.Alone)
                        {
                            bot.TryMove();
                        }
                        break;
                    case Actions.Eat:
                        bot.TryEat();
                        break;
                    case Actions.Share:
                        bot.Share();
                        break;
                    case Actions.Give:
                        bot.Give();
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
                        if (bot.y < World.Instance.Height / 2)
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

                if (action.IsStopAction())
                    break;
            }
        }
        
        private void CheckHealth(Bot bot, BotAction action)
        {
            int param = bot.Consciousness.Param(action);

            var magicHealth = param * Bot.MaxHealth / BotСonsciousness.Size;
            if (bot.health > magicHealth)
            {
                bot.Consciousness.SkipActions(1);
            }
        }

        private void CheckMinerals(Bot bot, BotAction action)
        {
            int param = bot.Consciousness.Param(action);

            var magicMinerals = param * Bot.MaxMinerals / BotСonsciousness.Size;
            if (bot.mineral > magicMinerals)
            {
                bot.Consciousness.SkipActions(1);
            }
        }
        
        private void SkipNextIfSurrounded(Bot bot, BotAction action)
        {
            if (bot.TryFindDirection(CheckResult.Empty, out _))
            {
                return;
            }

            bot.Consciousness.SkipActions(1);
        }
    }
}