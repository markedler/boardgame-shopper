using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoardGameShopper.Domain.Models;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

namespace BoardGameShopper.Domain.Crawlers
{
    public class DungeonCrawlCrawler : SiteCrawlerBase
    {
        public override string SiteCode => Constants.SiteCode.DungeonCrawl;

        private string RootUrl = "https://www.dungeoncrawl.com.au/";

        public override Dictionary<string, string> BaseUrls => new Dictionary<string, string>
        {
            ["Board Games"] = RootUrl + "board-games/?pgnum={0}"
        };

        public DungeonCrawlCrawler(DataContext dataContext) : base(dataContext)
        {
        }

        public override List<HtmlNode> GetGameNodes(HtmlDocument html)
        {
            return html.DocumentNode.QuerySelectorAll(".c_productThumbnail").ToList();
        }

        public override Game ExtractGameFromNode(HtmlNode gameNode)
        {
            var name = gameNode.QuerySelector("h3[itemprop='name']>a")?.InnerText?.Trim();
            var priceNode = gameNode.QuerySelector("span[itemprop='price']");
            var price = ConvertPrice(_priceRegex.Replace(priceNode?.InnerText ?? string.Empty, string.Empty));
            var imageNode = gameNode?.QuerySelector("img.product-image");
            var imageFragment = imageNode?.Attributes["src"]?.Value;
            var urlNode = gameNode?.QuerySelector("a");
            var url = urlNode?.Attributes["href"]?.Value;

            string image = null;
            if (!string.IsNullOrWhiteSpace(imageFragment))
                image = RootUrl + imageFragment;

            return CreateGame(name, price ?? 0, image, url);
        }
    }
}
