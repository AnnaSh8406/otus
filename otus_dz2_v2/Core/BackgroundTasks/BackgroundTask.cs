using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace otus_dz2_v2.Core.BackgroundTasks
{
    public abstract class BackgroundTask(TimeSpan delay, string name) : IBackgroundTask
    {
        protected abstract Task Execute(CancellationToken cancellationToken);

        public async Task Start(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine($"{name}. Execute");
                    await Execute(cancellationToken);

                    Console.WriteLine($"{name}. Start delay {delay}");
                    await Task.Delay(delay, cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    // 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{name}. Error: {ex}");
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
            }
        }
    }
}
