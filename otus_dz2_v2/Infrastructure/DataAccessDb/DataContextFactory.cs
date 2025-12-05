using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB.Data;
namespace otus_dz2_v2.Infrastructure.DataAccessDb
{
   
        internal class DataContextFactory : IDataContextFactory<DataConnection>
        {
            public DataConnection CreateDataContext()
            {
                return new ToDoDataContext(Keyboard.CONNECTION_STRING);
            }
        }
    }

