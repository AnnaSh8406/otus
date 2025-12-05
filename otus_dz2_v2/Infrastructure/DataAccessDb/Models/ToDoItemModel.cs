using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace otus_dz2_v2.Infrastructure.DataAccessDb.Models
{
    [Table("ToDoItem")]
    public class ToDoItemModel
    {
        [Column("id"), PrimaryKey]
        public Guid Id { get; set; }                        

        [Column("name"), NotNull]
        public string Name { get; set; }                     

        [Column("created_at"), NotNull]
        public DateTime CreatedAt { get; set; }              

        [Column("state"), NotNull]
        public ToDoItemState State { get; set; }                

        [Column("state_change_at")]
        public DateTime? StateChangedAt { get; set; }          

        [Column("date")]
        public DateTime? Date { get; set; }               

        [Column("user_id")]
        public Guid UserId { get; set; }                       

        [Column("list_id")]
        public Guid? ListId { get; set; }                       


        [Association(ThisKey = nameof(ListId), OtherKey = nameof(ToDoListModel.Id))]
        public ToDoListModel? List { get; set; }                    

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(ToDoUserModel.UserId))]
        public ToDoUserModel User { get; set; }                    
    }
}
