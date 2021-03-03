using Quartz;
using System.Threading.Tasks;
using System.Linq;
using TaskRunner.Model.Configuration.Task;
using TaskRunner.Model.Configuration.Template;
using TaskRunner.Utility;
using System.Diagnostics;
using System;

namespace TaskRunner.Model.RunnerTask
{

    /// <summary>
    /// manage a task that will launch an executable
    /// </summary>
    public class TaskLaunchExecutable : TaskBase
    {
        readonly FileExecuteTemplateConfig template;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="parent"></param>
        public TaskLaunchExecutable(TaskConfig config, TaskTreeItemBase parent)
            :base(config, parent)
        {
            template = (FileExecuteTemplateConfig)TaskRunnerController.Current.Config.Templates.First(x => x.ID == config.TemplateID);
        }

        /// <summary>
        /// the command line for the executable
        /// </summary>
        public string CommandLine
        {
            get
            {
                return template.CommandLine;
            }
        }

        /// <summary>
        /// the full path to the executable
        /// </summary>
        public string BrowserPath
        {
            get
            {
                return template.ExecutablePath;
            }
        }

        public string Url
        {
            get
            {
                return Config.Properties.Keys.Contains("Url") == true
                    ? Config.Properties["Url"].ToString()
                    : "https://www.google.com";
            }
            set
            {
                Config.Properties["Url"] = value;
                OnPropertyChanged(nameof(Url));
            }
        }

        /// <summary>
        /// execute the command
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task Execute(IJobExecutionContext context)
        {
            try
            {
                string cmdLine = CommandLine.FormatDynamic(GetConfig<TaskConfig>().Properties);

                await Task.Run(() =>
                {

                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        FileName = BrowserPath,
                        Arguments = cmdLine
                    };

                    Debug.WriteLine($"{startInfo.FileName} {startInfo.Arguments}");

                    var process = Process.Start(startInfo);
                });
            }
            catch(Exception e)
            {
                throw new JobExecutionException(e);
            }
        }


    }
}
