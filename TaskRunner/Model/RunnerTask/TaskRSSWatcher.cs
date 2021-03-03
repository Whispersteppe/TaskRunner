using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TaskRunner.Model.Configuration;
using TaskRunner.Model.Configuration.Task;
using TaskRunner.Model.Configuration.Template;
using TaskRunner.Utility;

namespace TaskRunner.Model.RunnerTask
{
    /// <summary>
    /// Watch a RSS feed on a schedule, and when things change, launch the RSS item
    /// </summary>
    /// <remarks>
    /// the addition here will be to keep track of RSS items that have been already looked at, and only launch the new ones
    /// </remarks>
    public class TaskRSSWatcher : TaskBase
    {

        List<ISyndicationItem> RSSItems { get; set; }
        readonly RSSWatcherTemplateConfig template;


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

        public DateTime LatestPublicationDate
        {
            get
            {
                return Config.Properties.Keys.Contains("LatestPublishDate") == true 
                    ? DateTime.Parse(Config.Properties["LatestPublishDate"].ToString()) 
                    : DateTime.Now;

            }
            set
            {
                Config.Properties["LatestPublishDate"] = value;
                IsChanged = true;
                OnPropertyChanged(nameof(LatestPublicationDate));

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
                OnPropertyChanged(nameof(Url)) ;
            }
        }

        public TaskRSSWatcher(TaskConfig config, TaskTreeItemBase parent)
            :base(config, parent)
        {
            template = (RSSWatcherTemplateConfig)TaskRunnerController.Current.Config.Templates.First(x => x.ID == config.TemplateID);
            RSSItems = new List<ISyndicationItem>();
            //TODO will want to load this from the config
        }

        public override async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await LoadFeed();
            }
            catch (Exception e)
            {
                throw new JobExecutionException(e);
            }
        }

        private async Task LoadFeed()
        {
            DateTime newLatestPublishDate = LatestPublicationDate;

//            var currentItems = new List<ISyndicationItem>();

            HttpClient client = new HttpClient();
            string url = Url;

            var rslt = await client.GetAsync(url);
            if (rslt.IsSuccessStatusCode)
            {
                var rsltData = await rslt.Content.ReadAsStreamAsync();
                using (var xmlReader = XmlReader.Create(rsltData, new XmlReaderSettings() { Async = true }))
                {
                    var feedReader = new RssFeedReader(xmlReader);

                    while (await feedReader.Read())
                    {
                        switch(feedReader.ElementType)
                        {
                            // Read category
                            case SyndicationElementType.Category:
                                ISyndicationCategory category = await feedReader.ReadCategory();
                                break;

                            // Read Image
                            case SyndicationElementType.Image:
                                ISyndicationImage image = await feedReader.ReadImage();
                                break;

                            // Read Item
                            case SyndicationElementType.Item:
                                ISyndicationItem item = await feedReader.ReadItem();
                                if (item.Published > LatestPublicationDate)
                                {
                                    await LaunchItem(item);
                                    if (newLatestPublishDate < item.Published) newLatestPublishDate = item.Published.DateTime;
                                }
                                //currentItems.Add(item);

                                break;

                            // Read link
                            case SyndicationElementType.Link:
                                ISyndicationLink link = await feedReader.ReadLink();
                                break;

                            // Read Person
                            case SyndicationElementType.Person:
                                ISyndicationPerson person = await feedReader.ReadPerson();
                                break;

                            // Read content
                            default:
                                ISyndicationContent content = await feedReader.ReadContent();
                                break;
                        }
                    }
                }

                if (newLatestPublishDate > LatestPublicationDate)
                {
                    //                await SyncCurrentItems(currentItems);
                    LatestPublicationDate = newLatestPublishDate;
                }

            }
            else
            {
                //  we'll log it and go on.
            }
        }

        private async Task LaunchItem(ISyndicationItem item)
        {

            try
            {
                System.Threading.Thread.Sleep(5000);

                //  just do the first link for now
                if (item.Links.Count() > 0)
                {
                    var link = item.Links.First();

                    Dictionary<string, object> properties = new Dictionary<string, object>() { { "URL", link.Uri } };

                    string cmdLine = CommandLine.FormatDynamic(properties);

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
            }
            catch (Exception e)
            {
                throw new JobExecutionException(e);
            }

        }

        //private async Task SyncCurrentItems(List<ISyndicationItem> currentItems)
        //{

        //    var newItems = currentItems.Where(x => RSSItems.Any(y => y.Description == x.Description) == false).ToList();
        //    foreach (var newItem in newItems)
        //    {

        //        try
        //        {
        //            System.Threading.Thread.Sleep(5000);

        //            //  just do the first link for now
        //            if (newItem.Links.Count() > 0)
        //            {
        //                var link = newItem.Links.First();

        //                Dictionary<string, object> properties = new Dictionary<string, object>() { { "URL", link.Uri } };

        //                string cmdLine = CommandLine.FormatDynamic(properties);

        //                await Task.Run(() =>
        //                {

        //                    ProcessStartInfo startInfo = new ProcessStartInfo()
        //                    {
        //                        FileName = BrowserPath,
        //                        Arguments = cmdLine
        //                    };

        //                    Debug.WriteLine($"{startInfo.FileName} {startInfo.Arguments}");

        //                    var process = Process.Start(startInfo);
        //                });
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            throw new JobExecutionException(e);
        //        }


        //    }


        //    //  remove the old ones
        //    var removeItems = RSSItems.Where(x => currentItems.Any(y => y.Description == x.Description) == false).ToList();
        //    removeItems.ForEach(x => RSSItems.Remove(x));

        //    //  add in the new ones
        //    RSSItems.AddRange(newItems);
        //}

    }  //  TaskRSSWatcher
}
