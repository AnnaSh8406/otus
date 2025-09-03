using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Otus.ToDoList.ConsoleBot.Types;
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

        private readonly int maxTasks;
        private readonly int taskLength;
        private readonly IToDoRepository toDoRepository;


        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await toDoRepository.GetAllByUserIdAsync(userId, cancellationToken);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await toDoRepository.GetActiveByUserIdAsync(userId, cancellationToken);
        }


        public async Task<ToDoItem> AddAsync(ToDoUser? user, string name, CancellationToken cancellationToken)
        {
            if ((await toDoRepository.CountActiveAsync(user.UserId, cancellationToken)) >= maxTasks)
                throw new TaskCountLimitException(maxTasks);

            if (name.Length > taskLength)
            {
                throw new TaskLengthLimitException(name.Length);
            }



            if (await DublicateCheckAsync(name, user, cancellationToken))
                throw new DuplicateTaskException(name);

            ToDoItem newItem = new ToDoItem(name, user);
            toDoRepository.AddAsync(newItem, cancellationToken);
            return newItem;
        }


        public async Task MarkCompletedAsync(Guid id, CancellationToken cancellationToken)
        {
            var tasks = await toDoRepository.GetAsync(id, cancellationToken);
            if (tasks != null)
            {
                tasks.State = ToDoItemState.Completed;
                await toDoRepository.UpdateAsync(tasks, cancellationToken);
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

    }
}
