using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB.Mapping;

namespace otus_dz2_v2.Infrastructure.DataAccessDb.Models
{
    [Table("Notification")]
    internal class NotificationModel
    {
        [PrimaryKey, Column("Id")]
        public Guid Id { get; set; }

        [Column("User_Id"), NotNull]
        public Guid UserId { get; set; }

        [Column("Type"), NotNull]
        public string Type { get; set; }

        [Column("Text"), NotNull]
        public string Text { get; set; }

        [Column("Scheduled_At"), NotNull]
        public DateTime ScheduledAt { get; set; }

        [Column("Is_Notified"), NotNull]
        public bool IsNotified { get; set; }

        [Column("Notified_At")]
        public DateTime? NotifiedAt { get; set; }
    }
}
