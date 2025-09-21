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

    public class Program
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

        private static void HandleUpdateStarted(string message) => Console.WriteLine($"Началась обработка сообщения '{message}'.");

        private static void HandleUpdateCompleted(string message) => Console.WriteLine($"Закончилась обработка сообщения '{message}'.");

        static async Task Main(string[] args)
        {

            try
            {
                int maxTasks = SetMaxTasks();
                int maxLengthNameTask = SetMaxLengthNameTasks();

                string _botKey = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN", EnvironmentVariableTarget.User);
                var botClient = new TelegramBotClient(_botKey);
                string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                Console.WriteLine($"Базовая директория данных: {dataDir}");

                IUserRepository userRepository = new FileUserRepository(dataDir);
                IUserService userService = new UserService(userRepository);
                IToDoRepository toDoRepository = new FileToDoRepository(dataDir);
                IToDoService toDoService = new ToDoService(maxTasks, maxLengthNameTask, toDoRepository);
                var toDoReportService = new ToDoReportService(toDoService);// Генерация отчетов по задачам
                var contextRepository = new InMemoryScenarioContextRepository(); // создается экземпляр репозитория контекстов


                var handler = new UpdateHandler(botClient, userService, toDoService, toDoRepository, toDoReportService, contextRepository);

                void DisplayStartEventMessage(string message) => Console.WriteLine($"\r\nНачалась обработка сообщения {message}\r\n");
                void DisplayCompleteEventMessage(string message) => Console.WriteLine($"Закончилась обработка сообщения {message}\r\n");

                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = [UpdateType.Message],
                    DropPendingUpdates = true
                };

                try
                {
                    using (CancellationTokenSource ct = new CancellationTokenSource())
                    {
                        var me = await botClient.GetMe();
                        handler.OnHandleUpdateStarted += HandleUpdateStarted;
                        handler.OnHandleUpdateCompleted += HandleUpdateCompleted;


                        // команды для бота
                        await botClient.SetMyCommands(new BotCommand[]
                        {
                            new BotCommand("/start", "Начало работы"),     // Стартовая команда
                            new BotCommand("/help", "Справка по командам"),// Справочная команда
                            new BotCommand("/info", "Информация о сервисе"),// Команда для вывода инфо о сервисе
                            new BotCommand("/addtask", "Добавить задачу"), // Добавление новой задачи
                            new BotCommand("/showtasks", "Просмотреть активные задачи"),// Просмотр текущих задач
                            new BotCommand("/showalltasks", "Просмотреть все задачи"),// Просмотр всех задач
                            new BotCommand("/removetask", "Удалить задачу"),// Удаление задачи
                            new BotCommand("/completetask", "Завершить задачу"),// Завершение задачи
                            new BotCommand("/report", "Получить отчет по задачам"),// Формирование отчета
                            new BotCommand("/find", "Найти задачу по названию")// Поиск задачи по имени
                        });



                        botClient.StartReceiving(handler, receiverOptions, ct.Token);

                        Console.WriteLine("Нажмите английскую \"A\" для остановки бота.");
                        var inputKey = Console.ReadKey();

                        if (inputKey.Key == ConsoleKey.A)
                        {
                            ct.Cancel();
                            throw new Exception($"\r\n{me.FirstName} остановлен!");
                        }
                        else
                            Console.WriteLine($"\r\n{me.FirstName} запущен!");

                        await Task.Delay(-1);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    handler.OnHandleUpdateStarted -= HandleUpdateStarted;
                    handler.OnHandleUpdateCompleted -= HandleUpdateCompleted;

                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Произошла непредвиденная ошибка: {0} {1} {2} {3}", ex.GetType(), ex.Message,
                    ex.StackTrace, ex.InnerException);
            }
        }
    }
}
