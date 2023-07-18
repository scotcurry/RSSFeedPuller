using System.Xml;
using System.Globalization;

namespace RSSFeedPuller
{
	public class HttpHandler
	{
		public HttpHandler()
		{ }

		public RSSFeedList? GetHttpData()
		{
            var httpClient = new HttpClient();
            var feedURIBuilder = new UriBuilder("https", "www.theverge.com", 443, "rss/frontpage");
            var feedURI = feedURIBuilder.Uri;

			var xmlToSerialize = string.Empty;
            var response = httpClient.GetAsync(feedURI).Result;
			if (response.IsSuccessStatusCode)
			{
				xmlToSerialize = response.Content.ReadAsStringAsync().Result;
			}

            var feedList = parseXML(xmlToSerialize);
            return feedList;
        }

		private RSSFeedList? parseXML(string xmlToParse)
		{
			var rssFeedList = new RSSFeedList();
            var thisFeedList = new List<RSSFeed>();
            XmlDocument document = new XmlDocument();
		    document.LoadXml(xmlToParse);

			XmlNodeList entryNodes = document.GetElementsByTagName("entry");
			var childNodeCount = entryNodes.Count;
			for (var counter = 0; counter < childNodeCount; counter++)
			{
				
				var currentNode = entryNodes[counter];
				var dataNodes = currentNode?.ChildNodes;
				var updatedString = dataNodes?[1]?.InnerText;
				var title = dataNodes?[2]?.InnerText;
				var linkElement = dataNodes?[4]?.Attributes;
				var link = linkElement?[2].InnerText;
				var author = dataNodes?[6]?.FirstChild?.InnerText;

				var rssFeed = new RSSFeed();
				rssFeed.Author = author;
				rssFeed.Link = link;
				rssFeed.Title = title;

				rssFeed.UpdatedTime = updatedString?.Replace("T", " ");
				thisFeedList.Add(rssFeed);
			}

			var culture = new CultureInfo("en-us");
			rssFeedList.FeedList = thisFeedList;
			rssFeedList.CurrentLogTime = DateTime.Now.ToString(culture);

			return rssFeedList;
		}
	}
}

