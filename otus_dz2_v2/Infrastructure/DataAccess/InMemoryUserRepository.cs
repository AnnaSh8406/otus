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
  

        internal class InMemoryUserRepository : IUserRepository
        {
            private List<ToDoUser> _toDoUsers;
            public InMemoryUserRepository()
            {
                _toDoUsers = new List<ToDoUser>();
            }
            public async Task AddAsync(ToDoUser user, CancellationToken ct)
            {
                if (await GetUserByTelegramUserIdAsync(user.TelegramUserId, ct) == null)
                {
                    await Task.Run(() => _toDoUsers.Add(user));
                }
            }

            public async Task<ToDoUser?> GetUserAsync(Guid userId, CancellationToken ct)
            {
                return await Task.Run(() => _toDoUsers.Where(x => x.UserId == userId).FirstOrDefault());
            }

            public async Task<ToDoUser?> GetUserByTelegramUserIdAsync(long telegramUserId, CancellationToken ct)
            {
                return await Task.Run(() => _toDoUsers.Where(x => x.TelegramUserId == telegramUserId).FirstOrDefault(), ct);
            }

            public Task<IReadOnlyList<ToDoUser>> GetUsers(CancellationToken ct)
            {
                              throw new NotImplementedException();
            }
        }
    }
