using System;
using System.Collections.Generic;
using System.Text;

namespace TaskRunner.Model.RunnerTrigger
{
    internal class RangeValue
    {


        public int Start;
        public int Stop;

        public override string ToString()
        {

            if (Start == Stop) return $"{Start}";
            return $"{Start}-{Stop}";

        }


        public static string RangeFromNumberList(List<int> values, bool isWeekDay)
        {
            //  could do L and # items in here, too.

            var ranges = FindAllRanges(values, isWeekDay);


            string valueString = string.Join(',', ranges);

            return valueString;
        }



        public static List<RangeValue> FindAllRanges(List<int> values, bool isWeekDay)
        {
            List<RangeValue> ranges = new List<RangeValue>();

            values.Sort();
            RangeValue currentRange = new RangeValue() { Start = values[0], Stop = values[0] };

            for (int i = 1; i < values.Count; i++)
            {
                if (values[i] == currentRange.Stop + 1)
                {
                    currentRange.Stop++;
                }
                else
                {
                    ranges.Add(currentRange);
                    currentRange = new RangeValue() { Start = values[i], Stop = values[i] };
                }
            }

            ranges.Add(currentRange);

            return ranges;
        }

    }
}
