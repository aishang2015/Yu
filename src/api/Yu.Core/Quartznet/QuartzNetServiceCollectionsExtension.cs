using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Core.Quartznet
{
    public static class QuartzNetServiceCollectionsExtension
    {
        public static void AddQuartzNet(this IServiceCollection services)
        {
            var schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler().Result;
            services.AddSingleton<ISchedulerFactory>(schedulerFactory);
            services.AddSingleton(scheduler);
            services.AddSingleton<IJobFactory, QuartzNetJobFactory>();
            services.AddHostedService<QuartzNetHostedService>();
        }
    }
}
