using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using otus_dz2_v2.Core.Services;
using Telegram.Bot;

namespace otus_dz2_v2.Core.BackgroundTasks
{
    internal class NotificationBackgroundTask : BackgroundTask
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        private readonly ITelegramBotClient _bot;
        public NotificationBackgroundTask(INotificationService notificationService, IUserService userService, ITelegramBotClient bot) : base(TimeSpan.FromMinutes(1), nameof(NotificationBackgroundTask))
        {
            _userService = userService;
            _notificationService = notificationService;
            _bot = bot;
        }

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            var notificationList = await _notificationService.GetScheduledNotification(DateTime.UtcNow, cancellationToken);
            foreach (var notification in notificationList)
            {
                var toDoUser = await _userService.GetUserByIdAsync(notification.User.UserId, cancellationToken);
                if (toDoUser != null)
                {
                    await _bot.SendMessage(toDoUser.ChatId, notification.Text, cancellationToken: cancellationToken, replyMarkup: Keyboard.GetKeyboardButtons(true));
                    await _notificationService.MarkNotified(notification.Id, cancellationToken);
                }
            }
        }
    }
}
