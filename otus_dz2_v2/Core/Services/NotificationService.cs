using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using otus_dz2_v2.Core.Entities;
using otus_dz2_v2.Infrastructure.DataAccessDb;
using otus_dz2_v2.Infrastructure.DataAccessDb.Models;


namespace otus_dz2_v2.Core.Services
{

    public class NotificationService : INotificationService
    {
        private readonly IDataContextFactory<DataConnection> _dataContextFactory;
        public NotificationService(IDataContextFactory<DataConnection> factory)
        {
            _dataContextFactory = factory;
        }
        public async Task<IReadOnlyList<Notification>> GetScheduledNotification(DateTime scheduledBefore, CancellationToken cancellationToken)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();

            var resultList = new List<Notification>();
            var notificationList = await dbContext.GetTable<NotificationModel>().Where(x => x.IsNotified == false && x.ScheduledAt <= scheduledBefore).ToListAsync();

            foreach (var notification in notificationList)
            {
                resultList.Add(ModelMapper.MapFromModel(notification));
            }

            return resultList;
        }

        public async Task MarkNotified(Guid notificationId, CancellationToken cancellationToken)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            await dbContext.GetTable<NotificationModel>().Where(x => x.Id == notificationId).Set(x => x.IsNotified, true).Set(x => x.NotifiedAt, DateTime.Now).UpdateAsync(cancellationToken);
        }

        public async Task<bool> ScheduleNotification(Guid userId, string type, string text, DateTime scheduledAt, CancellationToken cancellationToken)
        {
            using var dbContext = _dataContextFactory.CreateDataContext();
            int notificationCount = await dbContext.GetTable<NotificationModel>().Where(x => x.UserId == userId && x.Type == type).CountAsync(cancellationToken);
            if (notificationCount > 0)
                return false;

            var notificationModel = new NotificationModel()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = type,
                Text = text,
                ScheduledAt = scheduledAt,
                IsNotified = false
            };

            await dbContext.InsertAsync<NotificationModel>(notificationModel, token: cancellationToken);
            return true;
        }
    }
}
