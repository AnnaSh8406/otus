using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace otus_dz2_v2.Core.Entities
{
    internal class IndexItem
    {
        public Guid ToDoItemId { get; init; }
        public Guid UserId { get; init; }
        public IndexItem() { }

        public IndexItem(Guid toDoItemId, Guid userId)
        {
            ToDoItemId = toDoItemId;
            UserId = userId;
        }

    }
}
