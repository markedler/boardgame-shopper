using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using BoardGameShopper.Domain.Models;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

namespace BoardGameShopper.Domain.Crawlers
{
    public class GameologyCrawler : SiteCrawlerBase
    {
        public GameologyCrawler(DataContext dataContext) : base(dataContext)
        {
        }

        public override string SiteCode => Constants.SiteCode.Gameology;
        public override Dictionary<string, string> BaseUrls => new Dictionary<string, string>
        {
            ["Board Games"] = $"{RootUrl}/collections/board-game?_=pf&sort=title-ascending&page={0}",
            ["Card Games"] = $"{RootUrl}/collections/living-card-games?_=pf&sort=title-ascending&page={0}",
        };

        private string RootUrl = "https://www.gameology.com.au";

        public override List<HtmlNode> GetGameNodes(HtmlDocument html)
        {
            return html.DocumentNode.QuerySelectorAll(".product-grid-item").ToList();
        }

        public override Game ExtractGameFromNode(HtmlNode gameNode)
        {
            var name = gameNode?.QuerySelector("p")?.InnerText?.Trim();
            var priceNode = gameNode?.QuerySelector(".product-item-price>span");
            var price = ConvertPrice(_priceRegex.Replace(priceNode?.InnerText ?? string.Empty, string.Empty));
            var imageNode = gameNode?.QuerySelector(".product-grid-image>.product-grid-image--centered>img");
            var image = imageNode?.Attributes["src"]?.Value;
            var urlFragment = gameNode?.Attributes["href"]?.Value;

            string url = null;
            if (!string.IsNullOrWhiteSpace(urlFragment))
                url = RootUrl + urlFragment;

            return CreateGame(name, price ?? 0, image, url);
        }
    }
}
