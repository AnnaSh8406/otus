using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otus.ToDoList.ConsoleBot.Types;
using otus_dz2_v2.core.DataAccess;
using otus_dz2_v2.core.Entities;
using otus_dz2_v2.core.Exceptions;

namespace otus_dz2_v2.core.Services
{

    public class ToDoService : IToDoService
    {
        public ToDoService(int maxTasks, int taskLength, IToDoRepository toDoRepository)
        {
            this.toDoRepository = toDoRepository;
            this.maxTasks = maxTasks;
            this.taskLength = taskLength;
        }

        private readonly int maxTasks;
        private readonly int taskLength;
        private readonly IToDoRepository toDoRepository;


        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return toDoRepository.GetActiveByUserId(userId);
        }


        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return toDoRepository.GetAllByUserId(userId);
        }


        public ToDoItem Add(ToDoUser user, string name)
        {
            if (toDoRepository.CountActive(user.UserId) >= maxTasks)
                throw new TaskCountLimitException(maxTasks);

            if (name.Length > taskLength)
                throw new TaskLengthLimitException(name.Length);

            if (!DublicateCheck(name, user))
                throw new DuplicateTaskException(name);

            ToDoItem newItem = new ToDoItem(name, user);
            toDoRepository.Add(newItem);
            return newItem;
        }


        public void MarkCompleted(Guid id, ToDoUser user)
        {
            IReadOnlyList<ToDoItem> tasks = GetAllByUserId(user.UserId).Where(x => x.Id == id).ToList();


            if (tasks == null)
                throw new ArgumentException("Задача не найдена");

            tasks[0].State = ToDoItemState.Completed;
            tasks[0].StartChangeAt = DateTime.Now;
            toDoRepository.Update(tasks[0]);


        }


        public void Delete(Guid id)
        {
            toDoRepository.Delete(id);
        }


        public IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix)
        {
            return toDoRepository.Find(user.UserId, x => x.Name.Substring(0, namePrefix.Length) == namePrefix);
        }


        private bool DublicateCheck(string name, ToDoUser user)
        {
            return toDoRepository.ExistsByName(user.UserId, name);
        }
    }
}
