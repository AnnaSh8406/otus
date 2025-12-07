using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.Core.Entities;

namespace otus_dz2_v2.Core.DataAccess
{
    public interface IToDoRepository
    {
        Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        //Возвращает ToDoItem для UserId со статусом Active
        Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<ToDoItem?> GetAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(ToDoItem item, CancellationToken cancellationTokenct);
        void Update(ToDoItem item);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> ExistsByNameAsync(Guid userId, string name, CancellationToken cancellationToken);
        Task<int> CountActiveAsync(Guid userId, CancellationToken cancellationToken);
        Task<IReadOnlyList<ToDoItem>> FindAsync(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken cancellationToken);
        Task<IReadOnlyList<ToDoItem>> GetByUserIdAndList(Guid userId, Guid? listId, CancellationToken cancellationToken);
        Task<IReadOnlyList<ToDoItem>> GetActiveWithDeadline(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken);
    }
}