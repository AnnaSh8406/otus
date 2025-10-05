using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.Core.Entities;

namespace otus_dz2_v2.Core.DataAccess
{
    public interface IToDoListRepository
    {
        public Task<ToDoList?> Get(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<ToDoList>> GetByUserId(Guid userId, CancellationToken cancellationToken);
        Task Add(ToDoList list, CancellationToken cancellationToken);
        Task Delete(Guid id, CancellationToken cancellationToken);
        Task<bool> ExistsByName(Guid userId, string name, CancellationToken cancellationToken);
    }
}
