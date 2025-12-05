using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB.Async;
using LinqToDB;
using otus_dz2_v2.Core.DataAccess;
using otus_dz2_v2.Core.Entities;
using otus_dz2_v2.Infrastructure.DataAccessDb.Models;
using LinqToDB.Data;
using System.Threading;

namespace otus_dz2_v2.Infrastructure.DataAccessDb.Repositories
{

    internal class SqlToDoListRepository : IToDoListRepository
    {
        private IDataContextFactory<DataConnection> _dataContextFactory;

        public SqlToDoListRepository(IDataContextFactory<DataConnection> dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        public async Task Add(ToDoList list, CancellationToken cancellationToken)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            await dbContext.InsertAsync<ToDoListModel>(ModelMapper.MapToModel(list), token: cancellationToken);
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            await Task.Run(() => dbContext.GetTable<ToDoListModel>().Where(x => x.Id == id).Delete());
        }

        public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken cancellationToken)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            return await Task.Run(() => dbContext.GetTable<ToDoListModel>().Where(x => x.UserId == userId && x.Name == name).Count() > 0);
        }

        public async Task<ToDoList?> Get(Guid id, CancellationToken cancellationToken)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            var toDolistModel = await dbContext.GetTable<ToDoListModel>().Where(x => x.Id == id).FirstOrDefaultAsync();
            return toDolistModel != null ? ModelMapper.MapFromModel(toDolistModel) : null;
        }

        public async Task<IReadOnlyList<ToDoList>> GetByUserId(Guid userId, CancellationToken cancellationToken)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            var toDolistModel = await Task.Run(() => dbContext.GetTable<ToDoListModel>().Where(x => x.UserId == userId).ToList());

            var resultList = new List<ToDoList>();

            foreach (var item in toDolistModel)
            {
                resultList.Add(ModelMapper.MapFromModel(item));
            }

            return resultList;
        }
    }
}
