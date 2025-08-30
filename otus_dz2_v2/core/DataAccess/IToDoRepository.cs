using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.core.Entities;

namespace otus_dz2_v2.core.DataAccess
{
    public interface IToDoRepository
    {
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
        void Add(ToDoItem item);
        void Update(ToDoItem item);
        void Delete(Guid id);
        bool ExistsByName(Guid userId, string name);
        int CountActive(Guid userId);
        IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate);
    }
}
