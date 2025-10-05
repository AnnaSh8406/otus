using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace otus_dz2_v2.Scenarios
{
    public interface IScenario
    {
        bool CanHandle(ScenarioType scenario);
        Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken cancellationToken);
    }
}
