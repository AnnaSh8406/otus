using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using static otus_dz2_v2.Program; 
namespace otus_dz2_v2
{ 
    public class UpdateHandler : IUpdateHandler
    {
        
            IToDoService toDoService = new ToDoService();
             ToDoService tdUsService = new ToDoService();
        //int MaxCount = Convert.ToInt32(Console.ReadLine());
        //int MaxTaskLenght = Convert.ToInt32(Console.ReadLine());

        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            botClient.SendMessage(update.Message.Chat, $"Получил '{update.Message.Text}'");

            var useri = update.Message.From.Id;
            IUserService newToDoUser = new ToDoUser();
            ToDoUser tdUs = new ToDoUser();
            //tdUs.TelegramUserId = 2222;

            

            var user= new ToDoUser ();
           // ToDoItem tdItem = new ToDoItem();



            List<ToDoItem> tdList = new List<ToDoItem>();
           
            

            if (update.Message != null)
            {
                switch (update.Message.Text)
                {
                    case "/help":
                        Console.WriteLine(" Краткое описание:\n" +
                        "/start - начало работы\n" +
                        "/help - краткое описание доступных комманд\n" +
                        "/info - информация о версии и дате запуска кода\n" +
                        "/exit - завершение работы\n" +
                        "/addtask - добавить задачу в сптсок дел\n" +
                        "/showtasks - показать ранее добавленные задачи\n" +
                        "/removetask - удалить задачу");
                        break;

                    case "/start":
                        // botClient.SendMessage(update.Message.Chat, $"Получил '{update.Message.From.Id}");
                        // botClient.SendMessage(update.Message.Chat, $"Получил '{update.Message.From.Username}");
                        // ToDoUser t = new ToDoUser();
                        //ToDoUser newTodoUser = new ToDoUser();
                        // //newTodoUser.TelegramUserId = update.Message.From.Id;
                        // IUserService newUs = new ToDoUser();
                        //newUs.GetUser(update.Message.From.Id);
                        tdUs = newToDoUser.RegisterUser(update.Message.From.Id, update.Message.From.Username);

                        tdUs.RegistereAt=DateTime.Now;
                        tdUs.UserId= Guid.NewGuid();

                        Console.WriteLine($"Пользователь {tdUs.TelegramUserName}, id {tdUs.UserId} зарегестрирован");

                        break;

                    case string Contains when update.Message.Text.Contains("/addtask"):
                        string addtask = "/addtask";
                        string ss=update.Message.Text;
                        ss=ss.Remove(0,addtask.Length);
                        ToDoUser toDoUser = new ToDoUser();
                        toDoUser.UserId = Guid.NewGuid();
                        toDoUser.TelegramUserName = update.Message.From.Username;
                        var taskName= toDoUser.TelegramUserName;
                        //Guid dd= Guid.NewGuid();
                        var chatId=update.Message.Chat;
                        //Guid UserId= Guid.NewGuid();
                        //ToDoItem it = new ToDoItem(Guid.NewGuid(), ss);
                        //ToDoItem t = new ToDoItem(Guid.NewGuid(), ss);

                        /*TaskU taskU = new TaskU();
                        taskU.AddTask(tdUs, ss);

                        var newItem = toDoService.Add(user, taskName);
                        botClient.SendMessage(chatId, newItem.Name);
                        */
                        try
                        {
                            var newItem = toDoService.Add(user, ss);



                            Console.WriteLine($"задание {newItem.Name}, состояние {newItem.State}");
                        }
                        catch (Exception ex) { 
                            botClient.SendMessage(update.Message.Chat, ex.Message);
                        }
                        //it.Complete();

                            //{ ID=1,Name=ss,CreatedAt= DateTime.Now; };


                         break;

                    case "/showtasks":

                        ToDoService tdUsService = (ToDoService)toDoService;
                        var activeTasks = tdUsService.GetActiveByUserId(user.UserId);
                        if (activeTasks.Count > 0)
                        {
                            var taskList = activeTasks.Select((task, index) => $"{index + 1},{task.Name}");

                            // botClient.SendMessage(chatId, taskList);
                            foreach (var item in taskList)
                            {
                                Console.WriteLine(item);
                            }
                            //Console.WriteLine(taskList);
                        }
                        else 
                        {
                            throw new Exception("Нет активных задач");
                        }
                            break;

                    case string Contains when update.Message.Text.Contains("/removetask"):
                        tdUsService = (ToDoService)toDoService;

                        string removetask = "/removetask";

                        var rs = update.Message.Text;
                        rs = rs.Remove(0, removetask.Length);
                        rs=  rs.Trim();
                        int ii;
                        ii=int.Parse(rs);

                        var taskRemove = tdUsService.GetActiveByUserId(user.UserId).ToList();
                        int taskIndex  = ii;
                        if (taskIndex > 0 && taskIndex <= taskRemove.Count)
                        {
                            tdUsService.Delete(taskRemove[taskIndex - 1].ID);
                        }
                        else
                        {
                            throw new Exception("Неверный номер задачи");
                        }
                            break;
                    case string Contains when update.Message.Text.Contains("/completetask"):
                        tdUsService = (ToDoService)toDoService;
                        var taskCompl = tdUsService.GetActiveByUserId(user.UserId).ToList();
                        string compltask = "/completetask";

                        var rsc = update.Message.Text;
                        rsc = rsc.Remove(0, compltask.Length);
                        rsc = rsc.Trim();
                        int iic;

                        iic = int.Parse(rsc);
                        int taskIndexComp = iic;
                        tdUsService.MarkCompleted(taskCompl[taskIndexComp - 1].ID);

                        break;
                    case "/showalltasks":

                        tdUsService = (ToDoService)toDoService;
                        var allTasks = tdUsService.GetAllByUserId(user.UserId);
                        if (allTasks.Count > 0)
                        {
                            var taskList = allTasks.Select((task, index) => $"{index + 1},{task.Name}");

                            // botClient.SendMessage(chatId, taskList);
                            foreach (var item in taskList)
                            {
                                Console.WriteLine(item);
                            }
                            //Console.WriteLine(taskList);
                        }
                        else
                        {
                            throw new Exception("Нет активных задач");
                        } 
                        break;
                    default:
                        Console.WriteLine(" введите корректную команду");
                        break;

                }
            }
        } 

        private readonly IToDoService _toDoService;
        public UpdateHandler(IToDoService toDoService  )
        {
            _toDoService = toDoService;
        }

        public UpdateHandler()
        {
            
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
