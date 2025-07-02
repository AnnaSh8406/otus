using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace otus_dz2_v2
{
    
    public interface IUserService
    {

         ToDoUser RegisterUser(long telegramUserId, string telegramUserName);
        ToDoUser? GetUser(long telegramUserId);

       

    }
    
    public class ToDoUser : IUserService

    {
        
        public Guid UserId { get; set; }= Guid.NewGuid();


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
            TelegramUserId = telegramUserId++;
            ConsoleBotClient botClient = new ConsoleBotClient();
            Update update = new Update();
            ToDoUser newTodoUser = new ToDoUser();
            newTodoUser.TelegramUserId = telegramUserId;

            UserId = Guid.NewGuid();
            //telegramUserId = update.Message.From.Id;
            return newTodoUser;
        }


        ToDoUser IUserService.RegisterUser(long telegramUserId, string telegramUserName)
        {
            //  throw new NotImplementedException();
            TelegramUserId = telegramUserId++;

            UserId= Guid.NewGuid();
            ConsoleBotClient botClient = new ConsoleBotClient();
            Update update = new Update();
            ToDoUser newTodoUser = new ToDoUser();
            newTodoUser.TelegramUserId = telegramUserId;
            newTodoUser.TelegramUserName = telegramUserName;
            newTodoUser.UserId=UserId;
            return newTodoUser;
        }

        
        

    }
}
