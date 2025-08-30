using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using otus_dz2_v2.core.DataAccess;
using otus_dz2_v2.core.Entities;
using otus_dz2_v2.core.Exceptions;
using otus_dz2_v2.core.Services;
using otus_dz2_v2.Infrastructure.DataAccess;
using static otus_dz2_v2.Program;
namespace otus_dz2_v2.TelegramBot
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;

        public static class IsUserReg
        {
            public static bool IsUserRegistered(ITelegramBotClient botClient, Chat chat, ToDoUser? user)
            {
                if (user != null)
                    return true;
                botClient.SendMessage(chat, "Вы не зарегестрированы. Вам доступны сл комманды:/start, /help и /exit");
                return false;
            }
        }
        public UpdateHandler(IUserService userService, IToDoService toDoService)
        {
            _userService = userService;
            _toDoService = toDoService;
        }


        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            var input = update.Message.Text;
            var chat = update.Message.Chat;
            var from = update.Message.From;

            try
            {
                var currentUser = _userService.GetUser(from.Id);

                switch (input)
                {
                    case "/start":
                        StartCommand(botClient, chat, from, currentUser);
                        break;
                    case "/help":
                        HelpCommand(botClient, chat);
                        break;
                    case "/info":
                        InfoCommand(botClient, chat, currentUser);
                        break;
                    case { } when input.StartsWith("/addtask"):
                        AddTaskCommand(botClient, chat, input, currentUser);
                        break;
                    case "/showtasks":
                        ShowTasksCommand(botClient, chat, currentUser);
                        break;
                    case "/showalltasks":
                        ShowAllTasksCommand(botClient, chat, currentUser);
                        break;
                    case { } when input.StartsWith("/removetask"):
                        RemoveTaskCommand(botClient, chat, input, currentUser);
                        break;
                    case { } when input.StartsWith("/completetask"):
                        CompleteTaskCommand(botClient, chat, input, currentUser);
                        break;
                    case { } when input.StartsWith("/find"):
                        FindCommand(botClient, chat, input, currentUser);
                        break;
                    case "/report":
                        ReportCommand(botClient, chat, currentUser);
                        break;
                    case "/exit":
                        ExitCommand(botClient, chat);
                        break;
                    default:
                        DefaultCommand(botClient, chat, currentUser);
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

        private void StartCommand(ITelegramBotClient botClient, Chat chat, User from, ToDoUser? currentUser)
        {
            if (currentUser == null)
            {
                _userService.RegisterUser(from.Id, from.Username!);
                botClient.SendMessage(chat, $"Вы зарегистрированы! Пользователь: {from.Username}");
            }
            else
            {
                botClient.SendMessage(chat, $"Вы уже зарегистрированы! Пользователь: {currentUser.TelegramUserName}");
            }
        }

        private void HelpCommand(ITelegramBotClient botClient, Chat chat)
        {
            botClient.SendMessage(chat, "Краткое описание:\n" +
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
                          "/report - статистика по задачам пользователя");
        }

        private void InfoCommand(ITelegramBotClient botClient, Chat chat, ToDoUser? currentUser)
        {
            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser))
                return;
            botClient.SendMessage(chat, "Версия бота 1.7");
        }

        private void AddTaskCommand(ITelegramBotClient botClient, Chat chat, string input, ToDoUser? currentUser)
        {
            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser))
                return;

            var taskDescription = input.Substring("/addtask".Length).Trim();

            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                botClient.SendMessage(chat, "Ошибка: вы не указали задачу");
                return;
            }

            var task = _toDoService.Add(currentUser!, taskDescription);
            botClient.SendMessage(chat, $"Задача добавлена: {task.Name}");
        }

        private void ShowTasksCommand(ITelegramBotClient botClient, Chat chat, ToDoUser? currentUser)
        {
            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser))
                return;

            var tasks = _toDoService.GetActiveByUserId(currentUser!.UserId);

            if (tasks.Count == 0)
            {
                botClient.SendMessage(chat, "Список задач пуст");
                return;
            }

            var tasksInfo = tasks
                .Select(task => $"{task.Name} - {task.CreatedAt} - {task.Id}").ToList();
            var message = "Ваши задачи:\n" + string.Join("\n", tasksInfo);
            botClient.SendMessage(chat, message);
        }

        private void ShowAllTasksCommand(ITelegramBotClient botClient, Chat chat, ToDoUser? currentUser)
        {
            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser))
                return;

            var tasks = _toDoService.GetAllByUserId(currentUser!.UserId);

            if (tasks.Count == 0)
            {
                botClient.SendMessage(chat, "Список задач пуст");
                return;
            }

            var tasksInfo = tasks
                .Select(task => $"Название: {task.Name}, Дата создания: {task.CreatedAt} , ID: {task.Id}, Состояние: {task.State}").ToList();
            var message = "Ваши задачи:\n" + string.Join("\n", tasksInfo);
            botClient.SendMessage(chat, message);
        }

        private void RemoveTaskCommand(ITelegramBotClient botClient, Chat chat, string input, ToDoUser? currentUser)
        {
            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser))
                return;

            var tasks = _toDoService.GetActiveByUserId(currentUser!.UserId).ToList();

            if (tasks.Count == 0)
            {
                botClient.SendMessage(chat, "Список задач пуст");
                return;
            }

            var taskToRemoveIndex = input.Substring("/removetask".Length).Trim();

            if (!int.TryParse(taskToRemoveIndex, out var taskNumber) || taskNumber < 1 || taskNumber > tasks.Count)
            {
                botClient.SendMessage(chat, $"Некорректный номер. Введите число от 1 до {tasks.Count}");
                return;
            }

            var taskToRemove = tasks[taskNumber - 1];
            _toDoService.Delete(taskToRemove.Id);
            botClient.SendMessage(chat, $"Задача удалена: {taskToRemove.Name}");
        }

        private void CompleteTaskCommand(ITelegramBotClient botClient, Chat chat, string input, ToDoUser? currentUser)
        {
            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser))
                return;

            var tasks = _toDoService.GetActiveByUserId(currentUser!.UserId).ToList();

            if (tasks.Count == 0)
            {
                botClient.SendMessage(chat, "Список активных задач пуст");
                return;
            }

            var taskIdString = input.Substring("/completeTask".Length).Trim();

            if (!int.TryParse(taskIdString, out var taskNumber) || taskNumber < 1 || taskNumber > tasks.Count)
            {
                botClient.SendMessage(chat, $"Некорректный номер. Введите число от 1 до {tasks.Count}");
                return;
            }
            var taskToComplete = tasks[taskNumber - 1];


            _toDoService.MarkCompleted(taskToComplete.Id, currentUser);
            botClient.SendMessage(chat, $"Задача: {taskToComplete.Name} завершена");
        }

        private void ExitCommand(ITelegramBotClient botClient, Chat chat)
        {
            botClient.SendMessage(chat, "Завершение работы...");
            Environment.Exit(0);
        }

        private void FindCommand(ITelegramBotClient botClient, Chat chat, string input, ToDoUser? currentUser)
        {
            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser))
                return;

            var tasks = input.Substring("/find".Length).Trim();
            IReadOnlyList<ToDoItem> toDoItems = _toDoService.Find(currentUser, tasks);
            var tasksf = _toDoService.GetAllByUserId(currentUser!.UserId);

            if (tasksf.Count == 0)
            {
                botClient.SendMessage(chat, "Список задач пуст");
                return;
            }


            if (toDoItems.Any())
            {
                foreach (var task in toDoItems)
                {
                    botClient.SendMessage(chat, $"Название: {task.Name}, Дата создания: {task.CreatedAt} , ID: {task.Id}, Состояние: {task.State}");
                }
            }
            else
            {
                botClient.SendMessage(chat, "Задачи не найдены.");
            }
        }



        private void ReportCommand(ITelegramBotClient botClient, Chat chat, ToDoUser? currentUser)
        {

            if (!IsUserReg.IsUserRegistered(botClient, chat, currentUser))
                return;
            IToDoReportService reportService = new ToDoReportService(_toDoService);

            var stats = reportService.GetUserStats(currentUser.UserId);
            botClient.SendMessage(chat, $" Статистика по задачам на {stats.generatedAt}:\nВсего: {stats.total};\nЗавершенных: {stats.completed};\nАктивных: {stats.active} ; ");

        }

        private void DefaultCommand(ITelegramBotClient botClient, Chat chat, ToDoUser? currentUser)
        {

            var commands = currentUser != null;

            botClient.SendMessage(chat, $"Вы ввели неизвестную команду!");
        }


        private readonly IToDoService _toDoService;
        public UpdateHandler(IToDoService toDoService)
        {
            _toDoService = toDoService;
        }




        public void HandlerAddTask(ToDoUser userId, string taskName)
        {
            try
            {

                _toDoService.Add(userId, taskName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void HandlerRemoveTask(Guid taskId)
        {
            _toDoService.Delete(taskId);
        }
    }


}
