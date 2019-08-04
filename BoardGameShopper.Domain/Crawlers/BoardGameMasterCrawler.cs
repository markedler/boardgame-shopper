using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using BoardGameShopper.Domain.Constants;
using BoardGameShopper.Domain.Models;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace BoardGameShopper.Domain.Crawlers
{
    public class BoardGameMasterCrawler : ISiteCrawler
    {
        private DataContext _dataContext;
        public BoardGameMasterCrawler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public List<Game> GetGames(int? maxPages = null, bool trace = false)
        {
            var pages = maxPages ?? 999;

            var site = _dataContext.Sites.SingleOrDefault(x => x.UniqueCode == SiteCode.BoardGameMaster);

            var games = new List<Game>();
            var baseUrl = "https://app.globosoftware.net/filter/filter?shop=boardgame-master.myshopify.com&sort_by=title-ascending&event=all&page={0}";

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.95 Safari/537.36");

            for (var i = 1; i <= pages; i++)
            {
                var url = string.Format(baseUrl, i);
                var result = client.GetStringAsync(url).Result;

                dynamic json = JValue.Parse(result);

                if (json != null && json.products != null && json.products.Count > 0)
                {
                    foreach (var product in json.products)
                    {
                        double.TryParse(product.price?.ToString(), out double currentPrice);

                        if (!double.TryParse(product.compare_at_price?.ToString(), out double previousPrice))
                            previousPrice = currentPrice;

                        var game = new Game
                        {
                            Id = Guid.NewGuid(),
                            SiteId = site.Id,
                            Name = product.title,
                            CurrentPrice = currentPrice,
                            PreviousPrice = previousPrice,
                            StockStatus = product.available == "1" ? Constants.StockStatus.InStock : Constants.StockStatus.OutOfStock
                        };
                        games.Add(game);
                    }
                }
                else
                    break;
            }

            return games;
        }
    }
}
