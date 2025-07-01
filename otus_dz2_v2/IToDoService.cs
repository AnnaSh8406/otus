using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace otus_dz2_v2
{
    public class IToDoService
    {
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
        //Возвращает ToDoItem для UserId со статусом Active
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
        ToDoItem Add(ToDoUser user, string name);
        void MarkCompleted(Guid id);
        void Delete(Guid id);
    }
    public class ToDoService : IToDoService
    { 
    private readonly List<ToDoItem> _toDoItems = new List<ToDoItem>();
        public const int MaxCount = 10;
        public const int MaxTaskLenght = 10;

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return _toDoItems.Where(_items => _items.UserId == userId).ToList().AsReadOnly();
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return _toDoItems.Where(_items => _items.UserId == userId && _items.State==ToDoItemState.Activ).ToList().AsReadOnly();
        }
        public ToDoItem Add(ToDoUser user, string name)
        {
            if (_toDoItems.Count >= MaxCount)
            {
                throw new Exception("Превышено максимальное кол-во задач");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Превышено количество символов в задаче");
            }
            if (_toDoItems.Any(item => item.Name == name && item.UserID == user.UserId && item.State == ToDoItemState.Activ))
            {
                throw new Exception("Такая задача уже существует");
            }
        }

        public void MarkCompleted(Guid id) 
        { 
        var item=_toDoItems.FirstOrDefault(item=>item.ID==id);
            if (item != null)
            {
                item.State = ToDoItemState.Complited;
            }
        }
        public void Delete(Guid id)
        { 
        _toDoItems.RemoveAll(item=>item.ID==id);
        }

    }


}
