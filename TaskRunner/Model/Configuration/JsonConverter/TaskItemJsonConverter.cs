using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TaskRunner.Model.Configuration;
using TaskRunner.Model.Configuration.Task;

namespace TaskRunner.Model.Configuration.JsonConverter
{
    public class TaskItemJsonConverter : JsonConverter<TaskBaseConfig>
    {
        public override TaskBaseConfig ReadJson(JsonReader reader, Type objectType, [AllowNull] TaskBaseConfig existingValue, bool hasExistingValue, JsonSerializer serializer)
        {

            JObject jA = JObject.Load(reader);

            TaskBaseConfig baseObject;
            if (jA.ContainsKey("ChildItems") == true)
            {
                baseObject = new TaskFolderConfig();
            }
            else
            {
                baseObject = new TaskConfig();
            }

            serializer.Populate(jA.CreateReader(), baseObject);

            return baseObject;

        }

        public override void WriteJson(JsonWriter writer, [AllowNull] TaskBaseConfig value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;
    }
}
