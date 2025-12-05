using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace otus_dz2_v2.Infrastructure.DataAccessDb.Models
{
    [Table("ToDoList")]
    public class ToDoListModel
    {
        [Column("Id"), PrimaryKey]
        public Guid Id { get; set; }                

        [Column("Name"), NotNull]
        public string Name { get; set; }           

        [Column("Created_At"), NotNull]
        public DateTime CreatedAt { get; set; }      

        [Column("User_Id"), NotNull]
        public Guid UserId { get; set; }           

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(ToDoUserModel.UserId))]
        public ToDoUserModel User { get; set; }          
    }
}
