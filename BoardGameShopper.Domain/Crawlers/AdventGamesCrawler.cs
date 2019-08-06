using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoardGameShopper.Domain.Models;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace BoardGameShopper.Domain.Crawlers
{
    public class AdventGamesCrawler : SiteCrawlerBase
    {
        public override string SiteCode => Constants.SiteCode.AdventGames;
        public override Dictionary<string, string> BaseUrls => new Dictionary<string, string>
        {
            ["Games A-K"] = "http://www.adventgames.com.au/Listing/Category/?categoryId=4504822&page={0}&sortItem=2&sortDirection=0",
            ["Games L-Z"] = "http://www.adventgames.com.au/Listing/Category/?categoryId=4553164&page={0}&sortItem=2&sortDirection=0"
        };

        public AdventGamesCrawler(DataContext dataContext) : base(dataContext)
        {
        }

        public override List<HtmlNode> GetGameNodes(HtmlDocument html)
        {
            return html.DocumentNode.QuerySelectorAll(".DataViewCell").ToList();
        }

        public override Game ExtractGameFromNode(HtmlNode gameNode)
        {
            var name = gameNode.QuerySelector(".DataViewItemProductTitle>a")?.InnerText?.Trim();
            var priceNode = gameNode.QuerySelector(".DataViewItemOurPrice");
            var price = ConvertPrice(_priceRegex.Replace(priceNode?.InnerText ?? string.Empty, string.Empty));
            var imageNode = gameNode?.QuerySelector(".DataViewItemThumbnailImage>div>a>img");
            var image = imageNode?.Attributes["data-src"]?.Value;
            return CreateGame(name, price ?? 0, image);
        }
    }
}
