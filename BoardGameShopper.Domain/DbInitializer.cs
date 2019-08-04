using BoardGameShopper.Domain.Constants;
using BoardGameShopper.Domain.Crawlers;
using BoardGameShopper.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoardGameShopper.Domain
{
    public class DbInitializer
    {
        public static void Initialize(DataContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Sites.Any() && context.Games.Any())
            {
                return;   // DB has been seeded
            }

            context.Games.RemoveRange(context.Games);
            context.Sites.RemoveRange(context.Sites);
            context.SaveChanges();

            var gameology = new Site { Id = Guid.NewGuid(), UniqueCode = SiteCode.Gameology, Name = "Gameology", Url = "https://www.gameology.com.au" };
            var adventGames = new Site { Id = Guid.NewGuid(), UniqueCode = SiteCode.AdventGames, Name = "Advent Games", Url = "https://www.adventgames.com.au" };
            var boardGameMaster = new Site { Id = Guid.NewGuid(), UniqueCode = SiteCode.BoardGameMaster, Name = "Board Game Master", Url = "https://www.boardgamemaster.com.au" };

            var sites = new Site[]
            {
                gameology,
                adventGames,
                boardGameMaster
            };

            context.Sites.AddRange(sites);
            context.SaveChanges();

            //ISiteCrawler crawler;
            //List<Game> games;
            //Gameology sample
            //crawler = new GameologyCrawler(gameology);
            //games = crawler.GetGames(1);
            //context.Games.AddRange(games);

            ////Advent Games sample
            //crawler = new AdventGamesCrawler(adventGames);
            //games = crawler.GetGames(1);
            //context.Games.AddRange(games);

            //Board Game Master sample
            //crawler = new BoardGameMasterCrawler(context);
            //games = crawler.GetGames();
            //context.Games.AddRange(games);

            //context.SaveChanges();
        }
    }
}
