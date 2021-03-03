using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TaskRunner.Model
{
    public class TRJobListener : IJobListener
    {

        ILogger _logger;
        JobScheduler _scheduler;


        public string Name { get; }

        public TRJobListener(string name, ILogger logger, JobScheduler scheduler)
        {
            Name = name;
            _logger = logger;
            _scheduler = scheduler;
        }

        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation($"JobToBeExecuted {context.JobDetail.Description}");
        }

        public async Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation($"JobExecutionVetoed {context.JobDetail.Description}");
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            await _scheduler.UpdateJobDetail(context.JobDetail.Key);

            var nextFireTime = context.Trigger.GetNextFireTimeUtc().GetValueOrDefault().DateTime.ToLocalTime();

            _scheduler.TotalTaskLaunches++;
            await _scheduler.ResetNextExecuteTime();
            _scheduler.LastTaskLaunch = DateTime.Now;

        }


    }
}
