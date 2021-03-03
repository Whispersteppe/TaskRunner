using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskRunner.Model
{
    public class JobDetailInfo : ModelBase
    {

        public JobKey Key { get; private set; }
        IScheduler _scheduler;

        IJobDetail _jobDetail;
        ITrigger _triggerDetail;
        TriggerState _triggerState;

        public JobDetailInfo(JobKey jobKey, IScheduler scheduler)
        {
            Key = jobKey;
            _scheduler = scheduler;

        }

        public async Task JobDetailChanged()
        {
            _jobDetail = await _scheduler.GetJobDetail(Key);
            _triggerDetail = (await _scheduler.GetTriggersOfJob(Key)).First();
            _triggerState = await _scheduler.GetTriggerState(_triggerDetail.Key);

            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(JobDetail));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(NextExecuteTime));
            OnPropertyChanged(nameof(PreviousExecuteTime));
        }

        public TriggerState TriggerState
        {
            get
            {
                return _triggerState;
            }
        }


        public IJobDetail JobDetail 
        { 
            get
            {
                return _jobDetail;
            }

        }


        public ITrigger TriggerDetail
        {
            get
            {
                return _triggerDetail;
            }
        }

        public string Name => JobDetail.Key.Name;

        public string Description => JobDetail.Description;

        public DateTime? NextExecuteTime
        {
            get
            {
                if (TriggerDetail == null) return null;
                if (TriggerDetail.GetNextFireTimeUtc().HasValue == false) return null;
                return TriggerDetail.GetNextFireTimeUtc().Value.DateTime.ToLocalTime();
            }
        }

        public DateTime? PreviousExecuteTime
        {
            get
            {
                if (TriggerDetail == null) return null;
                if (TriggerDetail.GetPreviousFireTimeUtc().HasValue == false) return null;
                return TriggerDetail.GetPreviousFireTimeUtc().Value.DateTime.ToLocalTime();
            }
        }
    }
}
