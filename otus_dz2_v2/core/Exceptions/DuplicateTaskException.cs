using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace otus_dz2_v2.Core.Exceptions
{
    class DuplicateTaskException : Exception
    {
        public DuplicateTaskException(string task) : base($"Задача {task} уже существует") { }
    }
}
