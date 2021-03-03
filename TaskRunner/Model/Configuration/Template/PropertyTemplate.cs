namespace TaskRunner.Model.Configuration.Template
{

    public enum PropertyTemplateType
    {
        String,
        Number,
        Date,
        Url,
        FilePath
    }

    public class PropertyTemplate
    {
        public string Name { get; set; }
        public string DefaultValue { get; set; }
        public int Order { get; set; }
        public PropertyTemplateType PropertyType { get; set; } = PropertyTemplateType.String;

    }
}
