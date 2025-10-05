using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.Core.Entities;

namespace otus_dz2_v2.Core.Services
{


    public interface IToDoService
    {
        Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken);
        Task<ToDoItem> AddAsync(ToDoUser? user, string name, DateTime deadline, ToDoList? list, CancellationToken cancellationToken);
        Task MarkCompletedAsync(Guid id, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<ToDoItem>> FindAsync(ToDoUser user, string namePrefix, CancellationToken cancellationToken);

        Task<IReadOnlyList<ToDoItem>> GetByUserIdAndListAsync(Guid userId, Guid? listId, CancellationToken cancellationToken);
        Task<ToDoItem?> Get(Guid toDoItemId, CancellationToken cancellationToken);
    }
}
