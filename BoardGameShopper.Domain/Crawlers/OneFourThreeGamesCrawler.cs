using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoardGameShopper.Domain.Models;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

namespace BoardGameShopper.Domain.Crawlers
{
    public class OneFourThreeGamesCrawler : SiteCrawlerBase
    {
        public override string SiteCode => Constants.SiteCode.OneFourThreeGames;

        private string RootUrl = "http://www.143games.com.au/";

        public override Dictionary<string, string> BaseUrls => new Dictionary<string, string>
        {
            ["Board Games"] = RootUrl + "advanced_search_result.php?search_in_description=1&inc_subcat=1&pfrom=-1&sort=1a&page={0}"
        };

        public OneFourThreeGamesCrawler(DataContext dataContext) : base(dataContext)
        {
        }

        public override List<HtmlNode> GetGameNodes(HtmlDocument html)
        {
            return html.DocumentNode.QuerySelectorAll(".productHolder").ToList();
        }

        public override Game ExtractGameFromNode(HtmlNode gameNode)
        {
            var name = gameNode.QuerySelector("span[itemprop='name']")?.InnerText?.Trim();
            var priceNode = gameNode.QuerySelector("span[itemprop='price']");
            var price = ConvertPrice(_priceRegex.Replace(priceNode?.InnerText ?? string.Empty, string.Empty));
            var imageNode = gameNode?.QuerySelector("img[itemprop='image']");
            var imageFragment = imageNode?.Attributes["src"]?.Value;
            var urlNode = gameNode?.QuerySelector("a[itemprop='url']");
            var url = urlNode?.Attributes["href"]?.Value;

            string image = null;
            if (!string.IsNullOrWhiteSpace(imageFragment))
                image = RootUrl + imageFragment;

            return CreateGame(name, price ?? 0, image, url);
        }
    }
}
