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
    internal class UpdateHandler : IUpdateHandler
    {
        
        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            botClient.SendMessage(update.Message.Chat, $"Получил '{update.Message.Text}'");


            // var update = new Update();

            //botClient.SendMessage(update.Message.Chat, "Для начала работыt");
             
            


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
                        //newTodoUser.TelegramUserId = update.Message.From.Id;
                        IUserService newUs = new ToDoUser();
                        newUs.GetUser(update.Message.From.Id);
                           





                        break;

                    default:
                        Console.WriteLine(" введите корректную команду");
                        break;

                }
            }
        }
      
    }



}
