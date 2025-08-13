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
       
        public Guid ID { get; set; }  
        public ToDoUser User { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public ToDoItemState State { get; set; }
        public DateTime? StartChangeAt { get; set; }
        
    }
}