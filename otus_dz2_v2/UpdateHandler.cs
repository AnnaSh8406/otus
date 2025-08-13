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
        private readonly IUserService _userService;

        IUserService newToDoUser = new ToDoUser();

        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            botClient.SendMessage(update.Message.Chat, $"Получил '{update.Message.Text}'");

        
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
               
                        ToDoUser toDoUser = new ToDoUser();

                        toDoUser = newToDoUser.RegisterUser(update.Message.From.Id, update.Message.From.Username);
                 
                        Console.WriteLine($"Пользователь {toDoUser.TelegramUserName}, id {toDoUser.UserId} зарегестрирован");

                        break;
                        ///////
                    case string Contains when update.Message.Text.Contains("/addtask"):

                         ToDoService tdUaService = (ToDoService)toDoService;
                         ToDoUser? toDoUsera = new ToDoUser();

                        toDoUsera = newToDoUser.GetUser(update.Message.From.Id);

                        if (toDoUsera != null)
                        {
                            string addtask = "/addtask";
                            string ss = update.Message.Text;
                            ss = ss.Remove(0, addtask.Length);
                            ss = ss.Trim();
                            var chatId = update.Message.Chat;
                   
                             try
                             {
                                var newItem = toDoService.Add(toDoUsera, ss);
                                Console.WriteLine($"задача: {newItem.Name}, состояние: {newItem.State} {newItem.ID}");
                             }
                             catch (Exception ex)
                             {
                                botClient.SendMessage(update.Message.Chat, ex.Message);
                             }
                          }
                        else 
                        {
                            botClient.SendMessage(update.Message.Chat, "Вы не зарегестрированы. Вам доступны сл комманды:/start и /help");
                        }

                            break;
                    ////////
                    case "/showtasks":
                        tdUsService = (ToDoService)toDoService;
                        ToDoUser? toDoUsers = new ToDoUser();
                      
                        toDoUsers = newToDoUser.GetUser(update.Message.From.Id);
                   
                        var activeTasks = tdUsService.GetActiveByUserId(toDoUsers.UserId);
                        if (toDoUsers != null)
                        {
                            if (activeTasks.Count > 0)
                            {
                                try
                                {
                                    var taskList = activeTasks.Select((task, index) => $"Номер: {index + 1}, задача: {task.Name}, состояние: {task.State}");

                                    foreach (var item in taskList)
                                    {
                                        Console.WriteLine(item);
                                    }
                                }
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
                        ToDoUser? toDoUserr = new ToDoUser();
                        toDoUserr = newToDoUser.GetUser(update.Message.From.Id);
                     
                        string removetask = "/removetask";

                        var rs = update.Message.Text;
                        rs = rs.Remove(0, removetask.Length);
                        rs = rs.Trim();


                        if (toDoUserr != null)
                        {

                            int ii;
                            if (int.TryParse(rs, out ii))
                            {
                                ii = int.Parse(rs);
                            }
                            var taskRemove = tdUsService.GetActiveByUserId(toDoUserr.UserId).ToList();
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

                        ToDoUser? toDoUserc = new ToDoUser();
                        toDoUserc = newToDoUser.GetUser(update.Message.From.Id);
                      
                        var taskCompl = tdUsService.GetActiveByUserId(toDoUserc.UserId).ToList();
                        string compltask = "/completetask";

                        var rsc = update.Message.Text;
                        rsc = rsc.Remove(0, compltask.Length);
                        rsc = rsc.Trim();
                        if (toDoUserc != null)
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

                        ToDoUser? toDoUseral = new ToDoUser();
                        toDoUseral = newToDoUser.GetUser(update.Message.From.Id);

                        var allTasks = tdUsService.GetAllByUserId(toDoUseral.UserId);
                        if (toDoUseral != null)
                        {
                            if (allTasks.Count > 0)
                            {
                                try
                                {
                                    var taskList = allTasks.Select((task, index) => $"{index + 1},{task.Name}, состояние: {task.State}");

                                    foreach (var item in taskList)
                                    {
                                        Console.WriteLine(item);
                                    }
                                }
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

                    ////////
                    case "/info":
                        ToDoUser? toDoUseri = new ToDoUser();
                        toDoUseri = newToDoUser.GetUser(update.Message.From.Id);
     
                        if (toDoUseri != null)
                        {
                            botClient.SendMessage(update.Message.Chat, $"{Vers} {localDate}");
                        }
                        else
                        {
                            botClient.SendMessage(update.Message.Chat, "Вы не зарегестрированы. Вам доступны сл комманды:/start и /help");
                        }
                        break;

                    case "/exit":
                        ToDoUser? toDoUsere = new ToDoUser();
                        toDoUsere = newToDoUser.GetUser(update.Message.From.Id);
   
                        if (toDoUsere != null)
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
