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
using otus_dz2_v2.Infrastructure.DataAccess;
using otus_dz2_v2.Infrastructure.DataAccessDb.Repositories;
using LinqToDB.Remote;
using otus_dz2_v2.Infrastructure.DataAccessDb;

namespace otus_dz2_v2.Core.Services
{

    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;

        public UserService()
        {
            _userRepository = new SqlUserRepository(new DataContextFactory());
        }

        public async Task<ToDoUser> RegisterUserAsync(long telegramUserId, string telegramUsername, CancellationToken cancellationToken)
        {
            var user = new ToDoUser
            {
                UserId = Guid.NewGuid(),
                TelegramUserId = telegramUserId,
                TelegramUserName = telegramUsername,
                RegisteredAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user, cancellationToken);
            return user;
        }

        public async Task<ToDoUser?> GetUserAsync(long telegramUserId, CancellationToken cancellationToken)
        {
            return await Task.Run(() => _userRepository.GetUserByTelegramUserIdAsync(telegramUserId, cancellationToken));
        }



    }

}

