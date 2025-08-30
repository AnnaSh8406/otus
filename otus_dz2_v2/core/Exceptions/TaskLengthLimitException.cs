using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace otus_dz2_v2.core.Exceptions
{
    internal class TaskLengthLimitException : Exception
    {
        public TaskLengthLimitException(int maxTaskLenght) : base($"Длина задачи {maxTaskLenght} превышает допустимое значение") { }
    }
}
