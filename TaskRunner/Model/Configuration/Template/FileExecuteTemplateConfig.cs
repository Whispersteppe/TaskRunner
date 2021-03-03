namespace TaskRunner.Model.Configuration.Template
{
    public class FileExecuteTemplateConfig : TaskTemplateBaseConfig
    {

        public override TaskTemplateType TemplateType => TaskTemplateType.FileExecute;
        public string ExecutablePath { get; set; }
        public string CommandLine { get; set; }
    }
}
