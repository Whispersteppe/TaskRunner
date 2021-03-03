using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaskRunner.Model.RunnerTask
{

    /// <summary>
    /// generic task job to find and call the task to do the actual work
    /// </summary>
    public class TaskJob : IJob
    {
        public string ID { get; set; }

        /// <summary>
        /// given an ID, find and execute the task
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Execute(IJobExecutionContext context)
        {
            var task = TaskRunnerController.Current.TaskTreeItems.FindTaskByID(ID);
            if (task != null)
            {
                await task.Execute(context);
            }
        }
    }
}
