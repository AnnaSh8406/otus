using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.Core.DataAccess;
using otus_dz2_v2.Core.Entities;
using otus_dz2_v2.Infrastructure.DataAccess;

namespace otus_dz2_v2.Core.Services
{
    public class ToDoReportService : IToDoReportService
    {
        private readonly IToDoRepository _toDoRepository;
        public ToDoReportService(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }


        public async Task<(int Total, int Completed, int Active, DateTime GeneratedAt)> GetUserStatsAsync(Guid userId, CancellationToken cancellationToken)
        {
            var todos = await _toDoRepository.GetAllByUserIdAsync(userId, cancellationToken);
            return (
                Total: todos.Count,
                Completed: todos.Count(t => t.State == ToDoItemState.Completed),
                Active: todos.Count(t => t.State == ToDoItemState.Active),
                GeneratedAt: DateTime.UtcNow
            );
        }

    }
}
