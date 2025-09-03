using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using otus_dz2_v2.Core.Entities;

namespace otus_dz2_v2.Core.Services
{
    public interface IToDoReportService
    {
        Task<(int Total, int Completed, int Active, DateTime GeneratedAt)> GetUserStatsAsync(Guid userId, CancellationToken cancellationToken);
    }
}
