using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otus.ToDoList.ConsoleBot.Types;

namespace otus_dz2_v2
{ 

    public class ToDoService() : IToDoService
    {
        private readonly List<ToDoItem> _toDoItems = new List<ToDoItem>();

      

        

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return _toDoItems.Where(t => t.User.UserId == userId).ToList();//.AsReadOnly();
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return _toDoItems.Where(t => t.User.UserId == userId && t.State == ToDoItemState.Active).ToList();//.AsReadOnly();
        }
        public ToDoItem Add(ToDoUser user, string name )
        {
            if (_toDoItems.Count(t=>t.User.UserId==user.UserId && t.State== ToDoItemState.Active) >= Program.maxTasks)
            {
                throw new Exception($"Превышено максимальное кол-во задач ({Program.maxTasks} задач)");
            }

            if (name.Length > Program.maxTaskLenght)
            {
                throw new Exception($"Превышено количество символов в задаче ({Program.maxTaskLenght} символов)");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Задача не введена");
            }
            if (_toDoItems.Any(t=>t.User.UserId ==user.UserId  && t.Name==name && t.State == ToDoItemState.Active))
            {
                throw new Exception("Такая задача уже существует");
            }

               

            var item = new ToDoItem 
            {
                ID=Guid.NewGuid(),
                User=user,
                Name = name,
                CreatedAt = DateTime.Now,
                State = ToDoItemState.Active 
                
                
            };

            _toDoItems.Add(item);
            return item;

        }


        public void MarkCompleted(Guid id)
        {
            var item = _toDoItems.FirstOrDefault(t => t.ID == id);
            if (item != null)
            {
                item.State = ToDoItemState.Completed;
                item.StartChangeAt = DateTime.Now;
            }
        }
        public void Delete(Guid id)
        {
            _toDoItems.RemoveAll(t => t.ID == id);
        }

    }
}
