using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using otus_dz2_v2.Core.DataAccess;
using otus_dz2_v2.Core.Entities;
using otus_dz2_v2.Core.Exceptions;
using otus_dz2_v2.Core.Services;
using otus_dz2_v2.Infrastructure.DataAccess;
using static otus_dz2_v2.Program;

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using otus_dz2_v2.Scenarios;
using System.Globalization;

namespace otus_dz2_v2.TelegramBot
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        private readonly IToDoRepository _toDoRepository;
        private readonly IToDoReportService _toDoReportService;
        private readonly IScenarioContextRepository _contextRepository;

        public static class IsUserReg
        {
            public static bool IsUserRegistered(ITelegramBotClient botClient, Chat chat, ToDoUser? user, CancellationToken cancellationToken)
            {
                if (user != null)





                    return true;

                botClient.SendMessage(chat, "Вы не зарегестрированы. Вам доступны сл комманды:/start, /help и /exit", cancellationToken: cancellationToken);
                return false;
            }


        }


        public delegate void MessageEventHandler(string message);
        public event MessageEventHandler? OnHandleUpdateStarted;
        public event MessageEventHandler? OnHandleUpdateCompleted;
        public UpdateHandler(
            ITelegramBotClient botClient,
            IUserService userService,
            IToDoService toDoService,
            IToDoRepository toDoRepository,
             IToDoReportService toDoReportService, IScenarioContextRepository contextRepository
            )
        {
            _botClient = botClient;
            _userService = userService;
            _toDoService = toDoService;
            _toDoRepository = toDoRepository;

            _toDoReportService = toDoReportService;
            _contextRepository = contextRepository;
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Ошибка при обработке обновления: {exception.Message}");


            if (exception is Telegram.Bot.Exceptions.ApiRequestException apiEx && !string.IsNullOrEmpty(apiEx.Message))
            {
                await botClient.SendMessage(-1, "Возникла внутренняя ошибка. Попробуйте позже.", cancellationToken: cancellationToken);
            }
            else
            {
                await botClient.SendMessage(new Chat { Id = 1 }, "Возникла внутренняя ошибка. Попробуйте позже.", cancellationToken: cancellationToken);
                Console.WriteLine("Сообщение об ошибке невозможно передать пользователю.");
            }
        }
        private string EscapeMarkdownCharacters(string input)
        {
            return input
                .Replace("\\", "\\\\")
                .Replace("-", "\\-");
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            OnHandleUpdateStarted?.Invoke(update.Message.Text);


            var context = await _contextRepository.GetContext(update.Message.From.Id, cancellationToken);


            if (context != null)
            {
                // Проверяем /cancel
                if (update.Message.Text == "/cancel")
                {

                    await _contextRepository.ResetContext(update.Message.From.Id, cancellationToken);
                    await botClient.SendMessage(update.Message.Chat.Id, "Действие отменено.", cancellationToken: cancellationToken);
                    return;
                }


                await ProcessScenario(context, update, cancellationToken);
                return;
            }


            await ProcessMessage(update.Message, cancellationToken, botClient, update);

        }

        private async Task ProcessScenario(ScenarioContext context, Update update, CancellationToken cancellationToken)
        {
            var scenario = GetScenario(context.CurrentScenario);
            var result = await scenario.HandleMessageAsync(_botClient, context, update, cancellationToken);

            if (result == ScenarioResult.Completed)
            {
                await _contextRepository.ResetContext(context.UserId, cancellationToken);
            }
            else
            {
                await _contextRepository.SetContext(context.UserId, context, cancellationToken);
            }
        }
        private IScenario GetScenario(ScenarioType type)
        {
            switch (type)
            {
                case ScenarioType.AddTask:
                    return new AddTaskScenario(_userService, _toDoService);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Сценарий не поддерживается");
            }
        }
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
            public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient botClient, ScenarioContext context, Update update, CancellationToken cancellationToken)
            {
                var message = update.Message!.Text!;
                try
                {
                    switch (context.CurrentStep)
                    {
                        case null:
                            var user = await _userService.GetUserAsync(update.Message.From.Id, cancellationToken);
                            context.Data["CurrentUser"] = user; await botClient.SendMessage(update.Message.Chat.Id, "Введите название задачи:", cancellationToken: cancellationToken);
                            context.CurrentStep = "Name";
                            return ScenarioResult.Transition;
                        case "Name":
                            var taskName = message;
                            context.Data["TaskName"] = taskName;
                            await botClient.SendMessage(update.Message.Chat.Id, "Введите срок выполнения задачи (ДД.ММ.ГГГГ):", cancellationToken: cancellationToken);
                            context.CurrentStep = "Deadline";
                            return ScenarioResult.Transition;
                        case "Deadline":

                            if (!DateTime.TryParseExact(message, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var deadline))
                            {
                                await botClient.SendMessage(update.Message.Chat.Id, "Некорректный формат даты. Повторите ввод.", cancellationToken: cancellationToken); return ScenarioResult.Transition;
                            }
                            var userFromContext = (ToDoUser)context.Data["CurrentUser"];
                            var taskNameFromContext = (string)context.Data["TaskName"];


                            await _toDoService.AddAsync(userFromContext, taskNameFromContext, deadline, cancellationToken);
                            await botClient.SendMessage(update.Message.Chat.Id, "Задача успешно добавлена!", cancellationToken: cancellationToken);
                            return ScenarioResult.Completed;

                        default: return ScenarioResult.Completed;
                    }
                }
                catch (TaskCountLimitException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                    await botClient.SendMessage(update.Message.Chat.Id, $"Ошибка: { e.Message}", cancellationToken: cancellationToken); 
                }
                catch (TaskLengthLimitException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                    await botClient.SendMessage(update.Message.Chat.Id, $"Ошибка: {e.Message}", cancellationToken: cancellationToken);
                }
                catch (DuplicateTaskException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                    await botClient.SendMessage(update.Message.Chat.Id, $"Ошибка: {e.Message}", cancellationToken: cancellationToken);
                }
                 return ScenarioResult.Completed;
            }
        }

        /// /////
        private async Task SendStartMenu(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            await botClient.SendMessage(chatId, "Для начала работы нажмите /start",
                replyMarkup: Keyboard.StartButton(), cancellationToken: cancellationToken);
        }
        private async Task SendRegisteredMenu(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            await botClient.SendMessage(chatId, "Ожидайте...",
                replyMarkup: Keyboard.RegisteredButtons(), cancellationToken: cancellationToken);
        }

        /// /////

        private async Task ProcessMessage(Message message, CancellationToken cancellationToken, ITelegramBotClient botClient, Update update)
        {


            ToDoUser? currentUser = await _userService.GetUserAsync(message.From.Id, cancellationToken);

            if (currentUser == null)
            {
                await SendStartMenu(botClient, message.Chat.Id, cancellationToken);
            }
            else
            {
                await SendRegisteredMenu(botClient, message.Chat.Id, cancellationToken);
            }

            await ProcessCommand(botClient, message, cancellationToken, update);
        }
        private async Task ProcessCommand(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken, Update update)
        {
            if (message.Text.StartsWith('/'))
            {
                var parts = message.Text.Split(' ', 2);
                string inp = parts.Length > 0 ? parts[0].ToLower() : "";
                var input = update.Message.Text;
                var chat = update.Message.Chat;
                var from = update.Message.From;
                ToDoUser? currentUser = await _userService.GetUserAsync(message.From.Id, cancellationToken);
                try
                {

                    switch (inp)
                    {
                        case "/start":
                            await StartCommand(_botClient, message, chat, from, currentUser, cancellationToken);
                            break;
                        case "/help":
                            await HelpCommand(_botClient, message, cancellationToken);
                            break;
                        case "/info":
                            await InfoCommand(_botClient, message, chat, currentUser, cancellationToken);
                            break;
                        /*case { } when input.StartsWith("/addtask"):
                            await AddTaskCommand(_botClient, message, input, chat, currentUser, cancellationToken);
                            break;*/
                        case "/addtask":
                            var addTaskContext = new ScenarioContext(message.From.Id, ScenarioType.AddTask);
                            await ProcessScenario(addTaskContext, update, cancellationToken); // Передаем update сюда
                            break;
                        case "/showtasks":
                            await ShowTasksCommand(_botClient, message, chat, currentUser, cancellationToken);
                            break;
                        case "/showalltasks":
                            await ShowAllTasksCommand(_botClient, message, chat, currentUser, cancellationToken);
                            break;
                        case { } when input.StartsWith("/removetask"):
                            await RemoveTaskCommand(_botClient, message, input, chat, currentUser, cancellationToken);
                            break;
                        case { } when input.StartsWith("/completetask"):
                            await CompleteTaskCommand(_botClient, message, input, chat, currentUser, cancellationToken);
                            break;
                        case { } when input.StartsWith("/find"):
                            await FindCommand(_botClient, message, input, chat, currentUser, cancellationToken);
                            break;
                        case "/report":
                            await ReportCommand(_botClient, message, chat, currentUser, cancellationToken);
                            break;
                        case "/exit":
                            await ExitCommand(_botClient, message, chat, cancellationToken);
                            break;
                        default:
                            await DefaultCommand(_botClient, message, chat, currentUser, cancellationToken);
                            break;
                    }
                }

                catch (TaskCountLimitException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
                catch (TaskLengthLimitException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
                catch (DuplicateTaskException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }

            }
        }



        private async Task StartCommand(ITelegramBotClient botClient, Message message, Chat chat, User from, ToDoUser? currentUser, CancellationToken cancellationToken)
        {
            if (currentUser == null)
            {

                await _userService.RegisterUserAsync(from.Id, from.Username!, cancellationToken);
                await _botClient.SendMessage(chat, $"Вы зарегистрированы! Пользователь: {from.Username}", cancellationToken: cancellationToken);

            }
            else
            {
                await _botClient.SendMessage(chat, $"Вы уже зарегистрированы! Пользователь: {currentUser.TelegramUserName}", cancellationToken: cancellationToken);
            }
        }

        private async Task HelpCommand(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            await _botClient.SendMessage(message.Chat, "Краткое описание:\n" +
                          "/start - начало работы\n" +
                          "/help - краткое описание доступных комманд\n" +
                          "/info - информация о версии и дате запуска кода\n" +
                          "/exit - завершение работы\n" +
                          "/addtask + название задачи - добавить задачу в список дел\n" +
                          "/showtasks - показать добавленные активные задачи\n" +
                          "/removetask + номер удаляемой задачи - удалить задачу\n" +
                          "/completetask + номер - завершить активную задачу\n" +
                          "/showalltasks - показать все задачи пользователя\n" +
                          "/find + название задачи - поиск задачи в списке дел\n" +
                          "/cancel - отмена действия при вводе задачи\n" +
                          "/report - статистика по задачам пользователя", cancellationToken: cancellationToken);
        }

        private async Task InfoCommand(ITelegramBotClient botClient, Message message, Chat chat, ToDoUser? currentUser, CancellationToken cancellationToken)
        {
            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser, cancellationToken))
                return;
            await botClient.SendMessage(chat, "Версия бота 1.11", cancellationToken: cancellationToken);
        }
        /*
        private async Task AddTaskCommand(ITelegramBotClient botClient, Message message, string input, Chat chat,   ToDoUser? currentUser, CancellationToken cancellationToken)
        {
            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser, cancellationToken))
                return;

            var taskDescription = input.Substring("/addtask".Length).Trim();

            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                await _botClient.SendMessage(chat, "Ошибка: вы не указали задачу", cancellationToken: cancellationToken);
                return;
            }

            try
            {
                ToDoItem task = await _toDoService.AddAsync(currentUser!, taskDescription,deadline, cancellationToken);
                await _botClient.SendMessage(chat, $"Задача добавлена: {task.Name}", cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                await _botClient.SendMessage(chat, $"Ошибка добавления задачи: {ex.Message}",
                                                      cancellationToken: cancellationToken);
            }
        } */

        private async Task ShowTasksCommand(ITelegramBotClient botClient, Message message, Chat chat, ToDoUser? currentUser, CancellationToken cancellationToken)
        {



            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser, cancellationToken))
                return;

            var tasks = await _toDoService.GetActiveByUserIdAsync(currentUser!.UserId, cancellationToken);

            if (tasks.Count == 0)
            {
                await _botClient.SendMessage(chat, "Список задач пуст", cancellationToken: cancellationToken);
                return;
            }



            var formattedOutput = string.Join("\n", tasks.Select((task, i) =>
                $"{i + 1}. `{EscapeMarkdownCharacters(task.Name)}` - {task.CreatedAt} - `{task.Id}` - дедлайн до {task.Deadline}"));

            await _botClient.SendMessage(chat, formattedOutput, parseMode: ParseMode.Markdown, cancellationToken: cancellationToken);

        }

        private async Task ShowAllTasksCommand(ITelegramBotClient botClient, Message message, Chat chat, ToDoUser? currentUser, CancellationToken cancellationToken)
        {
            if (!IsUserReg.IsUserRegistered(botClient, message.Chat, currentUser, cancellationToken))
                return;

            var tasks = await _toDoService.GetAllByUserIdAsync(currentUser!.UserId, cancellationToken);

            if (tasks.Count == 0)
            {
                await _botClient.SendMessage(message.Chat, "Список задач пуст", cancellationToken: cancellationToken);
                return;
            }


            var formattedOutput = string.Join("\n", tasks.Select((task, i) =>
              $"{i + 1}. `{EscapeMarkdownCharacters(task.Name)}` - {task.CreatedAt} - `{task.Id} - {task.State}`- дедлайн до {task.Deadline}"));

            await _botClient.SendMessage(chat, formattedOutput, parseMode: ParseMode.Markdown, cancellationToken: cancellationToken);

        }

        private async Task RemoveTaskCommand(ITelegramBotClient botClient, Message message, string input, Chat chat, ToDoUser? currentUser, CancellationToken cancellationToken)
        {

            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser, cancellationToken))
                return;

            var tasks = await _toDoService.GetActiveByUserIdAsync(currentUser!.UserId, cancellationToken);

            if (tasks.Count == 0)
            {
                await _botClient.SendMessage(chat, "Список задач пуст", cancellationToken: cancellationToken);
                return;
            }

            var taskToRemoveIndex = input.Substring("/removetask".Length).Trim();

            if (!int.TryParse(taskToRemoveIndex, out var taskNumber) || taskNumber < 1 || taskNumber > tasks.Count)
            {
                await _botClient.SendMessage(chat, $"Некорректный номер. Введите число от 1 до {tasks.Count}", cancellationToken: cancellationToken);
                return;
            }

            var taskToRemove = tasks[taskNumber - 1];
            await _toDoService.DeleteAsync(taskToRemove.Id, cancellationToken);
            await _botClient.SendMessage(chat, $"Задача удалена: {taskToRemove.Name}", cancellationToken: cancellationToken);
        }

        private async Task CompleteTaskCommand(ITelegramBotClient botClient, Message message, string input, Chat chat, ToDoUser? currentUser, CancellationToken cancellationToken)
        {
            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser, cancellationToken))
                return;

            var tasks = await _toDoService.GetActiveByUserIdAsync(currentUser!.UserId, cancellationToken);

            if (tasks.Count == 0)
            {
                await _botClient.SendMessage(chat, "Список активных задач пуст", cancellationToken: cancellationToken);
                return;
            }

            var taskIdString = input.Substring("/completeTask".Length).Trim();

            if (!int.TryParse(taskIdString, out var taskNumber) || taskNumber < 1 || taskNumber > tasks.Count)
            {
                await _botClient.SendMessage(chat, $"Некорректный номер. Введите число от 1 до {tasks.Count}", cancellationToken: cancellationToken);
                return;
            }
            var taskToComplete = tasks[taskNumber - 1];


            await _toDoService.MarkCompletedAsync(taskToComplete.Id, cancellationToken);
            await _botClient.SendMessage(chat, $"Задача: {taskToComplete.Name} завершена", cancellationToken: cancellationToken);
        }

        private async Task ExitCommand(ITelegramBotClient botClient, Message message, Chat chat, CancellationToken cancellationToken)
        {
            await _botClient.SendMessage(chat, "Завершение работы...", cancellationToken: cancellationToken);
            Environment.Exit(0);

        }

        private async Task FindCommand(ITelegramBotClient botClient, Message message, string input, Chat chat, ToDoUser? currentUser, CancellationToken cancellationToken)
        {
            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser, cancellationToken))
                return;

            var tasks = input.Substring("/find".Length).Trim();
            IReadOnlyList<ToDoItem> toDoItems = await _toDoService.FindAsync(currentUser, tasks, cancellationToken);
            var tasksf = await _toDoService.GetAllByUserIdAsync(currentUser!.UserId, cancellationToken);

            if (tasksf.Count == 0)
            {
                await _botClient.SendMessage(chat, "Список задач пуст", cancellationToken: cancellationToken);
                return;
            }


            if (toDoItems.Any())
            {
                foreach (var task in toDoItems)
                {
                    await _botClient.SendMessage(chat, $"Название: {task.Name}, Дата создания: {task.CreatedAt} , ID: {task.Id}, Состояние: {task.State}", cancellationToken: cancellationToken);
                }
            }
            else
            {
                await _botClient.SendMessage(chat, "Задачи не найдены.", cancellationToken: cancellationToken);
            }
        }



        private async Task ReportCommand(ITelegramBotClient botClient, Message message, Chat chat, ToDoUser? currentUser, CancellationToken cancellationToken)
        {

            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser, cancellationToken))
                return;
            IToDoReportService reportService = new ToDoReportService(_toDoService);

            var stats = await reportService.GetUserStatsAsync(currentUser.UserId, cancellationToken);
            await _botClient.SendMessage(chat, $" Статистика по задачам на {stats.GeneratedAt}:\nВсего: {stats.Total};\nЗавершенных: {stats.Completed};\nАктивных: {stats.Active} ; ", cancellationToken: cancellationToken);



        }

        private async Task DefaultCommand(ITelegramBotClient botClient, Message message, Chat chat, ToDoUser? currentUser, CancellationToken cancellationToken)
        {

            var commands = currentUser != null;

            await _botClient.SendMessage(chat, $"Вы ввели неизвестную команду!", cancellationToken: cancellationToken);
        }


    }



}
