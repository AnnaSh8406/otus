using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.Core.Entities;
using otus_dz2_v2.Core.Services;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Threading;

namespace otus_dz2_v2.Scenarios
{
    internal class AddListScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IToDoListService _toDoListService;
        public AddListScenario(IUserService userService, IToDoListService toDoListService)
        {
            _userService = userService;
            _toDoListService = toDoListService;
        }
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.AddList;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken cancellationToken)
        {
            ToDoUser? toDoUser = null;
            string? toDoItemName = null;
            switch (context.CurrentStep)
            {
                case null:
                    toDoUser = await _userService.GetUserAsync(context.UserId, cancellationToken);
                    context.CurrentStep = "Name";
                    context.Data.Add("User", toDoUser);
                    await bot.SendMessage(update.CallbackQuery.Message.Chat.Id, "Введите название списка:", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardCancel());
                    return ScenarioResult.Transition;
                case "Name":
                    toDoUser = (ToDoUser?)context.Data.GetValueOrDefault("User");
                    await _toDoListService.Add(toDoUser, update.Message.Text, cancellationToken);
                    await bot.SendMessage(update.Message.Chat.Id, "Список добавлен", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardButtons(true));
                    return ScenarioResult.Completed;
                default:
                    throw new NotSupportedException($"Нет шага {context.CurrentStep}");
            }
        }
    }
}
