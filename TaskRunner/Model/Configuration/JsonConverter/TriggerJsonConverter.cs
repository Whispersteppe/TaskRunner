using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics.CodeAnalysis;
using TaskRunner.Model.Configuration;
using TaskRunner.Model.Configuration.Trigger;

namespace TaskRunner.Model.Configuration.JsonConverter
{
    public class TriggerJsonConverter : JsonConverter<TriggerConfig>
    {
        public override TriggerConfig ReadJson(JsonReader reader, Type objectType, [AllowNull] TriggerConfig existingValue, bool hasExistingValue, JsonSerializer serializer)
        {

            var jA = JObject.Load(reader);

            TriggerConfig baseObject = new TriggerConfig();
            serializer.Populate(jA.CreateReader(), baseObject);

            //  get the type
            switch (baseObject.TriggerType)
            {
                case TriggerType.SimpleTrigger:
                    baseObject = new TriggerSimpleConfig();
                    break;
                case TriggerType.DatePicker:
                    baseObject = new TriggerDatePickerConfig();
                    break;
                case TriggerType.CronTrigger:
                    baseObject = new TriggerCronConfig();
                    break;
                default:
                    break;
            }

            serializer.Populate(jA.CreateReader(), baseObject);

            return baseObject;

        }

        public override void WriteJson(JsonWriter writer, [AllowNull] TriggerConfig value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;
    }
}
