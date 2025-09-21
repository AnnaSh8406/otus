using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Xml.Linq;
using otus_dz2_v2.Core.DataAccess;
using otus_dz2_v2.Core.Entities;

namespace otus_dz2_v2.Infrastructure.DataAccess
{
    public class InMemoryToDoRepository : IToDoRepository
    {
        public InMemoryToDoRepository()
        {
            tasks = new List<ToDoItem>();
        }
        private readonly List<ToDoItem> tasks;
        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await Task.FromResult(tasks.Where(x => x.ToDoUser.UserId == userId).ToList());
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await Task.FromResult(tasks.Where(x => x.ToDoUser.UserId == userId && x.State == ToDoItemState.Active).ToList());
        }

        public async Task AddAsync(ToDoItem item, CancellationToken cancellationToken)
        {
            await Task.Run(() => tasks.Add(item), cancellationToken);
        }

        public async Task UpdateAsync(ToDoItem item, CancellationToken cancellationToken)
        {
            int index = tasks.FindIndex(x => x.Id == item.Id);
            tasks[index] = item;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            int index = tasks.FindIndex(x => x.Id == id);
            await Task.Run(() => tasks.RemoveAt(index), cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(Guid userId, string name, CancellationToken cancellationToken)
        {

            return await Task.FromResult(tasks.Any(t => t.ToDoUser.UserId == userId && t.Name == name));
        }

        public async Task<int> CountActiveAsync(Guid userId, CancellationToken cancellationToken)
        {

            return await Task.FromResult(tasks.Count(t => t.ToDoUser.UserId == userId && t.State == ToDoItemState.Active));
        }
        public async Task<IReadOnlyList<ToDoItem>> FindAsync(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken cancellationToken)
        {
            return await Task.FromResult(tasks.Where(predicate).ToList());
        }


        public async Task<ToDoItem?> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return tasks.FirstOrDefault(t => t.Id == id);
        }
    }
}
