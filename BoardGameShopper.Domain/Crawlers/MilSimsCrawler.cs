using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoardGameShopper.Domain.Models;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

namespace BoardGameShopper.Domain.Crawlers
{
    public class MilSimsCrawler : SiteCrawlerBase
    {
        public override string SiteCode => Constants.SiteCode.MilSims;

        private string RootUrl = "https://www.milsims.com.au/";

        public override Dictionary<string, string> BaseUrls => new Dictionary<string, string>
        {
            ["Board Games"] = RootUrl + "boardgames/?pgnum={0}",
            ["Card Games"] = RootUrl + "card-games/?pgnum={0}"
        };

        public MilSimsCrawler(DataContext dataContext) : base(dataContext)
        {
        }

        public override List<HtmlNode> GetGameNodes(HtmlDocument html)
        {
            return html.DocumentNode.QuerySelectorAll(".one-item-wrapper").ToList();
        }

        public override Game ExtractGameFromNode(HtmlNode gameNode)
        {
            var name = gameNode.QuerySelector("h3>a")?.InnerText?.Trim();
            var priceNode = gameNode.QuerySelector(".price>span[itemprop='price']");
            var price = ConvertPrice(_priceRegex.Replace(priceNode?.InnerText ?? string.Empty, string.Empty));
            var imageNode = gameNode?.QuerySelector(".thumbnail-image-holder>img");
            var imageFragment = imageNode?.Attributes["src"]?.Value;
            var urlNode = gameNode?.QuerySelector(".thumbnail-image-holder");
            var url = urlNode?.Attributes["href"]?.Value;

            string image = null;
            if (!string.IsNullOrWhiteSpace(imageFragment))
                image = RootUrl + imageFragment;

            return CreateGame(name, price ?? 0, image, url);
        }
    }
}
