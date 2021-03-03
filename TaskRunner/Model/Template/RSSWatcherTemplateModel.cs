using System.Threading.Tasks;
using TaskRunner.Model.Configuration;
using TaskRunner.Model.Configuration.Task;
using TaskRunner.Model.Configuration.Template;
using TaskRunner.Model.RunnerTask;

namespace TaskRunner.Model.Template
{
    public class RSSWatcherTemplateModel : TaskTemplateBaseModel
    {

        public RSSWatcherTemplateConfig Config { get => GetConfig<RSSWatcherTemplateConfig>(); }
        public RSSWatcherTemplateModel(RSSWatcherTemplateConfig config, TemplateObservableCollection parent)
            : base(config, parent)
        {
            
        }

        public string ExecutablePath
        {
            get
            {
                return Config.ExecutablePath;
            }
            set
            {
                Config.ExecutablePath = value;
                OnPropertyChanged(nameof(ExecutablePath));
            }
        }

        public string CommandLine
        {
            get
            {
                return Config.CommandLine;
            }
            set
            {
                Config.CommandLine = value;
                OnPropertyChanged(nameof(CommandLine));
            }
        }


        public override TaskBase CreateTaskModel(TaskConfig config, TaskTreeItemBase parent)
        {
            return new TaskRSSWatcher(config, parent);
        }
    }
}
