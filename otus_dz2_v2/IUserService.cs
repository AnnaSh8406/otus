using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace otus_dz2_v2
{
    public interface IUserService
    {

         ToDoUser RegisterUser(long telegramUserId, string telegramUserName);
        ToDoUser? GetUser(long telegramUserId);

       

    }
    public class  UserService : IUserService

    {

        //private long telegramUserId;

        
           ToDoUser? IUserService.GetUser( long telegramUserId )
         {
            //  throw new NotImplementedException();
            ConsoleBotClient botClient = new ConsoleBotClient();   
            Update update = new Update();
             
            // botClient.SendMessage(update.Message.Chat, $"{update.Message.Id}");
              telegramUserId= update.Message.Id;
             
            return telegramUserId;
        }
        /*
        void IUserService.Myclass2()
        {
          //  throw new NotImplementedException();
        }*/

        ToDoUser IUserService.RegisterUser(long telegramUserId, string telegramUserName)
         {
              throw new NotImplementedException();
         }
         

    }
}
