using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using Microsoft.VisualBasic;
using otus_dz2_v2.Core.DataAccess;
using otus_dz2_v2.Core.Services;
using otus_dz2_v2.Infrastructure.DataAccess;
using otus_dz2_v2.TelegramBot;
using System.Numerics;



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

        static void Main(string[] args)
        {

            while (true)
            {

                try
                {
                    int maxTasks = SetMaxTasks();
                    int taskLength = SetMaxLengthNameTasks();


                    var botClient = new ConsoleBotClient();
                    IUserRepository userRepository = new InMemoryUserRepository();
                    IUserService userService = new UserService(userRepository);
                    IToDoRepository toDoRepository = new InMemoryToDoRepository();
                    IToDoService toDoService = new ToDoService(maxTasks, taskLength, toDoRepository);
                    var handler = new UpdateHandler(botClient, userService, toDoService, toDoRepository); // Обработчик обновлений

                    handler.OnHandleUpdateStarted += HandleUpdateStarted;
                    handler.OnHandleUpdateCompleted += HandleUpdateCompleted;

                    // Создаем токен отмены
                    var cts = new CancellationTokenSource();


                    Console.CancelKeyPress += (_, _) =>
                    {
                        cts.Cancel();
                        handler.OnHandleUpdateStarted -= HandleUpdateStarted;
                        handler.OnHandleUpdateCompleted -= HandleUpdateCompleted;
                        cts.Dispose();
                    };


                    try
                    {
                        botClient.StartReceiving(handler, cts.Token);

                        Console.WriteLine("Бот запущен...");
                    }
                    finally
                    {
                        handler.OnHandleUpdateStarted -= HandleUpdateStarted;
                        handler.OnHandleUpdateCompleted -= HandleUpdateCompleted;
                    }

                    cts.Cancel();

                    break;
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
                    Console.WriteLine("Произошла непредвиденная ошибка", ex.Message);

                }




            }
        }
    }
}
