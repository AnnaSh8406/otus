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

        public Guid  ID { get; set; }
        public ToDoUser User { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public ToDoItemState State { get; set; }
        public DateTime? StartChangeAt { get; set; }


        


        public ToDoItem(Guid  iD,string name)
        {
           // UserID =Guid.NewGuid();
              ID =Guid.NewGuid();

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
    private List<ToDoItem> task=new List<ToDoItem>();

        public void AddTask(ToDoUser user, string name)
        {
            ToDoItem newTask = new ToDoItem
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
    /*
    public class ToDoService
    {
        public List<ToDoItem> items= new List<ToDoItem>();

        public int nexid = 1;
        public DateTime crDt = DateTime.Now;

        public ToDoItem AddItem( string name)
        {
            var item = new ToDoItem(nexid,name, createdAt);
             
            items.Add(item);
            return item;
        }
        public List<ToDoItem> GetAllItems()
        {
            return items;
                }
    }*/
}
