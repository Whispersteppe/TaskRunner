# TaskRunner

Say you want to go to a particular web site periodically.  no reason - maybe it's to check your finances, or 
look at new cat videos.  But it's something you do periodically, and don't want to forget.  This is where this app
will help you.  it allows you to set up and on a schedule pop up a web page in your default browser.  the schedule can be as complicated
as you wish it to be.  daily, every week on tuesday, every 4 hours between 8am and 5 pm on a week day, 
7:02am during the month of december (that's my christmas countdown one)

and you're not limited to web sites.  with some finagling with the templates, you can start any application on your workstation, open 
any document, or do someother action to a schedule.

This is written in C# .Net Core 3.1, using WPF as the UI.  It's using Quartz as the scheduling component, and stores things in a 
local json document (stored in the same directory).  it'll log as it goes along, and (hopefully) only keeps a certain number of days of logs and backups of the json file.

it won't catch up on launching things if it isn't running, and needs to be running to do things (it's not a service).  

Right now it gets updated whenever i'm wanting a new feature, which is usually when i tell myself "dang, it'd be nice if it did <this>". so if you have any suggestions, tell me.
  
  
