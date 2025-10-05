using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace otus_dz2_v2.Core.Exceptions
{
    public static class EnumerableExtension
    {
        public static IEnumerable<T> GetBatchByNumber<T>(this IEnumerable<T> value, int batchSize, int batchNumber)
        {
            if (batchSize <= 0)
                throw new ArgumentException("Размер должен быть больше 0");

            if (batchNumber < 0)
                throw new ArgumentException("Номер не должен быть отрицательным");

            return value.Skip(batchNumber * batchSize).Take(batchSize);
        }
    }
}
