using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace otus_dz2_v2.Scenarios
{
    public interface IScenarioContextRepository
    {
        Task<ScenarioContext?> GetContext(long userId, CancellationToken cancellationToken);
        Task SetContext(long userId, ScenarioContext context, CancellationToken cancellationToken);
        Task ResetContext(long userId, CancellationToken cancellationToken);

        Task<IReadOnlyList<ScenarioContext>> GetContexts(CancellationToken cancellationToken);
    }
}
