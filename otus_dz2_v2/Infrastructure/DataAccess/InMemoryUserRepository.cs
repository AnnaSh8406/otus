using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using otus_dz2_v2.Core.DataAccess;
using otus_dz2_v2.Core.Entities;
using otus_dz2_v2.Core.Services;

namespace otus_dz2_v2.Infrastructure.DataAccess
{
    public class InMemoryUserRepository : IUserRepository
    {

        private readonly List<ToDoUser> users = new List<ToDoUser>();
        public async Task<ToDoUser?> GetUserAsync(long telegramUserId, CancellationToken cancellationToken)
        {

            return await Task.FromResult(users.FirstOrDefault(u => u.TelegramUserId == telegramUserId));
        }
        public async Task<ToDoUser?> GetUserByTelegramUserIdAsync(long telegramUserId, CancellationToken cancellationToken)
        {

            return await Task.FromResult(users.FirstOrDefault(u => u.TelegramUserId == telegramUserId));
        }


        public async Task AddAsync(ToDoUser user, CancellationToken cancellationToken)
        {
            await Task.Run(() => users.Add(user), cancellationToken);
        }

        public Task<ToDoUser?> GetUserAsync(ToDoUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
