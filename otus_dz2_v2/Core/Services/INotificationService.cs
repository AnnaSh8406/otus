using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.Core.Entities;

namespace otus_dz2_v2.Core.Services
{
    public interface INotificationService
    {
         
        Task<bool> ScheduleNotification(
            Guid userId,
            string type,
            string text,
            DateTime scheduledAt,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<Notification>> GetScheduledNotification(DateTime scheduledBefore, CancellationToken cancellationToken);

        Task MarkNotified(Guid notificationId, CancellationToken cancellationToken);
    }
}
