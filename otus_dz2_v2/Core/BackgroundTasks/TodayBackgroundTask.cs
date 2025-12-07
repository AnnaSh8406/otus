using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.Core.DataAccess;
using otus_dz2_v2.Core.Services;

namespace otus_dz2_v2.Core.BackgroundTasks
{
    internal class TodayBackgroundTask : BackgroundTask
    {
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly IToDoRepository _toDoRepository;
        public TodayBackgroundTask(INotificationService notificationService, IUserRepository userRepository, IToDoRepository toDoRepository) : base(TimeSpan.FromDays(1), nameof(TodayBackgroundTask))
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
                var todayTasks = await _toDoRepository.GetActiveWithDeadline(user.UserId, DateTime.UtcNow.Date, DateTime.UtcNow.Date.AddDays(+1).Date, cancellationToken);
                if (todayTasks.Any())
                {
                    var tasksText = string.Join(", ", todayTasks.Select(x => x.Name));
                    if (tasksText.Length > 1000)
                    {
                        tasksText = tasksText.Substring(0, 1000);
                    }
                    await _notificationService.ScheduleNotification(user.UserId, $"Today_{DateOnly.FromDateTime(DateTime.UtcNow)}", tasksText, DateTime.UtcNow, cancellationToken);
                }
            }
        }
    }
}
