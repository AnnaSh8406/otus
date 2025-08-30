using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace otus_dz2_v2.core.Exceptions
{
    class TaskCountLimitException : Exception
    {
        public TaskCountLimitException(int maxTasks) : base($"Превышено максимальное кол-во задач равное {maxTasks}") { }
    }
}
