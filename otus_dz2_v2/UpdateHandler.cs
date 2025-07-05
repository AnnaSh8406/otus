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

            IUserService newToDoUser = new ToDoUser();
            ToDoUser toDoUser = new ToDoUser();
        //int MaxCount = Convert.ToInt32(Console.ReadLine());
        //int MaxTaskLenght = Convert.ToInt32(Console.ReadLine());
        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            botClient.SendMessage(update.Message.Chat, $"Получил '{update.Message.Text}'");

            var useri = update.Message.From.Id;
            //tdUs.TelegramUserId = 2222;

            

            var user= new ToDoUser ();
           // ToDoItem tdItem = new ToDoItem();



            List<ToDoItem> tdList = new List<ToDoItem>();
            string Vers = "Версия 1.3";
            DateTime localDate = DateTime.Now;

            


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
                        "/addtask + название задачи - добавить задачу в список дел\n" +
                        "/showtasks - показать добавленные активные задачи\n" +
                        "/removetask + номер удаляемой задачи - удалить задачу\n" +
                        "/completetask + номер - завершить активную задачу\n" +
                        "/showalltasks - показать все задачи пользователя");
                        break;

                    case "/start":
                        // botClient.SendMessage(update.Message.Chat, $"Получил '{update.Message.From.Id}");
                        // botClient.SendMessage(update.Message.Chat, $"Получил '{update.Message.From.Username}");
                        // ToDoUser t = new ToDoUser();
                        //ToDoUser newTodoUser = new ToDoUser();
                        // //newTodoUser.TelegramUserId = update.Message.From.Id;
                        // IUserService newUs = new ToDoUser();
                        //newUs.GetUser(update.Message.From.Id);
                        // tdUs = newToDoUser.RegisterUser(update.Message.From.Id, update.Message.From.Username);

                        ToDoUser toDoUser = new ToDoUser();
                        toDoUser = newToDoUser.RegisterUser(update.Message.From.Id, update.Message.From.Username);
                        toDoUser.RegistereAt=DateTime.Now;
                        toDoUser.UserId= Guid.NewGuid();

                        Console.WriteLine($"Пользователь {toDoUser.TelegramUserName}, id {toDoUser.UserId} зарегестрирован");

                        break;
                        ///////
                    case string Contains when update.Message.Text.Contains("/addtask"):

                        //ToDoService tdUsService = (ToDoService)toDoService;
                        ToDoUser toDoUserf = (ToDoUser)newToDoUser;
                        var existUser = toDoUserf.TelegramUserId;
                        if (existUser != 0)
                        {

                            string addtask = "/addtask";
                            string ss = update.Message.Text;
                            ss = ss.Remove(0, addtask.Length);
                            ss = ss.Trim();
                            // toDoUser.UserId = Guid.NewGuid();
                            //toDoUser.TelegramUserName = update.Message.From.Username;
                            // var taskName = toDoUser.TelegramUserName;
                            //Guid dd= Guid.NewGuid();
                            var chatId = update.Message.Chat;
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



                                Console.WriteLine($"задача: {newItem.Name}, состояние: {newItem.State}");
                            }
                            catch (Exception ex)
                            {
                                botClient.SendMessage(update.Message.Chat, ex.Message);
                            }
                            //it.Complete();

                            //{ ID=1,Name=ss,CreatedAt= DateTime.Now; };
                        }
                        else 
                        {
                            botClient.SendMessage(update.Message.Chat, "Вы не зарегестрированы. Вам доступны сл комманды:/start и /help");
                        }

                            break;
                        ////////
                    case "/showtasks":

                        ToDoService tdUsService = (ToDoService)toDoService;

                        ToDoUser toDoUsers = (ToDoUser)newToDoUser;
                        var existUsers = toDoUsers.TelegramUserId;

                        var activeTasks = tdUsService.GetActiveByUserId(user.UserId);
                        if (existUsers != 0)
                        {
                            if (activeTasks.Count > 0)
                            {
                                try
                                {
                                    var taskList = activeTasks.Select((task, index) => $"{index + 1},{task.Name}");

                                    // botClient.SendMessage(chatId, taskList);
                                    foreach (var item in taskList)
                                    {
                                        Console.WriteLine(item);
                                    }
                                }
                                //Console.WriteLine(taskList);
                                catch (Exception ex)
                                {
                                    botClient.SendMessage(update.Message.Chat, ex.Message);
                                }
                            }
                            else
                            {
                                botClient.SendMessage(update.Message.Chat, "Нет активных задач");
                            }
                        }
                        else
                        {
                            botClient.SendMessage(update.Message.Chat, "Вы не зарегестрированы. Вам доступны сл комманды:/start и /help");
                        }
                        break;
                        ///////
                    case string Contains when update.Message.Text.Contains("/removetask"):
                        tdUsService = (ToDoService)toDoService;

                        ToDoUser toDoUserr = (ToDoUser)newToDoUser;
                        var existUserr = toDoUserr.TelegramUserId;
                        string removetask = "/removetask";

                        var rs = update.Message.Text;
                        rs = rs.Remove(0, removetask.Length);
                        rs=  rs.Trim();
                        

                        if (existUserr != 0)
                        {
                            
                            int ii;
                            if (int.TryParse(rs, out ii))
                            {
                                ii = int.Parse(rs);
                            }
                          

                                var taskRemove = tdUsService.GetActiveByUserId(user.UserId).ToList();
                            int taskIndex = ii;

                            if (taskIndex > 0 && taskIndex <= taskRemove.Count)
                            {
                                try
                                {
                                    tdUsService.Delete(taskRemove[taskIndex - 1].ID);

                                    botClient.SendMessage(update.Message.Chat, $"Задача под номером {taskIndex} удалена");
                                }
                                catch (Exception ex)
                                {
                                    botClient.SendMessage(update.Message.Chat, ex.Message);
                                }
                            }
                            else
                            {
                                
                                botClient.SendMessage(update.Message.Chat, "Неверный номер задачи");
                            }
                        }
                        else
                        {
                            botClient.SendMessage(update.Message.Chat, "Вы не зарегестрированы. Вам доступны сл комманды:/start и /help");
                        }
                        break;

                        ////////
                    case string Contains when update.Message.Text.Contains("/completetask"):
                        tdUsService = (ToDoService)toDoService;

                        ToDoUser toDoUserc = (ToDoUser)newToDoUser;
                        var existUserc = toDoUserc.TelegramUserId;

                        var taskCompl = tdUsService.GetActiveByUserId(user.UserId).ToList();
                        string compltask = "/completetask";

                        var rsc = update.Message.Text;
                        rsc = rsc.Remove(0, compltask.Length);
                        rsc = rsc.Trim();
                        if (existUserc != 0)
                        {

                            int iic;
                            if (int.TryParse(rsc, out iic))
                            {
                                iic = int.Parse(rsc);
                            }


                            int taskIndexComp = iic;
                            if (taskIndexComp > 0 && taskIndexComp <= taskCompl.Count)
                            {
                                try
                                {

                                    tdUsService.MarkCompleted(taskCompl[taskIndexComp - 1].ID);

                                    botClient.SendMessage(update.Message.Chat, $"Задача под номером {taskIndexComp} завершена");


                                }
                                catch (Exception ex)
                                {
                                    botClient.SendMessage(update.Message.Chat, ex.Message);
                                }
                             }
                            else
                            {

                            botClient.SendMessage(update.Message.Chat, "Неверный номер задачи");
                            }

                        }
                        else
                        {
                            botClient.SendMessage(update.Message.Chat, "Вы не зарегестрированы. Вам доступны сл комманды:/start и /help");
                        }
                            break;
                        /////////
                    case "/showalltasks":

                        tdUsService = (ToDoService)toDoService;

                        ToDoUser toDoUsera = (ToDoUser)newToDoUser;
                        var existUsera = toDoUsera.TelegramUserId;
                        if (existUsera != 0)
                        {

                            var allTasks = tdUsService.GetAllByUserId(user.UserId);
                        if (allTasks.Count > 0)
                        {
                            var taskList = allTasks.Select((task, index) => $"Номер: {index + 1}, задача: {task.Name}, состояние: {task.State}");
                              // botClient.SendMessage(chatId, taskList);
                            foreach (var item in taskList)
                            {
                                Console.WriteLine($"{item}") ;

                            }
                            //Console.WriteLine(taskList);
                        }
                            else
                            {
                                throw new Exception("Нет активных задач");
                            }
                        }
                        else
                        {
                            botClient.SendMessage(update.Message.Chat, "Вы не зарегестрированы. Вам доступны сл комманды:/start и /help");
                        }
                        break;

                        ////////
                    case "/info":

                        ToDoUser toDoUseri = (ToDoUser)newToDoUser;
                        var existUseri = toDoUseri.TelegramUserId;
                        if (existUseri != 0)
                        {
                            botClient.SendMessage(update.Message.Chat, $"{Vers} {localDate}");
                        }
                        else
                        {
                            botClient.SendMessage(update.Message.Chat, "Вы не зарегестрированы. Вам доступны сл комманды:/start и /help");
                        }
                        break;

                    case "/exit":
                        ToDoUser toDoUsere = (ToDoUser)newToDoUser;
                        var existUsere = toDoUsere.TelegramUserId;
                        if (existUsere != 0)
                        {
                            Environment.Exit(0);
                        }
                        else
                        {
                            botClient.SendMessage(update.Message.Chat, "Вы не зарегестрированы. Вам доступны сл комманды:/start и /help");
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
