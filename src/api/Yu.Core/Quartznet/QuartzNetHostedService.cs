using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Spi;
using System.Threading;
using System.Threading.Tasks;

namespace Yu.Core.Quartznet
{
    public class QuartzNetHostedService : IHostedService
    {
        private readonly IScheduler _scheduler;

        public QuartzNetHostedService(IScheduler scheduler, IJobFactory _jobFactory)
        {
            _scheduler = scheduler;
            _scheduler.JobFactory = _jobFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _scheduler.Shutdown(cancellationToken);
        }
    }
}
