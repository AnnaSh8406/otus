using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.core.DataAccess;
using otus_dz2_v2.core.Entities;

namespace otus_dz2_v2.Infrastructure.DataAccess
{
    public class InMemoryToDoRepository : IToDoRepository
    {
        public InMemoryToDoRepository()
        {
            tasks = new List<ToDoItem>();
        }
        private readonly List<ToDoItem> tasks;
        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return tasks.Where(x => x.User.UserId == userId).ToList();
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return tasks.Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active).ToList();
        }

        public void Add(ToDoItem item)
        {
            tasks.Add(item);
        }

        public void Update(ToDoItem item)
        {
            int index = tasks.FindIndex(x => x.Id == item.Id);
            tasks[index] = item;
        }

        public void Delete(Guid id)
        {
            int index = tasks.FindIndex(x => x.Id == id);
            tasks.RemoveAt(index);
        }

        public bool ExistsByName(Guid userId, string name)
        {
            int items = tasks.Where(x => x.User.UserId == userId && x.Name == name).ToList().Count();

            if (items == 0)
                return true;

            return false;
        }

        public int CountActive(Guid userId)
        {
            return tasks.Where(x => x.User.UserId == userId).ToList().Count;
        }
        public IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate)
        {
            return tasks.Where(x => x.User.UserId == userId).Where(predicate).ToList();
        }
    }
}
