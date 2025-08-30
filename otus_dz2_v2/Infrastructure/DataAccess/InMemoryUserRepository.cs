using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.core.DataAccess;
using otus_dz2_v2.core.Entities;

namespace otus_dz2_v2.Infrastructure.DataAccess
{
    public class InMemoryUserRepository : IUserRepository
    {
        public InMemoryUserRepository()
        {
            users = new List<ToDoUser>();
        }

        private readonly List<ToDoUser> users;
        public ToDoUser? GetUser(Guid userId)
        {
            foreach (var user in users)
                if (user.UserId == userId)
                    return user;

            return null;
        }
        public ToDoUser? GetUserByTelegramUserId(long telegramUserId)
        {
            foreach (var user in users)
                if (user.TelegramUserId == telegramUserId)
                    return user;

            return null;
        }
        public void Add(ToDoUser user)
        {
            users.Add(user);
        }
    }
}
