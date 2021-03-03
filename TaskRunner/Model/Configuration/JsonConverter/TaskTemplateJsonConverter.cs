using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics.CodeAnalysis;
using TaskRunner.Model.Configuration;
using TaskRunner.Model.Configuration.Template;

namespace TaskRunner.Model.Configuration.JsonConverter
{
    public class TaskTemplateJsonConverter : JsonConverter<TaskTemplateBaseConfig>
    {
        public override TaskTemplateBaseConfig ReadJson(JsonReader reader, Type objectType, [AllowNull] TaskTemplateBaseConfig existingValue, bool hasExistingValue, JsonSerializer serializer)
        {

            var jA = JObject.Load(reader);

            TaskTemplateBaseConfig baseObject = new TaskTemplateBaseConfig();
            serializer.Populate(jA.CreateReader(), baseObject);

            //  get the type
            switch (baseObject.TemplateType)
            {
                case TaskTemplateType.FileExecute:
                    baseObject = new FileExecuteTemplateConfig();
                    break;
                case TaskTemplateType.RSSWatcher:
                    baseObject = new RSSWatcherTemplateConfig();
                    break;
                default:
                    break;
            }

            serializer.Populate(jA.CreateReader(), baseObject);

            return baseObject;

        }

        public override void WriteJson(JsonWriter writer, [AllowNull] TaskTemplateBaseConfig value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;
    }
}
