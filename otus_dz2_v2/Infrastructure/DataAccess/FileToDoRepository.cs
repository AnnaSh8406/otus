using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using otus_dz2_v2.Core.DataAccess;
using otus_dz2_v2.Core.Entities;
using System.Text.Json;

namespace otus_dz2_v2.Infrastructure.DataAccess
{/*
    public class FileToDoRepository : IToDoRepository
    {
        private readonly string _baseDirectory;
        private readonly string _indexFile;
        public FileToDoRepository(string baseDirectory)
        {
            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }
            _baseDirectory = baseDirectory;

            _indexFile = Path.Combine(_baseDirectory, "index.json");
            if (!File.Exists(_indexFile))
            {
                CreateFileIndex();
            }
        }
        private async Task WriteFileIndex(List<Index> indexes)
        {
            await File.WriteAllTextAsync(_indexFile, JsonSerializer.Serialize<List<Index>>(indexes, new JsonSerializerOptions { WriteIndented = true }), cancellationToken: CancellationToken.None);
        }
        private async Task CreateFileIndex()
        {
            var indexes = new List<Index>();
            foreach (var folder in Directory.EnumerateDirectories(_baseDirectory))
            {
                foreach (var file in Directory.EnumerateFiles(folder, "*.json"))
                {
                    try
                    {
                        var item = JsonSerializer.Deserialize<ToDoItem>(File.ReadAllText(file));
                        if (item != null)
                        {
                           // indexes.Add(new Index(item.ToDoUser.UserId, item.Id));
                            indexes.Add(new Index(item.ToDoUser.UserId, item.Id));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при обработке файла {file}: {ex.Message}");
                    }
                }
            }
            await WriteFileIndex(indexes);
        }

        public Task AddAsync(ToDoItem item, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            var json = JsonSerializer.Serialize(item);
            string userPath = $"{_path}\\{item.User.UserId}\\";

            if (!Directory.Exists(userPath))
                Directory.CreateDirectory(userPath);

            File.WriteAllText($"{userPath}\\{item.Id}.json", json);

            Dictionary<Guid, List<Guid>> items = ReadIndexFile();

            if (items.ContainsKey(item.User.UserId))
                items[item.User.UserId].Add(item.Id);
            else
                items.Add(item.User.UserId, new List<Guid> { item.Id });

            SaveIndexFile(items);

            return Task.CompletedTask;
        }

        public Task<int> CountActiveAsync(Guid userId, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(GetActiveByUserIdAsync(userId, cancellationToken).Result.Count);
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            Dictionary<Guid, List<Guid>> items = ReadIndexFile();
            Guid userId = Guid.Empty;

            foreach (var item in items)
            {
                if (item.Value.Contains(id))
                {
                    userId = item.Key;
                    break;
                }
            }
            if (userId == Guid.Empty)
                throw new ArgumentException($"У пользователя {userId} еще нет задач!");

            if (!items[userId].Remove(id))
                throw new ArgumentException($"Задача \'{id}\' не найдена!");

            SaveIndexFile(items);

            File.Delete($"{_path}\\{userId}\\{id}.json");

            return Task.CompletedTask;
        }

        public Task<bool> ExistsByNameAsync(Guid userId, string name, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            ToDoItem? item = GetTasksByUserId(userId).Where(x => x.User.TelegramUserName == name).FirstOrDefault();

            return Task.FromResult(item != null);
        }

        public Task<IReadOnlyList<ToDoItem>> FindAsync(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult((IReadOnlyList<ToDoItem>)GetTasksByUserId(userId).Where(predicate).ToList());

        }

        public Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult((IReadOnlyList<ToDoItem>)GetTasksByUserId(userId).Where(x => x.State == ToDoItemState.Active).ToList());
        }

        public Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult((IReadOnlyList<ToDoItem>)GetTasksByUserId(userId));
        }

        public Task UpdateAsync(ToDoItem item, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            var json = JsonSerializer.Serialize(item);
            File.WriteAllText($"{_path}\\{item.User.UserId}\\{item.Id}.json", json);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Метод получения списка всех задач всех пользователей из файла индекса.
        /// </summary>
        /// <param name="path">Путь к файлу индексу.</param>
        /// <returns>Список всех задач в виде словаря, где ключ это id пользователя, а value это список id его задач.</returns>
        private Dictionary<Guid, List<Guid>> ReadIndexFile()
        {
            string indexPath = $"{_path}\\Index.json";
            Dictionary<Guid, List<Guid>> toDoItems = new Dictionary<Guid, List<Guid>>();

            if (!File.Exists(indexPath))
                return toDoItems;

            string[] indexArray = File.ReadAllLines(indexPath);

            foreach (string item in indexArray)
            {
                string[] toDoItem = item.Split(';');

                if (!Guid.TryParse(toDoItem[0], out Guid todoId))
                    continue;

                if (!Guid.TryParse(toDoItem[1], out Guid userId))
                    continue;

                if (toDoItems.ContainsKey(userId))
                    toDoItems[userId].Add(todoId);
                else
                    toDoItems.Add(userId, new List<Guid> { todoId });
            }

            return toDoItems;
        }

        /// <summary>
        /// Метод сохранения всех задач всех пользователей в файл индекс.
        /// </summary>
        /// <param name="path">Путь к файлу индекса.</param>
        /// <param name="toDoItems">Список всех задач.</param>
        private void SaveIndexFile(Dictionary<Guid, List<Guid>> toDoItems)
        {
            string indexPath = $"{_path}\\Index.json";

            List<string> indexList = new List<string>();

            foreach (KeyValuePair<Guid, List<Guid>> pair in toDoItems)
                foreach (Guid toDoItem in pair.Value)
                    indexList.Add($"{toDoItem};{pair.Key}");

            File.WriteAllLines(indexPath, indexList);
        }


        /// <summary>
        /// Метод получения списка всех задач пользователя.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <returns>Список задач пользователя.</returns>
        private List<ToDoItem> GetTasksByUserId(Guid userId)
        {
            Dictionary<Guid, List<Guid>> items = ReadIndexFile();

            List<ToDoItem> activeItems = new List<ToDoItem>();

            if (!items.ContainsKey(userId))
                return activeItems;

            foreach (Guid itemId in items[userId])
            {
                string itemPath = $"{_path}\\{userId}\\{itemId}.json";
                ToDoItem? item = JsonSerializer.Deserialize<ToDoItem>(File.ReadAllText(itemPath));

                if (item != null)
                    activeItems.Add(item);
            }

            return activeItems;
        }

        public Task<ToDoItem?> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}*/
    public class FileToDoRepository : IToDoRepository
    {
        private readonly string _baseDirectory;
        private readonly string _indexFile;

