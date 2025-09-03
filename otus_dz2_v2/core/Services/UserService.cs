using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using otus_dz2_v2.Core.DataAccess;
using otus_dz2_v2.Core.Entities;

namespace otus_dz2_v2.Core.Services
{

    public class UserService : IUserService
    {

        private readonly IUserRepository userRep;

        public async Task<ToDoUser> RegisterUserAsync(long telegramUserId, string telegramUsername, CancellationToken cancellationToken)
        {
            ToDoUser user = new ToDoUser(telegramUserId, telegramUsername);
            await userRep.AddAsync(user, cancellationToken);
            return user;

        }


        public async Task<ToDoUser?> GetUserAsync(long telegramUserId, CancellationToken cancellationToken)
        {


            ToDoUser user = await userRep.GetUserByTelegramUserIdAsync(telegramUserId, cancellationToken);
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

