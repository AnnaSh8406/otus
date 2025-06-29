using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otus.ToDoList.ConsoleBot.Types;
using Otus.ToDoList.ConsoleBot;
using static otus_dz2_v2.Program;

namespace otus_dz2_v2
{ 
    internal class UpdateHandler : IUpdateHandler
    {
        
        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            botClient.SendMessage(update.Message.Chat, $"Получил '{update.Message.Text}'");


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

                    default:
                        Console.WriteLine(" введите корректную команду");
                        break;

                }
            }
        }
        public void ToDoUser(Guid UserId, long TelegramUserId, string TelegramUserName, DateTime RegisteredAt)
        {

            IUserService u = new UserService();
             u.GetUser(telegramUserId);
            // u.RegisterUser(TelegramUserId.)
           

        }
    }



}
