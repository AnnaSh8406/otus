using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.Core.Entities;
using otus_dz2_v2.Core.Services;

namespace otus_dz2_v2.Core.DataAccess
{
    interface IUserRepository
    {
        Task<ToDoUser?> GetUserAsync(Guid userId, CancellationToken cancellationTokent);
        Task<ToDoUser?> GetUserByTelegramUserIdAsync(long telegramUserId, CancellationToken cancellationToken);
        Task AddAsync(ToDoUser user, CancellationToken cancellationToken);
        Task<IReadOnlyList<ToDoUser>> GetUsers(CancellationToken cancellationToken);
    }
}
