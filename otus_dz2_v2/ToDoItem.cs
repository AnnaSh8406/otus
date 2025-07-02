using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Otus.ToDoList.ConsoleBot.Types;

namespace otus_dz2_v2
{

    public class ToDoItem

    {

        public Guid ID { get; set; } = Guid.NewGuid();
        public ToDoUser User { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public ToDoItemState State { get; set; }
        public DateTime? StartChangeAt { get; set; }
        public Guid UserId { get; internal set; }

        public ToDoItem(Guid id, string name)
        {
            ToDoUser user = new ToDoUser();

            user.UserId = Guid.NewGuid(); 
            ID = id;

            Name = name;
            CreatedAt = DateTime.Now;
            State = ToDoItemState.Activ;
            StartChangeAt = null;
        }
        public void Complete()
        {
            State = ToDoItemState.Complited;
            StartChangeAt = DateTime.Now;

        }


    }

    public class TaskU

    {
        private List<ToDoItem> task = new List<ToDoItem>();

        public void AddTask(ToDoUser userId, string name)
        {
            var UserId = userId;

            Guid id = Guid.NewGuid();
            ToDoItem newTask = new ToDoItem(id, name)
            {
                ID = Guid.NewGuid(),

                Name = name,
                CreatedAt = DateTime.Now,
                State = ToDoItemState.Activ,
                StartChangeAt = null
            };
            task.Add(newTask);
            Console.WriteLine("Задача добавлена");
        }

    }

}