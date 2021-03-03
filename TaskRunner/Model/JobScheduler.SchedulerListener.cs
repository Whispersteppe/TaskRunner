using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TaskRunner.Model
{
    public class TRScheduleListener : ISchedulerListener
    {
        ILogger _logger;
        JobScheduler _scheduler;

        public TRScheduleListener(ILogger logger,  JobScheduler scheduler)
        {
            _logger = logger;
            _scheduler = scheduler;
        }


        public async Task JobScheduled(ITrigger trigger, CancellationToken cancellationToken = default)
        {

            await _scheduler.UpdateJobDetail(trigger.JobKey);

            var nextFireTime = trigger.GetNextFireTimeUtc().GetValueOrDefault().DateTime.ToLocalTime();

            await _scheduler.ResetNextExecuteTime();
        }

        public async Task JobUnscheduled(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation($"JobUnscheduled {triggerKey}");
        }

        public async Task TriggerFinalized(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation($"TriggerFinalized {trigger.Description}");
        }

        public async Task TriggerPaused(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation($"TriggerPaused {triggerKey}");
        }

        public async Task TriggersPaused(string triggerGroup, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation($"TriggersPaused {triggerGroup}");
        }

        public async Task TriggerResumed(TriggerKey triggerKey, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation($"TriggerResumed {triggerKey}");
        }

        public async Task TriggersResumed(string triggerGroup, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation($"TriggersResumed {triggerGroup}");
        }

        public async Task JobAdded(IJobDetail jobDetail, CancellationToken cancellationToken = default)
        {
            await _scheduler.AddJobDetail(jobDetail.Key);

        }

        public async Task JobDeleted(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            await _scheduler.RemoveJobDetail(jobKey);
        }

        public async Task JobPaused(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            await _scheduler.UpdateJobDetail(jobKey);

            await Task.Run(() => { });

            await _scheduler.ResetNextExecuteTime();

            _logger.LogInformation($"JobPaused {jobKey}");
        }

        public async Task JobInterrupted(JobKey jobKey, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation($"JobInterrupted {jobKey}");
        }

        public async Task JobsPaused(string jobGroup, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation($"JobsPaused {jobGroup}");
        }

        public async Task JobResumed(JobKey jobKey, CancellationToken cancellationToken = default)
        {

            await _scheduler.UpdateJobDetail(jobKey);

            await Task.Run(() => { });

            await _scheduler.ResetNextExecuteTime();

            _logger.LogInformation($"JobResumed {jobKey}");
        }

        public async Task JobsResumed(string jobGroup, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation($"JobsResumed {jobGroup}");
        }

        public async Task SchedulerError(string msg, SchedulerException cause, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogError(cause, msg);
        }

        public async Task SchedulerInStandbyMode(CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation("Scheduler In Standby Mode");
        }

        public async Task SchedulerStarted(CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation("Scheduler Started");
        }

        public async Task SchedulerStarting(CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation("Scheduler Starting");
        }

        public async Task SchedulerShutdown(CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation("Scheduler Shutdown");
        }

        public async Task SchedulerShuttingdown(CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation("Scheduler Shutting down");
        }

        public async Task SchedulingDataCleared(CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation("Scheduling Data Cleared");
        }

    }
}
