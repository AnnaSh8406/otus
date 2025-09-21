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

namespace otus_dz2_v2.Scenarios
{
    public class AddTaskScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;

        public AddTaskScenario(IUserService userService, IToDoService toDoService)
        {
            _userService = userService;
            _toDoService = toDoService;
        }

        public bool CanHandle(ScenarioType scenario) => scenario == ScenarioType.AddTask;

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message!.Text!;
            switch (context.CurrentStep)
            {
                case null:
                    var currentUser = await _userService.GetUserAsync(update.Message.From.Id, cancellationToken);
                    if (currentUser == null)
                    {
                        await bot.SendMessage(update.Message.Chat.Id, "Сначала зарегистрируйся.", cancellationToken: cancellationToken);
                        return ScenarioResult.Completed;
                    }

                    context.Data["CurrentUser"] = currentUser;

                    //ввод задачи
                    await bot.SendMessage(update.Message.Chat.Id, "Введите название задачи:", cancellationToken: cancellationToken);
                    context.CurrentStep = "Name";
                    return ScenarioResult.Transition;

                case "Name":
                    //задачи
                    var taskName = message;
                    context.Data["TaskName"] = taskName;

                    // дедлайн
                    await bot.SendMessage(update.Message.Chat.Id, "Введите дедлайн задачи в формате dd.MM.yyyy:", cancellationToken: cancellationToken);
                    context.CurrentStep = "Deadline";
                    return ScenarioResult.Transition;

                case "Deadline":
                    if (!DateTime.TryParseExact(message, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var deadline))
                    {
                        await bot.SendMessage(update.Message.Chat.Id, "Ошибка формата даты. Повторите попытку.", cancellationToken: cancellationToken);
                        return ScenarioResult.Transition;
                    }

                    var savedUser = (ToDoUser)context.Data["CurrentUser"];
                    var savedTaskName = (string)context.Data["TaskName"];

                    await _toDoService.AddAsync(savedUser, savedTaskName, deadline, cancellationToken);


                    await bot.SendMessage(update.Message.Chat.Id, "Задача успешно добавлена!", cancellationToken: cancellationToken);
                    return ScenarioResult.Completed;

                default:
                    return ScenarioResult.Completed;
            }
        }
    }

}
