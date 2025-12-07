using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.Core.DataAccess;
using otus_dz2_v2.Core.Services;

namespace otus_dz2_v2.Core.BackgroundTasks
{
    internal class DeadlineBackgroundTask : BackgroundTask
    {
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly IToDoRepository _toDoRepository;

        public DeadlineBackgroundTask(INotificationService notificationService, IUserRepository userRepository, IToDoRepository toDoRepository) : base(TimeSpan.FromHours(1), nameof(DeadlineBackgroundTask))
        {
            _notificationService = notificationService;
            _userRepository = userRepository;
            _toDoRepository = toDoRepository;
        }

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            var userList = await _userRepository.GetUsers(cancellationToken);
            foreach (var user in userList)
            {
                var deadlineTasks = await _toDoRepository.GetActiveWithDeadline(user.UserId, DateTime.UtcNow.AddDays(-1).Date, DateTime.UtcNow.Date, cancellationToken);
                foreach (var task in deadlineTasks)
                {
                    await _notificationService.ScheduleNotification(user.UserId, $"Deadline_{task.Id}", $"Вы пропустили дедлайн по задаче {task.Name}", DateTime.UtcNow, cancellationToken);
                }
            }
        }
    }
}
