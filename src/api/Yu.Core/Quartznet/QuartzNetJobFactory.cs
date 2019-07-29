using Quartz;
using Quartz.Spi;
using Quartz.Util;
using System;

namespace Yu.Core.Quartznet
{
    public class QuartzNetJobFactory : IJobFactory
    {
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobType = bundle.JobDetail.JobType;
            return ObjectUtils.InstantiateType<IJob>(jobType);
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