        public FileToDoRepository(string baseDirectory)
        {
            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }
            _baseDirectory = baseDirectory;

            _indexFile = Path.Combine(_baseDirectory, "index.json");
            if (!File.Exists(_indexFile))
            {
                CreateFileIndex();
            }
        }

        #region Public Methods Implementation

        public async Task<ToDoItem?> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return await GetByGuid(id, cancellationToken);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await GetAllByUserId(userId, cancellationToken);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await GetActiveByUserId(userId, cancellationToken);
        }

        public async Task AddAsync(ToDoItem item, CancellationToken cancellationToken)
        {
            await Add(item, cancellationToken);
        }

        public async Task UpdateAsync(ToDoItem item, CancellationToken cancellationToken)
        {
            await Update(item, cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await Delete(id, cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(Guid userId, string name, CancellationToken cancellationToken)
        {
            return await ExistsByName(userId, name, cancellationToken);
        }

        public async Task<int> CountActiveAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await CountActive(userId, cancellationToken);
        }

        public async Task<IReadOnlyList<ToDoItem>> FindAsync(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken cancellationToken)
        {
            return await Find(userId, predicate, cancellationToken);
        }

        #endregion

        #region Private Helper Methods

        private async Task WriteFileIndex(List<Index> indexes)
        {
            await File.WriteAllTextAsync(_indexFile, JsonSerializer.Serialize<List<Index>>(indexes, new JsonSerializerOptions { WriteIndented = true }), cancellationToken: CancellationToken.None);
        }

        private async Task<List<Index>> ReadFileIndex()
        {
            if (!File.Exists(_indexFile))
            {
                await CreateFileIndex();
            }
            return JsonSerializer.Deserialize<List<Index>>(File.ReadAllText(_indexFile)) ?? new List<Index>();
        }

        private async Task CreateFileIndex()
        {
            var indexes = new List<Index>();
            foreach (var folder in Directory.EnumerateDirectories(_baseDirectory))
            {
                foreach (var file in Directory.EnumerateFiles(folder, "*.json"))
                {
                    try
                    {
                        var item = JsonSerializer.Deserialize<ToDoItem>(File.ReadAllText(file));
                        if (item != null)
                        {
                            indexes.Add(new Index(item.ToDoUser.UserId, item.Id));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при обработке файла {file}: {ex.Message}");
                    }
                }
            }
            await WriteFileIndex(indexes);
        }

        private async Task<ToDoItem?> GetByGuid(Guid id, CancellationToken cancellationToken)
        {
            var indexRecords = await ReadFileIndex();
            var indexRecord = indexRecords.FirstOrDefault(record => record.ItemId == id);


            var filePath = Path.Combine(_baseDirectory, indexRecord.UserId.ToString(), $"{id}.json");
            try
            {
                return JsonSerializer.Deserialize<ToDoItem>(File.ReadAllText(filePath));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла {filePath}: {ex.Message}");
                return null;
            }
        }

        private async Task<IReadOnlyList<ToDoItem>> GetAllByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var userFolder = Path.Combine(_baseDirectory, userId.ToString());
            if (!Directory.Exists(userFolder))
            {
                return new List<ToDoItem>().AsReadOnly();
            }

            return Directory.EnumerateFiles(userFolder)
                .Where(file => Path.GetExtension(file) == ".json")
                .Select(file =>
                {
                    try
                    {
                        return JsonSerializer.Deserialize<ToDoItem>(File.ReadAllText(file));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при обработке файла {file}: {ex.Message}");
                        return null;
                    }
                })
                .Where(item => item != null)
                .ToList()
                .AsReadOnly();
        }

        private async Task<IReadOnlyList<ToDoItem>> GetActiveByUserId(Guid userId, CancellationToken cancellationToken)
        {
            var allItems = await GetAllByUserId(userId, cancellationToken);
            return allItems.Where(i => i.State == ToDoItemState.Active).ToList().AsReadOnly();
        }

        private async Task Add(ToDoItem item, CancellationToken cancellationToken)
        {
            var index = await ReadFileIndex();
            var userFolder = Path.Combine(_baseDirectory, item.ToDoUser.UserId.ToString());
            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);
            }

            var filePath = Path.Combine(userFolder, $"{item.Id}.json");
            await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(item, new JsonSerializerOptions { WriteIndented = true }), cancellationToken);

            index.Add(new Index(item.ToDoUser.UserId, item.Id));
            await WriteFileIndex(index);
        }

        private async Task Update(ToDoItem item, CancellationToken cancellationToken)
        {
            var filePath = Path.Combine(_baseDirectory, item.ToDoUser.UserId.ToString(), $"{item.Id}.json");
            await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(item, new JsonSerializerOptions { WriteIndented = true }), cancellationToken);
        }

        private async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            var index = await ReadFileIndex();
            var indexRecord = index.FirstOrDefault(record => record.ItemId == id);
            if (indexRecord.ItemId != id)
            {
                return;
            }

            index.Remove(indexRecord);
            await WriteFileIndex(index);

            var filePath = Path.Combine(_baseDirectory, indexRecord.UserId.ToString(), $"{id}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private async Task<bool> ExistsByName(Guid userId, string name, CancellationToken cancellationToken)
        {
            var items = await GetAllByUserId(userId, cancellationToken);
            return items.Any(i => i.Name == name);
        }

        private async Task<int> CountActive(Guid userId, CancellationToken cancellationToken)
        {
            var items = await GetAllByUserId(userId, cancellationToken);
            return items.Count(i => i.State == ToDoItemState.Active);
        }

        private async Task<IReadOnlyList<ToDoItem>> Find(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken cancellationToken)
        {
            var items = await GetAllByUserId(userId, cancellationToken);
            return items.Where(predicate).ToList().AsReadOnly();
        }

        #endregion
    }

    internal struct Index
    {
        public Guid UserId { get; set; }
        public Guid ItemId { get; set; }

        public Index(Guid userId, Guid itemId)
        {
            UserId = userId;
            ItemId = itemId;
        }
    }
}