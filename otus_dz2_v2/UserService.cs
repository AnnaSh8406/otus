using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace otus_dz2_v2
{
    
        public class ToDoUser : IUserService

        {

            public Guid UserId { get; set; }
            public long TelegramUserId { get; set; }
            public string TelegramUserName { get; set; }
            public DateTime RegistereAt { get; set; }


            ToDoUser? IUserService.GetUser(long telegramUserId)
            {
                return _user.FirstOrDefault(u => u.TelegramUserId == telegramUserId);
            }


            ToDoUser IUserService.RegisterUser(long telegramUserId, string telegramUserName)
            {

                var newUser = new ToDoUser
                {
                    TelegramUserId = telegramUserId,
                    TelegramUserName = telegramUserName,
                    UserId = Guid.NewGuid(),
                    RegistereAt = DateTime.Now

                };
                _user.Add(newUser);
                return newUser;
            }

            private static List<ToDoUser> _user = new List<ToDoUser>();


        }

}
 
