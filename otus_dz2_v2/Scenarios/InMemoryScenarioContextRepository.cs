using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace otus_dz2_v2.Scenarios
{
    public class InMemoryScenarioContextRepository : IScenarioContextRepository
    {
        private readonly Dictionary<long, ScenarioContext> _contexts = new();

        public async Task<ScenarioContext?> GetContext(long userId, CancellationToken cancellationToken)
        {
            ScenarioContext? result = null;
            await Task.Run(() =>
            {
                if (_contexts.TryGetValue(userId, out ScenarioContext? value))
                    result = value;
            }
            );
            return result;
        }

        public Task SetContext(long userId, ScenarioContext context, CancellationToken cancellationToken)
        {
            _contexts[userId] = context;
            return Task.CompletedTask;
        }

        public Task ResetContext(long userId, CancellationToken cancellationToken)
        {
            _contexts.Remove(userId);
            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<ScenarioContext>> GetContexts(CancellationToken cancellationToken)
        {
            var result = await Task.Run(() => _contexts.Select(x => x.Value).ToList());
            return result;
        }
    }
}
