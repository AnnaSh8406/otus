using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB.Data;
using LinqToDB;
using otus_dz2_v2.Infrastructure.DataAccessDb.Models;

namespace otus_dz2_v2.Infrastructure.DataAccessDb
{
    
    public class ToDoDataContext : DataConnection
    {
        public ToDoDataContext(string connectionString) : base(ProviderName.PostgreSQL, connectionString) { }

      
        public ITable<ToDoUserModel> ToDoUsers => this.GetTable<ToDoUserModel>();

       
        public ITable<ToDoListModel> ToDoLists => this.GetTable<ToDoListModel>();

         
        public ITable<ToDoItemModel> ToDoItems => this.GetTable<ToDoItemModel>();
    }
}
