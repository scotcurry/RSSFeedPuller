using System.ComponentModel.DataAnnotations;

namespace RSSFeedPuller;

public class RSSFeedList
{
    public List<RSSFeed>? FeedList { get; set; }
    public string? CurrentLogTime { get; set; }
}

public class RSSFeed
{
    public string? UpdatedTime { get; set; }
    public string? Title { get; set; }
    public string? Link { get; set; }
    public string? Author { get; set; }
}
