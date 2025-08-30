using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.core.Entities;

namespace otus_dz2_v2.core.Services
{
    public class ToDoReportService : IToDoReportService
    {
        public ToDoReportService(IToDoService toDoSer)
        {
            this.toDoSer = toDoSer;
        }

        private readonly IToDoService toDoSer;


        public (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId)
        {
            IReadOnlyList<ToDoItem> tasks = toDoSer.GetAllByUserId(userId);
            int totalTasks = tasks.Count;
            int activeTasks = tasks.Where(x => x.State == ToDoItemState.Active).ToList().Count();

            return (totalTasks, totalTasks - activeTasks, activeTasks, DateTime.Now);
        }
    }
}
