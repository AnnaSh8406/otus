using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace otus_dz2_v2.Scenarios
{
    public class ScenarioContext
    {
        public long UserId { get; init; }
        public ScenarioType CurrentScenario { get; set; }
        public string? CurrentStep { get; set; }
        public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();

        public DateTime CreatedAt { get; }
        public long ChatId { get; set; }
        public ScenarioContext(long userId, ScenarioType currentScenario)
        {
            UserId = userId;
            CurrentScenario = currentScenario;
            Data = new Dictionary<string, object>();
            CreatedAt = DateTime.UtcNow;
        }
    }
}
