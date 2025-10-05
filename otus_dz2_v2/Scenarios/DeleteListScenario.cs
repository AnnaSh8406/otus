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
    internal class DeleteListScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IToDoListService _toDoListService;
        private readonly IToDoService _toDoService;
        public DeleteListScenario(IUserService userService, IToDoListService toDoListService, IToDoService toDoService)
        {
            _userService = userService;
            _toDoListService = toDoListService;
            _toDoService = toDoService;
        }
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.DeleteList;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken cancellationToken)
        {
            ToDoUser? toDoUser = null;
            string? toDoItemName = null;
            InlineKeyboardMarkup replyKeyboardMarkup;
            ToDoListCallbackDto? toDoListCallback = null;
            CallbackDto? callback = null;
            switch (context.CurrentStep)
            {
                case null:
                    toDoUser = await _userService.GetUserAsync(context.UserId, cancellationToken);
                    context.CurrentStep = "Approve";
                    context.Data.Add("User", toDoUser);

                    var toDoLists = await _toDoListService.GetUserLists(toDoUser.UserId, cancellationToken);
                    if (toDoLists.Count == 0)
                    {
                        await bot.SendMessage(update.CallbackQuery.Message.Chat.Id, "Нет списков для удаления", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardButtons(true));
                        return ScenarioResult.Completed;
                    }
                    var listButtons = new List<InlineKeyboardButton>();
                    foreach (var list in toDoLists)
                    {
                        listButtons.Add(InlineKeyboardButton.WithCallbackData(list.Name, $"deletelist|{list.Id}"));
                    }

                    replyKeyboardMarkup = new InlineKeyboardMarkup(new[]
                    {
                        listButtons.ToArray()
                    });

                    await bot.SendMessage(update.CallbackQuery.Message.Chat.Id, "Выберите список:", cancellationToken: cancellationToken, replyMarkup: replyKeyboardMarkup);
                    return ScenarioResult.Transition;
                case "Approve":
                    toDoUser = (ToDoUser?)context.Data.GetValueOrDefault("User");

                    toDoListCallback = ToDoListCallbackDto.FromString(update.CallbackQuery.Data);
                    var toDoList = await _toDoListService.Get(toDoListCallback.ToDoListId.GetValueOrDefault(), cancellationToken);

                    context.CurrentStep = "Delete";
                    context.Data.Add("toDoList", toDoList);

                    replyKeyboardMarkup = new InlineKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            InlineKeyboardButton.WithCallbackData("✅Да", "yes"),
                            InlineKeyboardButton.WithCallbackData("❌Нет", "no")
                        }
                    });

                    await bot.SendMessage(update.CallbackQuery.Message.Chat.Id, $"Подтверждаете удаление списка {toDoList.Name} и всех его задач?", cancellationToken: cancellationToken, replyMarkup: replyKeyboardMarkup);
                    return ScenarioResult.Transition;
                case "Delete":
                    callback = CallbackDto.FromString(update.CallbackQuery.Data);
                    if (callback.Action.Equals("yes"))
                    {
                        toDoUser = (ToDoUser?)context.Data.GetValueOrDefault("User");
                        toDoList = (ToDoList?)context.Data.GetValueOrDefault("toDoList");

                        var userTasks = await _toDoService.GetByUserIdAndListAsync(toDoUser.UserId, toDoList.Id, cancellationToken);
                        foreach (var task in userTasks)
                        {
                            await _toDoService.DeleteAsync(task.Id, cancellationToken);
                        }

                        await _toDoListService.Delete(toDoList.Id, cancellationToken);
                        await bot.SendMessage(update.CallbackQuery.Message.Chat.Id, "Список удален", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardButtons(true));
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
