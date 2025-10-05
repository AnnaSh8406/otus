using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.VisualBasic;
using otus_dz2_v2.Core.DataAccess;
using otus_dz2_v2.Core.Services;
using otus_dz2_v2.Infrastructure.DataAccess;
using otus_dz2_v2.TelegramBot;
using System.Numerics;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using otus_dz2_v2.Scenarios;




namespace otus_dz2_v2
{

    internal class Program
    {
        public static int maxTasks;
        public static int maxTaskLenght;


        private static int ParseAndValidateInt(string? str, int min, int max)
        {

            if (!int.TryParse(str, out int number))
                throw new ArgumentException("Не удалось преобразовать строку в число");


            if (number < min || number > max)

                throw new Exception("Превышено максимальное кол-во задач/длина задачи или некорректное значение");

            return number;
        }
        private static int SetMaxTasks()
        {

            Console.WriteLine("Введите максимально допустимое количество задач от 1 до 100 шт");

            string inputString = Console.ReadLine();

            IsString(inputString);

            int maxTasks = ParseAndValidateInt(inputString, 1, 100);

            return maxTasks;
        }
        private static int SetMaxLengthNameTasks()
        {
            Console.WriteLine("Введите максимально допустимую длину задачи от 1 символа до 100");
            string inputString = Console.ReadLine();
            IsString(inputString);
            int maxLengthNameTask = ParseAndValidateInt(inputString, 1, 100);

            return maxLengthNameTask;
        }
        private static void IsString(string? str)
        {
            if (str == null || str.Trim() == "")
                throw new ArgumentException("Введеная строка пустая");

        }
        static async Task Main()
        {
            Console.WriteLine("Введите максимально допустимое количество задач");
            var taskCountLimit = UpdateHandler.ParseAndValidateInt(Console.ReadLine(), 1, 100);

            Console.WriteLine("Введите максимально допустимую длину задачи");
            var taskLengthLimit = UpdateHandler.ParseAndValidateInt(Console.ReadLine(), 1, 100);

            string token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN", EnvironmentVariableTarget.User) ?? "";
            var botClient = new TelegramBotClient(token);
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery],
                DropPendingUpdates = true,

            };
            var ping = await botClient.GetMe();


            var toDoRepository = new FileToDoRepository(Keyboard.dataDir);

            var cts = new CancellationTokenSource();
            var userService = new UserService();
            var toDoService = new ToDoService(taskCountLimit, taskLengthLimit, toDoRepository);
            var toDoListService = new ToDoListService();

            var scenarioList = new List<IScenario>();
            scenarioList.Add(new AddTaskScenario(userService, toDoService, toDoListService));
            scenarioList.Add(new AddListScenario(userService, toDoListService));
            scenarioList.Add(new DeleteListScenario(userService, toDoListService, toDoService));
            scenarioList.Add(new DeleteTaskScenario(userService, toDoService));

            var handler = new UpdateHandler(userService, botClient, toDoService,
                                            new ToDoReportService(toDoRepository), scenarioList, new InMemoryScenarioContextRepository(), new ToDoListService());

            try
            {

                handler.SubscribeOnUpdateStarted(UpdateStarted);
                handler.SubscribeOnUpdateCompleted(UpdateCompleted);

                await botClient.SetMyCommands(
                new[]
                {
                    new BotCommand { Command = "start", Description = "Запуск бота" },
                    new BotCommand { Command = "help", Description = "Описание комманд" },
                    new BotCommand { Command = "show", Description = "Список задач" },
                    new BotCommand { Command = "report", Description = "Статистика по задачам" },
                }, cancellationToken: cts.Token
              );

                botClient.StartReceiving(handler, receiverOptions, cancellationToken: cts.Token);
                Console.WriteLine("Нажмите английскую \"A\" для остановки бота.");
                var keyPressTask = Task.Run(() => KeyPress(botClient, cts.Token));

                await Task.WhenAny(keyPressTask);
                if (keyPressTask.IsCompleted)
                {
                    cts.Cancel();
                    Console.WriteLine("\nЗавершение работы бота...");
                }
            }
            catch (OperationCanceledException e)
            {
                ShowError("Бот остановлен");
            }
            catch (Exception ex)
            {
                ShowError($"Произошла непредвиденная ошибка:{ex.GetType()}\n{ex.Message}\n{ex.StackTrace}\n{ex.InnerException}");
            }
            finally
            {
                handler.UnSubscribeOnUpdateStarted(UpdateStarted);
                handler.UnSubscribeOnUpdateCompleted(UpdateCompleted);
            }
        }

        private async static Task KeyPress(TelegramBotClient bot, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.A)
                {
                    return;
                }
                else
                {

                    var me = await bot.GetMe();
                    Console.WriteLine($"\nИнформация о боте: @{me.Username}, ID: {me.Id}");
                    Console.WriteLine("Нажмите клавишу A для выхода");
                }
                await Task.Delay(100);
            }
        }
        private static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void UpdateStarted(string message)
        {
            Console.WriteLine($"Началась обработка сообщения {message}");
        }

        private static void UpdateCompleted(string message)
        {
            Console.WriteLine($"Закончилась обработка сообщения {message}");
        }
    }

}
