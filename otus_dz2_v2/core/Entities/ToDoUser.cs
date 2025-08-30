using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace otus_dz2_v2.core.Entities
{
    public class ToDoUser
    {
        public ToDoUser(long TelegramUserId, string TelegramUserName)
        {
            UserId = Guid.NewGuid();
            RegisteredAt = DateTime.Now;
            this.TelegramUserId = TelegramUserId;
            this.TelegramUserName = TelegramUserName;
        }
        public Guid UserId { get; init; }
        public long TelegramUserId { get; init; }
        public string TelegramUserName { get; init; }
        public DateTime RegisteredAt { get; init; }
    }
}
