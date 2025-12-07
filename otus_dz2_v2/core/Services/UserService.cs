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
using System.Threading;

namespace otus_dz2_v2.Core.Services
{

    internal class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<ToDoUser?> GetUserAsync(long telegramUserId, CancellationToken cancellationToken)
        {
            return await Task.Run(() => _userRepository.GetUserByTelegramUserIdAsync(telegramUserId, cancellationToken));
        }

        public async Task<ToDoUser?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await Task.Run(() => _userRepository.GetUserAsync(userId, cancellationToken));
        }

        public async Task<ToDoUser> RegisterUserAsync(long telegramUserId, string telegramUserName, long chatId, CancellationToken cancellationToken)
        {
            var user = new ToDoUser()
            {
                UserId = Guid.NewGuid(),
                TelegramUserName = telegramUserName,
                TelegramUserId = telegramUserId,
                RegisteredAt = DateTime.UtcNow,
                ChatId = chatId
            };

            await _userRepository.AddAsync(user, cancellationToken);
            return user;
        }
    }

}

