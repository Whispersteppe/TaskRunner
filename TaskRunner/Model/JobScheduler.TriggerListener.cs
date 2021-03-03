using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading;
using System.Threading.Tasks;

namespace TaskRunner.Model
{
    public class TRTriggerListener : ITriggerListener
    {

        ILogger _logger;
        JobScheduler _scheduler;
        public string Name { get; }

        public TRTriggerListener(string name, ILogger logger, JobScheduler scheduler)
        {
            Name = name;
            _logger = logger;
            _scheduler = scheduler;
        }


        public async Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation($"TriggerFired {context.JobDetail.Description}");
        }

        public async Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation($"VetoJobExecution {context.JobDetail.Description}");

            return false; //  do not veto;
        }

        public async Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => { });
            _logger.LogInformation($"TriggerMisfired {trigger.Description}");
        }

        public async Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"TriggerComplete {context.JobDetail.Description}");
            await _scheduler.ResetNextExecuteTime();
        }


    }
}
