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
using Telegram.Bot.Types;
using System.Threading;

namespace otus_dz2_v2.Infrastructure.DataAccessDb.Repositories
{ 
        internal class SqlUserRepository : IUserRepository
        {
            private IDataContextFactory<DataConnection> _dataContextFactory;
            public SqlUserRepository(IDataContextFactory<DataConnection> dataContextFactory)
            {
                _dataContextFactory = dataContextFactory;
            }
            public async Task AddAsync(ToDoUser user, CancellationToken cancellationToken)
            {
                if (await GetUserByTelegramUserIdAsync(user.TelegramUserId, cancellationToken) == null)
                {
                    using var dbContext = _dataContextFactory.CreateDataContext();
                    await dbContext.InsertAsync<ToDoUserModel>(ModelMapper.MapToModel(user), token: cancellationToken);
                }
            }

            public async Task<ToDoUser?> GetUserAsync(Guid userId, CancellationToken cancellationToken)
            {
                using var dbContext = _dataContextFactory.CreateDataContext();
                var toDoUserModel = await dbContext.GetTable<ToDoUserModel>().Where(x => x.UserId == userId).FirstOrDefaultAsync(cancellationToken);
                return toDoUserModel != null ? ModelMapper.MapFromModel(toDoUserModel) : null;
            }

            public async Task<ToDoUser?> GetUserByTelegramUserIdAsync(long telegramUserId, CancellationToken cancellationToken)
            {
                using var dbContext = _dataContextFactory.CreateDataContext();
                var toDoUserModel = await dbContext.GetTable<ToDoUserModel>().Where(x => x.TelegramUserId == telegramUserId).FirstOrDefaultAsync();
                return toDoUserModel != null ? ModelMapper.MapFromModel(toDoUserModel) : null;
            }
        }
    }
