using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.Core.Entities; 
using otus_dz2_v2.Core.Services;

namespace otus_dz2_v2.Core.DataAccess
{
    public interface IUserRepository
    {
        Task<ToDoUser?> GetUserAsync(ToDoUser user, CancellationToken cancellationToken);
        Task AddAsync(ToDoUser user, CancellationToken cancellationToken);
        Task<ToDoUser?>? GetUserByTelegramUserIdAsync(long telegramUserId, CancellationToken cancellationToken);
    }
}
