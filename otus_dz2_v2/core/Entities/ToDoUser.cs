using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace otus_dz2_v2.Core.Entities
{
    public class ToDoUser
    {
        public Guid UserId { get; init; }
        public string TelegramUserName { get; init; }
        public DateTime RegisteredAt { get; init; }
        public long TelegramUserId { get; init; }
        public ToDoUser(string telegramUserName, long telegramUserId)
        {
            UserId = Guid.NewGuid();
            RegisteredAt = DateTime.UtcNow;
            TelegramUserName = telegramUserName;
            TelegramUserId = telegramUserId;
        }
        public ToDoUser() { }


    }
}
