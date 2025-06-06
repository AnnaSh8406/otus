using System;
using System.Text;
namespace otus_dz2_v2
{
    internal class Program
    {
        public class MyClass
        {
            public static string? UserName;
        }
        static void Main(string[] args)
        {
            List<string> ToDoList = new List<string>();
            string Vers = "Версия 1.2";
            DateTime localDate= DateTime.Now;
            Console.WriteLine("Для начала работы введите одну из следующих комманд: /strat, /help, /info, /exit");
            string CmdEcho = "/echo";

            string UserCommand=Console.ReadLine();
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
                        "/echo + слово/фраза - возврат введенного пользователем слова, доступно только после выполнения /start");
                       /* "/addtask - добавить задачу в сптсок дел\n" +
                        "/showtasks - показать ранее добавленные задачи\n" +
                        "/removetask - удалить задачу");*/
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
                        /*
                    case "/addtask":
                        Console.WriteLine($"{MyClass.UserName} Пожалуйста, введите описание задачи. Для выхода  из команды введите символ /");
                        string UserTask = Console.ReadLine();
                        while (UserTask != "/")
                        {
                            ToDoList.Add(UserTask);
                            Console.WriteLine($"{MyClass.UserName} задача {UserTask} добавлена, введите след. задачу");
                            UserTask = Console.ReadLine();
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
                            Console.WriteLine("Задача удалена");
                        }
                     
                        else
                        {
                            Console.WriteLine($"{MyClass.UserName} Такого номера не существует");
                        }
                        
                            break;
                        */
                    default:
                        Console.WriteLine($"{MyClass.UserName} введите клрректную команду");
                        break;

                }
                UserCommand = Console.ReadLine();
            }
            while (UserCommand != "/exit");
            Environment.Exit(0);

            

        }
    }
}
