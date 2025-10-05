using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.Core.Entities;

namespace otus_dz2_v2.Core.Services
{
    public interface IToDoListService
    {
        Task<ToDoList> Add(ToDoUser user, string name, CancellationToken cancellationToken);
        Task<ToDoList?> Get(Guid id, CancellationToken cancellationToken);
        Task Delete(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<ToDoList>> GetUserLists(Guid userId, CancellationToken cancellationToken);
    }
}
