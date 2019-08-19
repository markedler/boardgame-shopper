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
    public class PolymorphGamesCrawler : SiteCrawlerBase
    {
        public override string SiteCode => Constants.SiteCode.PolymorphGames;

        private string RootUrl = "https://polymorphgames.com.au/";

        public override Dictionary<string, string> BaseUrls => new Dictionary<string, string>
        {
            ["Board Games"] = RootUrl + "index.php?route=product/category&path=280&page={0}"
        };

        public PolymorphGamesCrawler(DataContext dataContext) : base(dataContext)
        {
        }

        public override List<HtmlNode> GetGameNodes(HtmlDocument html)
        {
            return html.DocumentNode.QuerySelectorAll(".product-thumb").ToList();
        }

        public override Game ExtractGameFromNode(HtmlNode gameNode)
        {
            var name = gameNode.QuerySelector(".caption>h4>a")?.InnerText?.Trim();
            var priceNode = gameNode.QuerySelector(".price");
            var price = ConvertPrice(_priceRegex.Replace(priceNode?.InnerText ?? string.Empty, string.Empty));
            var imageNode = gameNode?.QuerySelector(".image>a>img");
            var image = imageNode?.Attributes["src"]?.Value;
            var urlNode = gameNode?.QuerySelector(".image>a");
            var url = WebUtility.HtmlDecode(urlNode?.Attributes["href"]?.Value);
            
            return CreateGame(name, price ?? 0, image, url);
        }
    }
}
