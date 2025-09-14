using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using otus_dz2_v2.Core.DataAccess;
using otus_dz2_v2.Core.Entities;

namespace otus_dz2_v2.Infrastructure.DataAccess
{
    public class FileUserRepository : IUserRepository
    {
        private readonly string _baseDirect;

        public FileUserRepository(string baseDirect)
        {
            if (!Directory.Exists(baseDirect))
            {
                Directory.CreateDirectory(baseDirect);
            }
            _baseDirect = baseDirect;
        }


        public async Task AddAsync(ToDoUser user, CancellationToken cancellationToken)
        {
            await Add(user);
        }


        public async Task<ToDoUser?> GetUserAsync(long telegramUserId, CancellationToken cancellationToken)
        {
            return await GetUserByTelegramUserId(telegramUserId,cancellationToken);
        }

        
        private async Task<IEnumerable<ToDoUser>> GetAllToFile()
        {
            return Directory.EnumerateFiles(_baseDirect)
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

                               
        private async Task<ToDoUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken cancellationToken)
        {
            return (await GetAllToFile()).FirstOrDefault(user => user.TelegramUserId == telegramUserId);
        }

        private async Task Add(ToDoUser user)
        {
            var filePath = Path.Combine(_baseDirect, $"{user.UserId}.json");
            await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(user, new JsonSerializerOptions { WriteIndented = true }));
        }
        private async Task<ToDoUser?> GetUser(Guid userId)
        {
            return (await GetAllToFile()).FirstOrDefault(user => user.UserId == userId);
        }


    }
}
