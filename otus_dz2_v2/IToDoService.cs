using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace otus_dz2_v2
{

   
    public interface IToDoService
    {
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
        //Возвращает ToDoItem для UserId со статусом Active
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
        ToDoItem Add(ToDoUser userId, string name);
        void MarkCompleted(Guid id);
        void Delete(Guid id);
    }
    public class ToDoService : IToDoService
    { 
    private readonly List<ToDoItem> _toDoItems = new List<ToDoItem>();
       public const int MaxCount = 100;
        public const int MaxTaskLenght = 100;
       // public readonly int MaxCount ;
       // public readonly int MaxTaskLenght ;

       /* public class NumberValidator
        {
            public static bool ParseAndValidateInt(int MaxCount, int min, int max)
            {
                if (MaxCount >= min && MaxCount <= max)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public static bool ParseAndValidateIntLen(int MaxTaskLenght, int min, int max)
            {
                if (MaxTaskLenght >= min && MaxTaskLenght <= max)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }*/
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
                    throw new Exception("Превышено максимальное кол-во задач (100 задач)");
                }

                if (name.Length > MaxTaskLenght)
                {
                    throw new Exception("Превышено количество символов в задаче (100 символов)");
                }
                if (string.IsNullOrEmpty(name))
                {
                    throw new Exception("Задача не введена");
                }
                if (_toDoItems.Any(item => item.Name == name && item.UserId == user.UserId && item.State == ToDoItemState.Activ))
                {
                    throw new Exception("Такая задача уже существует");
                }
          

            Guid id = Guid.NewGuid();
            
            var newItem = new ToDoItem(id,name)
            {
                 
                UserId = user.UserId,
                Name = name,
                CreatedAt = DateTime.Now,
                State = ToDoItemState.Activ,
                ID = Guid.NewGuid()
            };
            
                _toDoItems.Add(newItem);
                return newItem;
            
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
