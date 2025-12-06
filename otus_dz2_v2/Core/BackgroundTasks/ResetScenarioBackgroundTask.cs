using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.Scenarios;
using Telegram.Bot;

namespace otus_dz2_v2.Core.BackgroundTasks
{
    public class ResetScenarioBackgroundTask : BackgroundTask
    {
        private readonly TimeSpan _resetScenarioTimeout;
        private readonly IScenarioContextRepository _scenarioRepository;
        private readonly ITelegramBotClient _bot;
        public ResetScenarioBackgroundTask(TimeSpan resetScenarioTimeout, IScenarioContextRepository scenarioRepository, ITelegramBotClient bot) : base(TimeSpan.FromHours(1), nameof(ResetScenarioBackgroundTask))
        {
            _resetScenarioTimeout = resetScenarioTimeout;
            _scenarioRepository = scenarioRepository;
            _bot = bot;
        }

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            var scenarioContexts = await _scenarioRepository.GetContexts(cancellationToken);
            foreach (var scenarioContext in scenarioContexts)
            {
                if ((DateTime.Now - scenarioContext.CreatedAt) > _resetScenarioTimeout)
                {
                    await _bot.SendMessage(scenarioContext.ChatId, $"Сценарий отменен, так как не поступил ответ в течение {_resetScenarioTimeout}", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardButtons(true));
                    await _scenarioRepository.ResetContext(scenarioContext.UserId, cancellationToken);
                }
            }
        }
    }
}
