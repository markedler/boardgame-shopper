using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BoardGameShopper.Domain.Models;
using HtmlAgilityPack;

namespace BoardGameShopper.Domain.Crawlers
{
    public abstract class SiteCrawlerBase : ISiteCrawler
    {
        private Site _site;

        protected Regex _priceRegex = new Regex("[^0-9.]");

        public abstract string SiteCode { get; }
        public abstract Dictionary<string, string> BaseUrls { get; }
        protected HtmlWeb Web { get; }
        protected List<Game> Games { get; set; }

        public DataContext DataContext { get; }

        protected virtual int StartPage => 1;

        //Some sites will block async calls
        protected virtual bool AllowsAsync => true;

        private readonly string UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.95 Safari/537.36";

        protected SiteCrawlerBase(DataContext dataContext)
        {
           DataContext = dataContext;

            Web = new HtmlWeb
            {
                OverrideEncoding = Encoding.UTF8,
                UserAgent = UserAgent
            };

            Games = new List<Game>();
            _site = DataContext.Sites.SingleOrDefault(x => x.UniqueCode == SiteCode);
        }
        
        public async Task<List<Game>> GetGames(int? maxPages = null, bool trace = false)
        {
            var pages = maxPages ?? 999;

            var games = new List<Game>();

            foreach (var baseUrl in BaseUrls)
            {
                Game firstGamePreviousPage = null, firstGameCurrentPage = null;
                for (var i = StartPage; i <= pages; i++)
                {
                    if (trace)
                        Console.WriteLine($"Querying page {i}/{pages} for {_site.Name} ({baseUrl.Key}).");
                    var url = string.Format(baseUrl.Value, i);

                    HtmlDocument html = null;
                    //Some sites will block async calls
                    if (AllowsAsync)
                        html = await Web.LoadFromWebAsync(url);
                    else
                        html = Web.Load(url);
                    html.DisableServerSideCode = true;
                    //run overriden method - get game nodes
                    var gameNodes = GetGameNodes(html);

                    if (!gameNodes.Any()) // Nothing! We've probably reached the end
                        break;

                    firstGamePreviousPage = firstGameCurrentPage;
                    firstGameCurrentPage = ExtractGameFromNode(gameNodes.First());

                    //items repeating means we have most likely reached the end of the list
                    if (firstGameCurrentPage.Equals(firstGamePreviousPage))
                        break;

                    foreach (var gameNode in gameNodes)
                    {
                        games.Add(ExtractGameFromNode(gameNode));
                    }
                    if (trace)
                        Console.WriteLine($"Completed page {i}/{pages} for {_site.Name} ({baseUrl.Key}).");
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
