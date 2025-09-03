using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using otus_dz2_v2.Core.Entities;


namespace otus_dz2_v2.Core.Services
{

    public interface IUserService
    {
        Task<ToDoUser?> GetUserAsync(long telegramUserId, CancellationToken cancellationToken);
        Task<ToDoUser> RegisterUserAsync(long telegramUserId, string telegramUsername, CancellationToken cancellationToken);
    }


}
