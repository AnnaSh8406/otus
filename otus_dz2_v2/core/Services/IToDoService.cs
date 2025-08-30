using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.core.Entities;

namespace otus_dz2_v2.core.Services
{


    public interface IToDoService
    {
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
        ToDoItem Add(ToDoUser user, string name);
        void MarkCompleted(Guid id, ToDoUser user);
        void Delete(Guid id);
        IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix);
    }

}
