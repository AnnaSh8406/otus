using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using otus_dz2_v2.Core.Services;

namespace otus_dz2_v2.Core.Entities
{

    public class ToDoItem

    {


        public Guid Id { get; init; }
        public ToDoUser ToDoUser { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public ToDoItemState State { get; set; }
        public DateTime? StartChangeAt { get; set; }
        public DateTime Deadline { get; set; }

        /*  public ToDoItem(string Name, ToDoUser User)
          {
              State = ToDoItemState.Active;
              CreatedAt = DateTime.Now;
              Id = Guid.NewGuid(); 
              this.Name = Name;
              this.User = User;

              //Deadline = deadline // Задание срока выполнения
          }*/
    }
}