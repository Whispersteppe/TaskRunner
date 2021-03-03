using System;
using System.Collections.Generic;
using System.Text;

namespace TaskRunner.Utility
{
    public static class DynamicFormat
    {
        public static string FormatDynamic(this string formatString, Dictionary<string, object> parameters)
        {
            List<object> values = new List<object>();

            int currentIdx = -1;
            foreach(string key in parameters.Keys)
            {
                values.Add(parameters[key]);
                currentIdx++;

                formatString = formatString.Replace($"{{{key}", $"{{{currentIdx}");
            
            }

            string formattedString = String.Format(formatString, values.ToArray());
            return formattedString;
        }
    }
}
