using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace otus_dz2_v2
{
    internal class Program
    {
        public class MyClass
        {
            public static string? UserName;
        }
        public class TaskCountLimitException : Exception
        {
            public int ErrorCode { get; } // Дополнительное свойство
            public TaskCountLimitException() { }
            public TaskCountLimitException(string message) : base(message) { }
            public TaskCountLimitException(string message, Exception innerException) : base(message, innerException) { }
            public TaskCountLimitException(string message, int errorCode) : this(message)
            {
                ErrorCode = errorCode;
            }
        }
        public class DuplicateTaskException : Exception
        {
            public int ErrorCode { get; } // Дополнительное свойство
            public DuplicateTaskException() { }
            public DuplicateTaskException(string message) : base(message) { }
            public DuplicateTaskException(string message, Exception innerException) : base(message, innerException) { }
            public DuplicateTaskException(string message, int errorCode) : this(message)
            {
                ErrorCode = errorCode;
            }
        }

        public class TaskLengthLimitException : Exception
        {
            public int ErrorCode { get; } // Дополнительное свойство
            public TaskLengthLimitException() { } 
            public TaskLengthLimitException(string message) : base(message) { }
            public TaskLengthLimitException(string message, Exception innerException) : base(message, innerException) { }
            public TaskLengthLimitException(string message, int errorCode) : this(message)
            {
                ErrorCode = errorCode;
            }
        }
        public class StringValidator
        {
            public static bool ValidateString(string? UserCommand)
            {
                if (string.IsNullOrEmpty(UserCommand) || UserCommand.Trim() == "")
                {
                    throw new ArgumentException("Строка не должна быть null, пустой или содержать только пробел.");
                }
                return true;
            }
        }
        public class NumberValidator
        {
            public static bool ParseAndValidateInt(int taskCountLimit, int min, int max)
            {
                if (taskCountLimit >= min && taskCountLimit <= max)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public static bool ParseAndValidateIntLen(int taskLengthLimit, int min, int max)
            {
                if (taskLengthLimit >= min && taskLengthLimit <= max)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

            static void Main(string[] args)
            {
                int min = 0;
                int max = 100;

                List<string> ToDoList = new List<string>();
                string Vers = "Версия 1.3";
                DateTime localDate = DateTime.Now;
 
            int taskCountLimit = 0;
            bool validInputCnt = false;

            while (!validInputCnt)
            {
                validInputCnt = true;
                Console.WriteLine("Введите максимально допустимое количество задач от 1 до 100 шт");
                try
                {
                    taskCountLimit = Convert.ToInt32(Console.ReadLine());
                        if  (!NumberValidator.ParseAndValidateInt(taskCountLimit, min, max)) 
                        {
                        throw new ArgumentException( "Введите максимально допустимое количество задач от 1 до 100 шт");
                        }
                }
                catch (ArgumentException ex)  
                    {
                    Console.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");
                    Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                    Console.WriteLine($"Трассировка стека: {ex.InnerException}");
                    
                    validInputCnt = false;
                    }
                catch (FormatException ex)
                    {
                        Console.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");
                        Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                    validInputCnt = false;
                }
            }


             int taskLengthLimit = 0;
            bool validInputLen = false;

            while (!validInputLen)
            {
                validInputLen = true;
                Console.WriteLine("Введите максимально допустимое количество символов от 1 до 100 шт");
                try
                {
                    taskLengthLimit = Convert.ToInt32(Console.ReadLine());
                    if (!NumberValidator.ParseAndValidateIntLen(taskLengthLimit, min, max))
                    {
                        throw new ArgumentException(  "Введите максимально допустимое длину задач от 1 до 100 символов");
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");
                    Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                    Console.WriteLine($"InnerException: {ex.InnerException}");
                    validInputLen = false;
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");
                    Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                    validInputLen = false;
                }
            }


           
                Console.WriteLine("Для начала работы введите одну из следующих комманд: /strat, /help, /info, /addtask, /showtasks, /removetask, /exit");
                string CmdEcho = "/echo";

                string UserCommand = Console.ReadLine();
            try
            {
                StringValidator.ValidateString(UserCommand);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");

                Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                Console.WriteLine($"InnerException: {ex.InnerException}");
            }
        
                do
                {



                    switch (UserCommand.ToLower())
                    {
                        case "/start":
                            Console.WriteLine("Введите Ваше имя");
                            MyClass.UserName = Console.ReadLine();
                            bool IsUserName = string.IsNullOrEmpty(MyClass.UserName);

                            while (IsUserName == true || MyClass.UserName == " ")
                            {
                                Console.WriteLine("Введите корректное имя");
                                MyClass.UserName = Console.ReadLine();
                                IsUserName = string.IsNullOrEmpty(MyClass.UserName);
                            }
                            Console.WriteLine($"Привет {MyClass.UserName} =)");
                            break;
                        case "/help":
                            Console.WriteLine($"{MyClass.UserName} Краткое описание:\n" +
                            "/start - начало работы\n" +
                            "/help - краткое описание доступных комманд\n" +
                            "/info - информация о версии и дате запуска кода\n" +
                            "/exit - завершение работы\n" +
                            "/echo + слово/фраза - возврат введенного пользователем слова, доступно только после выполнения /start\n" +
                           "/addtask - добавить задачу в сптсок дел\n" +
                            "/showtasks - показать ранее добавленные задачи\n" +
                            "/removetask - удалить задачу");
                            break;

                        case "/info":
                            Console.WriteLine($"{MyClass.UserName} {Vers} {localDate}");
                            break;

                        case string Contains when UserCommand.Contains("/echo") && string.IsNullOrEmpty(MyClass.UserName) == false:
                            string Cont = UserCommand.Remove(0, CmdEcho.Length);
                            Console.WriteLine(Cont.Trim());
                            break;

                        case string Contains when UserCommand.Contains("/echo") && string.IsNullOrEmpty(MyClass.UserName) == true:
                            Console.WriteLine("Сначала запустите команду /start");
                            break;

                        case "/addtask":
                            try
                            {

                                if (ToDoList.Count < taskCountLimit)
                                {

                                    Console.WriteLine($"{MyClass.UserName} Пожалуйста, введите описание задачи");
                                    string UserTask = Console.ReadLine();
                                    int taskLength = UserTask.Length;
                                    if (taskLength <= taskLengthLimit)
                                    {
                                        if (!ToDoList.Contains(UserTask))
                                        {
                                            ToDoList.Add(UserTask);

                                            Console.WriteLine($"{MyClass.UserName} задача {UserTask} добавлена, введите след. команду");
                                        }
                                        else
                                        {
                                            throw new DuplicateTaskException($"Задача {UserTask} уже существует. Введите след команду");
                                        }
                                    }
                                    else
                                    {
                                        throw new TaskLengthLimitException($"Длина задачи {taskLength} превышает максимально допустимое значение {taskLengthLimit}. Введите след команду");
                                    }



                                }

                            }
                            catch (TaskLengthLimitException ex)
                            {
                                Console.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");

                                Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                                Console.WriteLine($"InnerException: {ex.InnerException}");
                            }

                            catch (DuplicateTaskException ex)
                            {
                                Console.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");

                                Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                                Console.WriteLine($"InnerException: {ex.InnerException}");
                            }


                            catch (TaskCountLimitException ex)
                            {
                                Console.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");

                                Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                                Console.WriteLine($"InnerException: {ex.InnerException}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");

                                Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                                Console.WriteLine($"InnerException: {ex.InnerException}");
                            }



                            break;

                        case "/showtasks":
                            if (ToDoList.Count > 0)
                            {
                                Console.WriteLine($"{MyClass.UserName} Вот Ваш список задач");
                                for (int i = 0; i < ToDoList.Count; i++)
                                {
                                    Console.WriteLine((i + 1) + " - " + ToDoList[i]);
                                }
                            }
                            else
                            {
                                Console.WriteLine($"{MyClass.UserName} В списке нет задач");
                            }
                            break;

                        case "/removetask":
                            if (ToDoList.Count > 0)
                            {
                                Console.WriteLine($"{MyClass.UserName} Вот Ваш список задач:\n" +
                                    $"Введите номер задачи, которую хотите удалить:");
                                for (int i = 0; i < ToDoList.Count; i++)
                                {
                                    Console.WriteLine((i + 1) + " - " + ToDoList[i]);
                                }
                            }
                            else
                            {
                                Console.WriteLine($"{MyClass.UserName} В списке еще нет задач");
                            }
                            int UserTaskNum = int.Parse(Console.ReadLine());
                            UserTaskNum--;

                            if (UserTaskNum >= 0 && UserTaskNum < ToDoList.Count)
                            {
                                ToDoList.RemoveAt(UserTaskNum);
                                Console.WriteLine("Задача удалена. Введите след команду");
                            }

                            else
                            {
                                Console.WriteLine($"{MyClass.UserName} Такого номера не существует");
                            }


                            break;

                        default:
                            Console.WriteLine($"{MyClass.UserName} введите корректную команду");
                            break;

                    }
                    UserCommand = Console.ReadLine();
                    try
                    {
                        StringValidator.ValidateString(UserCommand);
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");

                        Console.WriteLine($"Трассировка стека: {ex.StackTrace}");
                        Console.WriteLine($"InnerException: {ex.InnerException}");
                    }



            } while (UserCommand != "/exit");
                Environment.Exit(0);


             

        }
    }
}
