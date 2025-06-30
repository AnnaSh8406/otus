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

    public class ToDoUser : IUserService

    {
        public Guid UserId { get; set; }


        public long TelegramUserId { get; set; }
        public string TelegramUserName { get; set; }
        public DateTime RegistereAt { get; set; }
       
        /* public ToDoUser(long telegramUserId, string telegramUserName)
        { 
        TelegramUserId = telegramUserId;

            TelegramUserName = telegramUserName;
        } 
        */
        ToDoUser? IUserService.GetUser(long telegramUserId)
        {
            //  throw new NotImplementedException();
            //ConsoleBotClient botClient = new ConsoleBotClient();   
            //Update update = new Update();

            //botClient.SendMessage(update.Message.Chat, $"{update.Message.Id}");
            // telegramUserId= update.Message.Id;
            telegramUserId = telegramUserId++;
            ConsoleBotClient botClient = new ConsoleBotClient();
            Update update = new Update();
            ToDoUser newTodoUser = new ToDoUser();
            newTodoUser.TelegramUserId = telegramUserId;
            //telegramUserId = update.Message.From.Id;
            return new ToDoUser();
        }


        ToDoUser IUserService.RegisterUser(long telegramUserId, string telegramUserName)
        {
            //  throw new NotImplementedException();
            telegramUserId = telegramUserId++;

            ConsoleBotClient botClient = new ConsoleBotClient();
            Update update = new Update();
            ToDoUser newTodoUser = new ToDoUser();
            newTodoUser.TelegramUserId = telegramUserId;
            newTodoUser.TelegramUserName = telegramUserName;
            return new ToDoUser();
        }



    }
}