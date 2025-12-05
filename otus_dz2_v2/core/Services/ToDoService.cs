using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using otus_dz2_v2.Core.DataAccess;
using otus_dz2_v2.Core.Entities;
using otus_dz2_v2.Core.Exceptions;

namespace otus_dz2_v2.Core.Services
{

    public class ToDoService : IToDoService
    {
        public ToDoService(int maxTasks, int taskLength, IToDoRepository toDoRepository)
        {
            this.toDoRepository = toDoRepository;
            this.maxTasks = maxTasks;
            this.taskLength = taskLength;
        }

        public readonly int maxTasks;
        public readonly int taskLength;
        private readonly IToDoRepository toDoRepository;


        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await toDoRepository.GetAllByUserIdAsync(userId, cancellationToken);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await toDoRepository.GetActiveByUserIdAsync(userId, cancellationToken);
        }



        public async Task<ToDoItem> AddAsync(ToDoUser user, string toDoItemName, DateTime date, ToDoList? list, CancellationToken cancellationToken)
        {
            Keyboard.ValidateString(toDoItemName);

            if (await  toDoRepository.CountActiveAsync(user.UserId, cancellationToken) >= maxTasks)
                throw new TaskLengthLimitException(maxTasks);

            if (toDoItemName.Length > taskLength)
                throw new TaskLengthLimitException(toDoItemName.Length);

            if (await toDoRepository.ExistsByNameAsync(user.UserId, toDoItemName, cancellationToken))
            {
                throw new DuplicateTaskException(toDoItemName);
            }
            var newToDoItem = new ToDoItem()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                State = ToDoItemState.Active,
                User = user,
                Name = toDoItemName,
                Date = date,
                List = list
            };

            await  toDoRepository.AddAsync(newToDoItem, cancellationToken);
            return newToDoItem;
        }



        public async Task MarkCompletedAsync(Guid id, CancellationToken cancellationToken)
        {
            var toDoItem = await toDoRepository.GetAsync(id, cancellationToken);
            if (toDoItem != null)
            {
                toDoRepository.Update(toDoItem);
            }
        }


        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await toDoRepository.DeleteAsync(id, cancellationToken);
        }



        public async Task<IReadOnlyList<ToDoItem>> FindAsync(ToDoUser user, string namePrefix, CancellationToken cancellationToken)
        {
            return await toDoRepository.FindAsync(user.UserId, t => t.Name.StartsWith(namePrefix, StringComparison.OrdinalIgnoreCase), cancellationToken);
        }


        private async Task<bool> DublicateCheckAsync(string name, ToDoUser? user, CancellationToken cancellationToken)
        {
            return await toDoRepository.ExistsByNameAsync(user.UserId, name, cancellationToken);
        }


        public async Task<IReadOnlyList<ToDoItem>> GetByUserIdAndListAsync(Guid userId, Guid? listId, CancellationToken cancellationToken)
        {
            return await Task.Run(() => toDoRepository.GetByUserIdAndList(userId, listId, cancellationToken));
        }

        public async Task<ToDoItem?> Get(Guid toDoItemId, CancellationToken cancellationToken)
        {
            return await toDoRepository.GetAsync(toDoItemId, cancellationToken);

        }
    }
}
