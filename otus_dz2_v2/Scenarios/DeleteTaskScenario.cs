using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.Core.Entities;
using otus_dz2_v2.Core.Services;
using otus_dz2_v2.Dto;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Threading;

namespace otus_dz2_v2.Scenarios
{
    internal class DeleteTaskScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        public DeleteTaskScenario(IUserService userService, IToDoService toDoService)
        {
            _userService = userService;
            _toDoService = toDoService;
        }
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.DeleteTask;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken cancellationToken)
        {
            ToDoUser? toDoUser = null;
            string? toDoItemName = null;
            InlineKeyboardMarkup replyKeyboardMarkup;
            ToDoItemCallbackDto? toDoItemCallback = null;
            CallbackDto? callback = null;
            switch (context.CurrentStep)
            {
                case null:
                    toDoUser = await _userService.GetUserAsync(context.UserId, cancellationToken);
                    context.Data.Add("User", toDoUser);

                    toDoItemCallback = ToDoItemCallbackDto.FromString(update.CallbackQuery.Data);
                    var toDoItem = await _toDoService.Get(toDoItemCallback.ToDoItemId.GetValueOrDefault(), cancellationToken);

                    context.CurrentStep = "Delete";
                    context.Data.Add("toDoItem", toDoItem);

                    replyKeyboardMarkup = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("✅Да", "yes"),
                            InlineKeyboardButton.WithCallbackData("❌Нет", "no")
                        }
                    });

                    await bot.SendMessage(update.CallbackQuery.Message.Chat.Id, $"Подтверждаете удаление задачи {toDoItem.Name}?", cancellationToken: cancellationToken, replyMarkup: replyKeyboardMarkup);
                    return ScenarioResult.Transition;
                case "Delete":
                    callback = CallbackDto.FromString(update.CallbackQuery.Data);
                    if (callback.Action.Equals("yes"))
                    {
                        toDoUser = (ToDoUser?)context.Data.GetValueOrDefault("User");
                        toDoItem = (ToDoItem?)context.Data.GetValueOrDefault("toDoItem");
                        await _toDoService.DeleteAsync(toDoItem.Id, cancellationToken);

                        await bot.SendMessage(update.CallbackQuery.Message.Chat.Id, "Задача удалена", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardButtons(true));
                    }
                    else if (callback.Action.Equals("no"))
                    {
                        await bot.SendMessage(update.CallbackQuery.Message.Chat.Id, "Удаление отменено", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardButtons(true));
                    }
                    return ScenarioResult.Completed;
                default:
                    throw new NotSupportedException($"Нет шага {context.CurrentStep}");
            }
        }
    }
}
