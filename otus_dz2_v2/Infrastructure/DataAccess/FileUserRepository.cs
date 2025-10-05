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
        private readonly string _baseDirectory;

        public FileUserRepository(string baseDirectory)
        {
            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }
            _baseDirectory = baseDirectory;
        }



        public async Task<ToDoUser?> GetUserAsync(long telegramUserId, CancellationToken cancellationToken)
        {
            return await GetUserByTelegramUserId(telegramUserId);
        }

        public async Task AddAsync(ToDoUser user, CancellationToken cancellationToken)
        {
            await Add(user);
        }





        private async Task<IEnumerable<ToDoUser>> GetAll()
        {
            return Directory.EnumerateFiles(_baseDirectory)
                .Where(file => Path.GetExtension(file) == ".json")
                .Select(file =>
                {
                    try
                    {
                        return JsonSerializer.Deserialize<ToDoUser>(File.ReadAllText(file));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при обработке файла {file}: {ex.Message}");
                        return null;
                    }
                })
                .Where(user => user != null);
        }

        private async Task<ToDoUser?> GetUser(Guid userId)
        {
            return (await GetAll()).FirstOrDefault(user => user.UserId == userId);
        }

        private async Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId)
        {
            return (await GetAll()).FirstOrDefault(user => user.TelegramUserId == telegramUserId);
        }

        private async Task Add(ToDoUser user)
        {
            var filePath = Path.Combine(_baseDirectory, $"{user.UserId}.json");
            await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(user, new JsonSerializerOptions { WriteIndented = true }));
        }


    }
}
