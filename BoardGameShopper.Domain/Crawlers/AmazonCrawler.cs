using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoardGameShopper.Domain.Models;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using System.Net;

namespace BoardGameShopper.Domain.Crawlers
{
    public class AmazonCrawler : SiteCrawlerBase
    {
        public override string SiteCode => Constants.SiteCode.Amazon;

        protected override int StartPage => 2;

        private string RootUrl = "https://www.amazon.com.au/";

        public override Dictionary<string, string> BaseUrls => new Dictionary<string, string>
        {
            ["Board Games"] = RootUrl + "s?rh=n%3A4852617051%2Cn%3A%214852618051%2Cn%3A5030684051&page={0}&qid=1566186057&s=price-asc-rank&ref=sr_st_price-asc-rank"
        };

        public AmazonCrawler(DataContext dataContext) : base(dataContext)
        {
        }

        public override List<HtmlNode> GetGameNodes(HtmlDocument html)
        {
            var nodes = html.DocumentNode.QuerySelectorAll(".s-result-item").ToList();
            return nodes;                
        }

        public override Game ExtractGameFromNode(HtmlNode gameNode)
        {
            var name = WebUtility.HtmlDecode(gameNode.QuerySelector("h2>a>span")?.InnerText?.Trim());

            if (name == null)
                return null;

            var priceNode = gameNode.QuerySelector(".a-price>.a-offscreen");
            var price = ConvertPrice(_priceRegex.Replace(priceNode?.InnerText ?? string.Empty, string.Empty));
            var imageNode = gameNode?.QuerySelector("img[data-image-latency='s-product-image']");
            var image = imageNode?.Attributes["src"]?.Value;
            var urlNode = gameNode?.QuerySelector("h2>a");
            var urlFragment = urlNode?.Attributes["href"]?.Value;

            string url = null;
            if (!string.IsNullOrWhiteSpace(urlFragment))
                url = RootUrl + urlFragment;

            return CreateGame(name, price ?? 0, image, url);
        }
    }
}
