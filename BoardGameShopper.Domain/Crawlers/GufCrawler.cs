using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoardGameShopper.Domain.Models;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

namespace BoardGameShopper.Domain.Crawlers
{
    public class GufCrawler : SiteCrawlerBase
    {
        public override string SiteCode => Constants.SiteCode.Guf;

        private string RootUrl = "https://guf.com.au/";

        protected override bool AllowsAsync => false;

        public override Dictionary<string, string> BaseUrls => new Dictionary<string, string>
        {
            //["All Games"] = RootUrl + "collections/all?page={0}"
            ["Board Games"] = RootUrl + "collections/board-games?page={0}"
        };

        public GufCrawler(DataContext dataContext) : base(dataContext)
        {
        }

        public override List<HtmlNode> GetGameNodes(HtmlDocument html)
        {
            return html.DocumentNode.QuerySelectorAll(".product-item-wapper").ToList();
        }

        public override Game ExtractGameFromNode(HtmlNode gameNode)
        {
            var name = gameNode.QuerySelector("a.title-5")?.InnerText?.Trim();
            var priceNode = gameNode.QuerySelector(".price, .price_sale");
            var price = ConvertPrice(_priceRegex.Replace(priceNode?.InnerText ?? string.Empty, string.Empty));
            var imageNode = gameNode?.QuerySelector("img.img-responsive");
            var image = imageNode?.Attributes["src"]?.Value;
            var urlNode = gameNode?.QuerySelector("a.title-5");
            var urlFragment = urlNode?.Attributes["href"]?.Value;

            string url = null;
            if (!string.IsNullOrWhiteSpace(urlFragment))
                url = RootUrl + urlFragment;

            return CreateGame(name, price ?? 0, image, url);
        }
    }
}
