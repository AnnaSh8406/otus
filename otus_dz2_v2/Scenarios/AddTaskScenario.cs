using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.Core.Entities;
using otus_dz2_v2.Core.Services;
using Telegram.Bot.Types;
using Telegram.Bot;
using otus_dz2_v2.Core.Exceptions;
using otus_dz2_v2.Dto;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;


namespace otus_dz2_v2.Scenarios
{
    internal class AddTaskScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        private readonly IToDoListService _toDoListService;
        public AddTaskScenario(IUserService userService, IToDoService toDoService, IToDoListService toDoListService)
        {
            _userService = userService;
            _toDoService = toDoService;
            _toDoListService = toDoListService;
        }
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.AddTask;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken cancellationToken)
        {
            ToDoUser? toDoUser = null;
            string? toDoItemName = null;
            DateTime toDoDate;
            switch (context.CurrentStep)
            {
                case null:
                    toDoUser = await _userService.GetUserAsync(context.UserId, cancellationToken);
                    context.CurrentStep = "Name";
                    context.Data.Add("User", toDoUser);
                    await bot.SendMessage(update.Message.Chat, "Введите название задачи:", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardCancel());
                    return ScenarioResult.Transition;
                case "Name":
                    context.Data.Add(context.CurrentStep, update.Message.Text);
                    context.CurrentStep = "Date";
                    await bot.SendMessage(update.Message.Chat, "Введите срок выполнения:", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardCancel());
                    return ScenarioResult.Transition;
                case "Date":
                    toDoUser = (ToDoUser?)context.Data.GetValueOrDefault("User");
                    toDoItemName = (string)context.Data.GetValueOrDefault("Name");
                    if (DateTime.TryParse(update.Message.Text, out toDoDate))
                    {
                        context.Data.Add(context.CurrentStep, toDoDate);
                        context.CurrentStep = "List";

                        var toDoLists = await _toDoListService.GetUserLists(toDoUser.UserId, cancellationToken);
                        var listButtons = new List<InlineKeyboardButton>();

                        listButtons.Add(InlineKeyboardButton.WithCallbackData("📌Без списка", $"selectlist|null"));
                        foreach (var list in toDoLists)
                        {
                            listButtons.Add(InlineKeyboardButton.WithCallbackData(list.Name, $"selectlist|{list.Id}"));
                        }

                        var replyKeyboardMarkup = new InlineKeyboardMarkup(new[]
                        {
                        listButtons.ToArray()
                    });

                        await bot.SendMessage(update.Message.Chat.Id, "Выберите список:", cancellationToken: cancellationToken, replyMarkup: replyKeyboardMarkup);
                    }
                    else
                    {
                        await bot.SendMessage(update.Message.Chat, "Ошибка в дате. Введите срок выполнения еще раз:", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardCancel());
                    }
                    return ScenarioResult.Transition;
                case "List":
                    toDoUser = (ToDoUser?)context.Data.GetValueOrDefault("User");
                    toDoItemName = (string)context.Data.GetValueOrDefault("Name");
                    toDoDate = (DateTime)context.Data.GetValueOrDefault("Date");

                    var callback = ToDoListCallbackDto.FromString(update.CallbackQuery.Data);
                    if (callback.ToDoListId == null)
                    {
                        await _toDoService.AddAsync(toDoUser, toDoItemName, toDoDate, null, cancellationToken);
                    }
                    else
                    {
                        var toDoList = await _toDoListService.Get(callback.ToDoListId.GetValueOrDefault(), cancellationToken);
                        await _toDoService.AddAsync(toDoUser, toDoItemName, toDoDate, toDoList, cancellationToken);
                    }


                    await bot.SendMessage(update.CallbackQuery.Message.Chat, "Задача добавлена", cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardButtons(true));
                    return ScenarioResult.Completed;
                default:
                    throw new NotSupportedException($"Нет шага {context.CurrentStep}");
            }


        }
    }

}
