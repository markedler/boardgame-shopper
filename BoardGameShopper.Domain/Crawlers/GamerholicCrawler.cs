using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoardGameShopper.Domain.Models;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

namespace BoardGameShopper.Domain.Crawlers
{
    public class GamerholicCrawler : SiteCrawlerBase
    {
        public override string SiteCode => Constants.SiteCode.Gamerholic;
        
        public override Dictionary<string, string> BaseUrls => new Dictionary<string, string>
        {
            ["Board Games"] = "http://www.ebaystores.com.au/Collectables-Australia/_i.html"
        };

        public GamerholicCrawler(DataContext dataContext) : base(dataContext)
        {
        }

        public override List<HtmlNode> GetGameNodes(HtmlDocument html)
        {
            return html.DocumentNode.QuerySelectorAll("table[itemprop='offers']").ToList();
        }

        public override Game ExtractGameFromNode(HtmlNode gameNode)
        {
            var name = gameNode.QuerySelector("a[itemprop='name']")?.InnerText?.Trim();
            var priceNode = gameNode.QuerySelector("span[itemprop='price']");
            var price = ConvertPrice(_priceRegex.Replace(priceNode?.InnerText ?? string.Empty, string.Empty));
            var imageNode = gameNode?.QuerySelector("img[itemprop='image']");
            var image = imageNode?.Attributes["src"]?.Value;
            var urlNode = gameNode?.QuerySelector("a[itemprop='name']");
            var url = urlNode?.Attributes["href"]?.Value;

            return CreateGame(name, price ?? 0, image, url);
        }
    }
}
