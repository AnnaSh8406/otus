using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using otus_dz2_v2.Core.DataAccess;
using otus_dz2_v2.Core.Entities;
using System.Text.Json;
using System.Threading;

namespace otus_dz2_v2.Infrastructure.DataAccess
{

    public class FileUserRepository : IUserRepository
    {
        private readonly string _directoryName;
        public FileUserRepository(string baseDirectoryName)
        {
            _directoryName = Path.Combine(baseDirectoryName, "ToDoUsers");
            if (!Directory.Exists(_directoryName))
            {
                Directory.CreateDirectory(_directoryName);
            }
        }
        public async Task AddAsync(ToDoUser user, CancellationToken cancellationToken)
        {
            if (await GetUserByTelegramUserIdAsync(user.TelegramUserId, cancellationToken) == null)
            {
                string fileName = Path.Combine(_directoryName, $"{user.UserId}.json");
                using var createStream = File.Create(fileName);
                await JsonSerializer.SerializeAsync(createStream, user, cancellationToken: cancellationToken);
            }
        }

        public async Task<ToDoUser?> GetUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var toDoUsers = await GetAllUsersAsync(cancellationToken);
            return await Task.Run(() => toDoUsers.Where(x => x.UserId == userId).FirstOrDefault());
        }

        public async Task<ToDoUser?> GetUserByTelegramUserIdAsync(long telegramUserId, CancellationToken cancellationToken)
        {
            var toDoUsers = await GetAllUsersAsync(cancellationToken);
            return await Task.Run(() => toDoUsers.Where(x => x.TelegramUserId == telegramUserId).FirstOrDefault(), cancellationToken);
        }

        public async Task<IReadOnlyList<ToDoUser>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            var userList = new List<ToDoUser>();

            if (Directory.Exists(_directoryName))
            {
                var userFiles = Directory.EnumerateFiles(_directoryName);
                foreach (var file in userFiles)
                {
                    using var reader = File.OpenRead(file);
                    var toDoUser = await JsonSerializer.DeserializeAsync<ToDoUser>(reader, cancellationToken: cancellationToken);
                    if (toDoUser != null)
                    {
                        userList.Add(toDoUser);
                    }
                }
            }
            return userList;
        }
    }
}
