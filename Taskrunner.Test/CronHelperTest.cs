using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TaskRunner.Model.RunnerTrigger;
using Xunit;
using Xunit.Abstractions;

namespace Taskrunner.Test
{
    public class CronHelperTest
    {
        internal ITestOutputHelper Output { get; set; }

        public CronHelperTest(ITestOutputHelper output)
        {
            Output = output;
        }

        //[Fact]
        //public void CheckCronHelper()
        //{
        //    CronHelper helper = new CronHelper("0 0 8 * * ?");
        //    Output.WriteLine(JsonConvert.SerializeObject(helper, Formatting.Indented));

        //    //helper.Hours.Items.Add(9);
        //    //helper.Hours.Items.Add(10);
        //    //helper.Hours.Items.Add(11);
        //    helper.HourString = "9,10,11,12";
        //    Output.WriteLine(JsonConvert.SerializeObject(helper, Formatting.Indented));

        //    helper.Days.Items.Add(1);
        //    helper.Days.Items.Add(3);
        //    helper.Days.Items.Add(5);
        //    helper.DayString = "1,3,5";
        //    Output.WriteLine(JsonConvert.SerializeObject(helper, Formatting.Indented));

        //    helper.Days.Items.Clear();
        //    helper.Weekdays.Items.Add(1);
        //    helper.Weekdays.Items.Add(3);
        //    helper.Weekdays.Items.Add(5);
        //    Output.WriteLine(JsonConvert.SerializeObject(helper, Formatting.Indented));




        //}


    }
}
