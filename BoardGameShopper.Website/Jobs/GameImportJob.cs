using BoardGameShopper.Domain;
using BoardGameShopper.Domain.Constants;
using BoardGameShopper.Domain.Crawlers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameShopper.Website.Jobs
{
    public class GameImportJob
    {
        private readonly DataContext _dataContext;

        public GameImportJob(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task Execute(string siteCode, int? maxPages)
        {
            ISiteCrawler crawler = null;

            switch (siteCode)
            {
                case SiteCode.AdventGames:
                    crawler = new AdventGamesCrawler(_dataContext);
                    break;
                case SiteCode.BoardGameMaster:
                    crawler = new BoardGameMasterCrawler(_dataContext);
                    break;
                case SiteCode.Gameology:
                    crawler = new GameologyCrawler(_dataContext);
                    break;
                default:
                    break;
            }

            if (crawler == null)
                return;

            var games = crawler.GetGames(maxPages);

            await _dataContext.Games.AddRangeAsync(games);
            await _dataContext.SaveChangesAsync();
        }
    }
}
