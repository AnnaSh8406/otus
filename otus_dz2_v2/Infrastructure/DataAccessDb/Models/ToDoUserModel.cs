using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB.Mapping;

namespace otus_dz2_v2.Infrastructure.DataAccessDb.Models
{
    [Table("ToDoUser")]
    internal class ToDoUserModel
    {
        [PrimaryKey, Column("User_Id")]
        public Guid UserId { get; set; }

        [Column("Telegram_User_Name"), NotNull]
        public string TelegramUserName { get; set; }

        [Column("Registered_At"), NotNull]
        public DateTime RegisteredAt { get; set; }

        [Column("Telegram_User_Id"), NotNull]
        public long TelegramUserId { get; set; }
        [Column("chat_id")]
        public long ChatId { get; set; }

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(ToDoListModel.UserId))]
        public List<ToDoListModel> ToDoLists { get; set; } = [];

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(ToDoItemModel.UserId))]
        public List<ToDoItemModel> ToDoItems { get; set; } = [];

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(NotificationModel.UserId))]
        public List<NotificationModel> Notifications { get; set; } = [];

    }
}
