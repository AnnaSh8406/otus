using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.Core.DataAccess;
using otus_dz2_v2.Core.Entities;
using otus_dz2_v2.Infrastructure.DataAccess;

using static otus_dz2_v2.Program;


namespace otus_dz2_v2.Core.Services
{
    public class ToDoListService : IToDoListService
    {
        private IToDoListRepository _toDoListRepository;

        public ToDoListService()
        {
            _toDoListRepository = new FileToDoListRepository(Keyboard.dataDir);
        }
        public async Task<ToDoList> Add(ToDoUser user, string name, CancellationToken cancellationToken)
        {
            if (name.Length > 100)
                throw new ArgumentException($"Длина списка больше 100 символов");

            if (await _toDoListRepository.ExistsByName(user.UserId, name, cancellationToken))
            {
                throw new ArgumentException($"Список с таким названием уже существует");
            }

            var toDoList = new ToDoList() { Id = Guid.NewGuid(), User = user, Name = name, CreatedAt = DateTime.UtcNow };
            await _toDoListRepository.Add(toDoList, cancellationToken);
            return toDoList;
        }

        public async Task Delete(Guid id, CancellationToken cancellationToken)
        {
            await _toDoListRepository.Delete(id, cancellationToken);
        }

        public async Task<ToDoList?> Get(Guid id, CancellationToken cancellationToken)
        {
            return await _toDoListRepository.Get(id, cancellationToken);
        }

        public async Task<IReadOnlyList<ToDoList>> GetUserLists(Guid userId, CancellationToken cancellationToken)
        {
            return await _toDoListRepository.GetByUserId(userId, cancellationToken);
        }
    }
}
