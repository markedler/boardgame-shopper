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
    public class GamerholicCrawler : SiteCrawlerBase
    {
        public override string SiteCode => Constants.SiteCode.Gamerholic;
        
        public override Dictionary<string, string> BaseUrls => new Dictionary<string, string>
        {
            ["Board Games"] = "http://www.ebaystores.com.au/Collectables-Australia/_i.html?rt=nc&_sid=1103520273&_trksid=p4634.c0.m14.l1513&_pgn={0}"
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
            var name = WebUtility.HtmlDecode(gameNode.QuerySelector("a[itemprop='name']")?.InnerText?.Trim());
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
