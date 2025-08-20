using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using Microsoft.VisualBasic;



namespace otus_dz2_v2
{

    public class Program
    {
        public static int maxTasks;
        public static int maxTaskLenght;


        private static int ParseAndValidateInt(string? str, int min, int max)
        {
            var isNumber = int.TryParse(str, out var number);
            if (!isNumber || number < min || number > max)
                throw new Exception("Превышено максимальное кол-во задач/длина задачи или некорректное значение"); ;
            return number;
        }
        static void Main(string[] args )
            {
            bool validInputLen = false;

            while (!validInputLen)
            {
                validInputLen = true;
                try
                {
                    Console.WriteLine("Введите количество задач");
                    maxTasks = ParseAndValidateInt(Console.ReadLine(), 1, 100);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");
                      validInputLen = false;
                }
            }
            bool validInputLenTask = false;
            while (!validInputLenTask)
            {
                validInputLenTask = true;
                try
                {

                    Console.WriteLine("Введите длину задачи");
                    maxTaskLenght = ParseAndValidateInt(Console.ReadLine(), 1, 100);
                }
                catch
                {
                    Console.WriteLine($"Не корректный формат числа");
                    validInputLenTask = false;
                }
            }

            Console.WriteLine("Ведите одну из команд:/start, /help, /info, /exit, /addtask имя задачи, /showtasks, /removetask номер задачи, /completetask номер задачи, /showalltasks");
            var botClient = new ConsoleBotClient();

             
            IUpdateHandler t=new UpdateHandler();
             botClient.StartReceiving(t);
         

        }
    }
}
