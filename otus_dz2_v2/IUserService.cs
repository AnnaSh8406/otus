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

    
}
