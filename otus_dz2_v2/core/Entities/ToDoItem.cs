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
        public ToDoUser User { get; init; }
        public string Name { get; init; }
        public DateTime CreatedAt { get; init; }
        public ToDoItemState State { get; set; }
        public DateTime? StateChangedAt { get; set; }
        public DateTime Deadline { get; set; }
        public ToDoList? List { get; init; }

        public ToDoItem(ToDoUser user, string name, DateTime deadline, ToDoList? list)
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            State = ToDoItemState.Active;
            User = user;
            Name = name;
            Deadline = deadline;
            List = list;
        }
        public ToDoItem() { }
    }
}