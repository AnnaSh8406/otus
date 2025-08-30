using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using otus_dz2_v2.core.DataAccess;
using otus_dz2_v2.core.Entities;

namespace otus_dz2_v2.core.Services
{

    public class UserService : IUserService
    {

        private readonly IUserRepository userRep;

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            ToDoUser user = new ToDoUser(telegramUserId, telegramUserName);
            userRep.Add(user);
            return user;
        }

        public ToDoUser? GetUser(long telegramUserId)
        {
            ToDoUser user = userRep.GetUserByTelegramUserId(telegramUserId);
            if (user != null)
                return user;

            return null;
        }

        public UserService(IUserRepository userRep)
        {
            this.userRep = userRep;
        }

    }

}
 
