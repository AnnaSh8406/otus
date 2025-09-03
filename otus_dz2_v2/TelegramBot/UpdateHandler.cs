using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using otus_dz2_v2.Core.DataAccess;
using otus_dz2_v2.Core.Entities;
using otus_dz2_v2.Core.Exceptions;
using otus_dz2_v2.Core.Services;
using otus_dz2_v2.Infrastructure.DataAccess;
using static otus_dz2_v2.Program;
namespace otus_dz2_v2.TelegramBot
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        private readonly IToDoRepository _toDoRepository;

        public static class IsUserReg
        {
            public static bool IsUserRegistered(ITelegramBotClient botClient, Chat chat, ToDoUser? user, CancellationToken cancellationToken)
            {
                if (user != null)
                    return true;
                botClient.SendMessage(chat, "Вы не зарегестрированы. Вам доступны сл комманды:/start, /help и /exit", cancellationToken);
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
            IToDoRepository toDoRepository)
        {
            _botClient = botClient;
            _userService = userService;
            _toDoService = toDoService;
            _toDoRepository = toDoRepository;
        }
        
        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            await botClient.SendMessage(new Chat { Id = 1 }, "Возникла ошибка: " + exception.Message, cancellationToken);
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update.Message != null && !string.IsNullOrEmpty(update.Message.Text))
                {

                    OnHandleUpdateStarted?.Invoke(update.Message.Text);

                    await ProcessMessage(update.Message, cancellationToken, botClient, update);

                    OnHandleUpdateCompleted?.Invoke(update.Message.Text);
                }
            }
            catch (OperationCanceledException)
            {
                // null
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(botClient, ex, cancellationToken);
            }
        }

        private async Task ProcessMessage(Message message, CancellationToken cancellationToken, ITelegramBotClient botClient, Update update)
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
                    case { } when input.StartsWith("/addtask"):
                        await AddTaskCommand(_botClient, message, input, chat, currentUser, cancellationToken);
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



        private async Task StartCommand(ITelegramBotClient botClient, Message message, Chat chat, User from, ToDoUser? currentUser, CancellationToken cancellationToken)
        {
            if (currentUser == null)
            {

                await _userService.RegisterUserAsync(from.Id, from.Username!, cancellationToken);
                await _botClient.SendMessage(chat, $"Вы зарегистрированы! Пользователь: {from.Username}", cancellationToken);

            }
            else
            {
                await _botClient.SendMessage(chat, $"Вы уже зарегистрированы! Пользователь: {currentUser.TelegramUserName}", cancellationToken);
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
                          "/report - статистика по задачам пользователя", cancellationToken);
        }

        private async Task InfoCommand(ITelegramBotClient botClient, Message message, Chat chat, ToDoUser? currentUser, CancellationToken cancellationToken)
        {
            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser, cancellationToken))
                return;
            await botClient.SendMessage(chat, "Версия бота 1.7", cancellationToken);
        }

        private async Task AddTaskCommand(ITelegramBotClient botClient, Message message, string input, Chat chat, ToDoUser? currentUser, CancellationToken cancellationToken)
        {
            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser, cancellationToken))
                return;

            var taskDescription = input.Substring("/addtask".Length).Trim();

            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                await _botClient.SendMessage(chat, "Ошибка: вы не указали задачу", cancellationToken);
                return;
            }

            ToDoItem task = await _toDoService.AddAsync(currentUser!, taskDescription, cancellationToken);
            await _botClient.SendMessage(chat, $"Задача добавлена: {task.Name}", cancellationToken);

        }

        private async Task ShowTasksCommand(ITelegramBotClient botClient, Message message, Chat chat, ToDoUser? currentUser, CancellationToken cancellationToken)
        {



            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser, cancellationToken))
                return;

            var tasks = await _toDoService.GetActiveByUserIdAsync(currentUser!.UserId, cancellationToken);

            if (tasks.Count == 0)
            {
                await _botClient.SendMessage(chat, "Список задач пуст", cancellationToken);
                return;
            }

            var tasksInfo = tasks
                .Select(task => $"{task.Name} - {task.CreatedAt} - {task.Id}").ToList();
            var message2 = "Ваши задачи:\n" + string.Join("\n", tasksInfo);
            await _botClient.SendMessage(chat, message2, cancellationToken);


        }

        private async Task ShowAllTasksCommand(ITelegramBotClient botClient, Message message, Chat chat, ToDoUser? currentUser, CancellationToken cancellationToken)
        {
            if (!IsUserReg.IsUserRegistered(botClient, message.Chat, currentUser, cancellationToken))
                return;

            var tasks = await _toDoService.GetAllByUserIdAsync(currentUser!.UserId, cancellationToken);

            if (tasks.Count == 0)
            {
                await _botClient.SendMessage(message.Chat, "Список задач пуст", cancellationToken);
                return;
            }

            var tasksInfo = tasks
                .Select(task => $"Название: {task.Name}, Дата создания: {task.CreatedAt} , ID: {task.Id}, Состояние: {task.State}").ToList();
            var message2 = "Ваши задачи:\n" + string.Join("\n", tasksInfo);
            await _botClient.SendMessage(message.Chat, message2, cancellationToken);
        }

        private async Task RemoveTaskCommand(ITelegramBotClient botClient, Message message, string input, Chat chat, ToDoUser? currentUser, CancellationToken cancellationToken)
        {

            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser, cancellationToken))
                return;

            var tasks = await _toDoService.GetActiveByUserIdAsync(currentUser!.UserId, cancellationToken);

            if (tasks.Count == 0)
            {
                await _botClient.SendMessage(chat, "Список задач пуст", cancellationToken);
                return;
            }

            var taskToRemoveIndex = input.Substring("/removetask".Length).Trim();

            if (!int.TryParse(taskToRemoveIndex, out var taskNumber) || taskNumber < 1 || taskNumber > tasks.Count)
            {
                await _botClient.SendMessage(chat, $"Некорректный номер. Введите число от 1 до {tasks.Count}", cancellationToken);
                return;
            }

            var taskToRemove = tasks[taskNumber - 1];
            await _toDoService.DeleteAsync(taskToRemove.Id, cancellationToken);
            await _botClient.SendMessage(chat, $"Задача удалена: {taskToRemove.Name}", cancellationToken);
        }

        private async Task CompleteTaskCommand(ITelegramBotClient botClient, Message message, string input, Chat chat, ToDoUser? currentUser, CancellationToken cancellationToken)
        {
            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser, cancellationToken))
                return;

            var tasks = await _toDoService.GetActiveByUserIdAsync(currentUser!.UserId, cancellationToken);

            if (tasks.Count == 0)
            {
                await _botClient.SendMessage(chat, "Список активных задач пуст", cancellationToken);
                return;
            }

            var taskIdString = input.Substring("/completeTask".Length).Trim();

            if (!int.TryParse(taskIdString, out var taskNumber) || taskNumber < 1 || taskNumber > tasks.Count)
            {
                await _botClient.SendMessage(chat, $"Некорректный номер. Введите число от 1 до {tasks.Count}", cancellationToken);
                return;
            }
            var taskToComplete = tasks[taskNumber - 1];


            await _toDoService.MarkCompletedAsync(taskToComplete.Id, cancellationToken);
            await _botClient.SendMessage(chat, $"Задача: {taskToComplete.Name} завершена", cancellationToken);
        }

        private async Task ExitCommand(ITelegramBotClient botClient, Message message, Chat chat, CancellationToken cancellationToken)
        {
            await _botClient.SendMessage(chat, "Завершение работы...", cancellationToken);
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
                await _botClient.SendMessage(chat, "Список задач пуст", cancellationToken);
                return;
            }


            if (toDoItems.Any())
            {
                foreach (var task in toDoItems)
                {
                    await _botClient.SendMessage(chat, $"Название: {task.Name}, Дата создания: {task.CreatedAt} , ID: {task.Id}, Состояние: {task.State}", cancellationToken);
                }
            }
            else
            {
                await _botClient.SendMessage(chat, "Задачи не найдены.", cancellationToken);
            }
        }



        private async Task ReportCommand(ITelegramBotClient botClient, Message message, Chat chat, ToDoUser? currentUser, CancellationToken cancellationToken)
        {

            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser, cancellationToken))
                return;
            IToDoReportService reportService = new ToDoReportService(_toDoService);

            var stats = await reportService.GetUserStatsAsync(currentUser.UserId, cancellationToken);
            await _botClient.SendMessage(chat, $" Статистика по задачам на {stats.GeneratedAt}:\nВсего: {stats.Total};\nЗавершенных: {stats.Completed};\nАктивных: {stats.Active} ; ", cancellationToken);



        }

        private async Task DefaultCommand(ITelegramBotClient botClient, Message message, Chat chat, ToDoUser? currentUser, CancellationToken cancellationToken)
        {

            var commands = currentUser != null;

            await _botClient.SendMessage(chat, $"Вы ввели неизвестную команду!", cancellationToken);
        }


    }



}
