using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BoardGameShopper.Domain.Models;
using HtmlAgilityPack;

namespace BoardGameShopper.Domain.Crawlers
{
    public abstract class SiteCrawlerBase : ISiteCrawler
    {
        private DataContext _dataContext;

        private Site _site;

        protected Regex _priceRegex = new Regex("[^0-9.]");
        
        public abstract string SiteCode { get; }
        public abstract List<string> BaseUrls { get; }
        protected HtmlWeb Web { get; }
        protected List<Game> Games { get; set; }
        
        protected SiteCrawlerBase(DataContext dataContext)
        {
           _dataContext = dataContext;

            Web = new HtmlWeb
            {
                OverrideEncoding = Encoding.UTF8,
                UserAgent =
                    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.95 Safari/537.36"
            };

            Games = new List<Game>();
            _site = _dataContext.Sites.SingleOrDefault(x => x.UniqueCode == SiteCode);
        }
        
        public List<Game> GetGames(int? maxPages = null, bool trace = false)
        {
            var pages = maxPages ?? 999;

            var games = new List<Game>();

            foreach (var baseUrl in BaseUrls)
            {
                for (var i = 1; i <= pages; i++)
                {
                    if (trace)
                        Console.Write($"Querying page {i} for {_site.Name}...");
                    var url = string.Format(baseUrl, i);

                    var html = Web.Load(url);
                    html.DisableServerSideCode = true;
                    //run overriden method - get game nodes
                    var gameNodes = GetGameNodes(html);
                    if (!gameNodes.Any()) // Nothing! We've probably reached the end
                        break;
                    foreach (var gameNode in gameNodes)
                    {
                        games.Add(ExtractGameFromNode(gameNode));
                    }
                    if (trace)
                        Console.WriteLine("Done!");
                }
            }

            return games;
        }

        public abstract List<HtmlNode> GetGameNodes(HtmlDocument html);
        public abstract Game ExtractGameFromNode(HtmlNode gameNode);

        protected Game CreateGame(string name, double price, string image = null, string url = null)
        {
            return new Game
            {
                Id = Guid.NewGuid(),
                Name = name,
                CurrentPrice = price,
                SiteId = _site.Id,
                Image = image,
                Url = url
            };
        }

        protected double? ConvertPrice(string priceText)
        {
            if (double.TryParse(priceText?.Trim(), out var price))
                return price;

            return null;
        }
    }
}
