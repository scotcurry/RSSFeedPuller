using Serilog;
using Microsoft.AspNetCore.Mvc;

using Datadog.Trace.Annotations;

namespace RSSFeedPuller.Controllers;

[ApiController]
[Route("[controller]")]
public class RSSFeedPullerController : ControllerBase
{
    private readonly ILogger<RSSFeedPullerController> _logger;
    private readonly IConfiguration Configuration;

    public RSSFeedPullerController(ILogger<RSSFeedPullerController> logger, IConfiguration configuration)
    {
        _logger = logger;
        Configuration = configuration;
    }

    [Trace(OperationName = "get.rssfeedlist", ResourceName = "RSSFeedPuller.Controllers.RSSFeedList.Get")]
    [HttpGet(Name = "RSSFeed")]
    public RSSFeedList? Get()
    {

        var logPath = System.Environment.GetEnvironmentVariable("LOG_PATH");
        if (logPath != null)
        {
            if (logPath.EndsWith("/"))
            {
                logPath = logPath[..^1];
            }
            logPath += "/log-.txt";
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Month)
            .CreateLogger();

            Log.Information("Log Path: " + logPath);
        } else
        {
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

            Log.Information("Log Path Was NULL!");
        }

        Log.Information("Getting RSS Feed");
        var httpHandler = new HttpHandler();
        var feedList = httpHandler.GetHttpData();
        if (feedList is null)
        {
            feedList = new RSSFeedList();
            var feed = new RSSFeed();
            feed.Author = "Error";
            feed.Link = "Error";
            feed.Title = "Error";
            feed.UpdatedTime = "Error";
        } else
        {
            var firstFeedItem = feedList?.FeedList?[0].Title;
            Log.Information("Latest Item: " + firstFeedItem);

            return feedList;
        }
        return feedList;
    }
}
