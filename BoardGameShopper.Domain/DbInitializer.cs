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

            var dungeonCrawl = new Site { UniqueCode = SiteCode.DungeonCrawl, Name = "Dungeon Crawl", Url = "https://www.dungeoncrawl.com.au" };
            var gameology = new Site { UniqueCode = SiteCode.Gameology, Name = "Gameology", Url = "https://www.gameology.com.au" };
            var adventGames = new Site { UniqueCode = SiteCode.AdventGames, Name = "Advent Games", Url = "https://www.adventgames.com.au" };
            var boardGameMaster = new Site { UniqueCode = SiteCode.BoardGameMaster, Name = "Board Game Master", Url = "https://www.boardgamemaster.com.au" };
            var milsims = new Site { UniqueCode = SiteCode.MilSims, Name = "Milsims", Url = "https://www.milsims.com.au" };
            var gamerholic = new Site { UniqueCode = SiteCode.Gamerholic, Name = "Gamerholic", Url = "http://www.ebaystores.com.au/Collectables-Australia" };
            var oneFourThreeGames = new Site { UniqueCode = SiteCode.OneFourThreeGames, Name = "143 Games", Url = "http://www.143games.com.au" };
            var guf = new Site { UniqueCode = SiteCode.Guf, Name = "Guf", Url = "https://guf.com.au" };
            var polymorphGames = new Site { UniqueCode = SiteCode.PolymorphGames, Name = "Polymorph Games", Url = "https://polymorphgames.com.au" };
            var amazon = new Site { UniqueCode = SiteCode.Amazon, Name = "Amazon AU", Url = "https://amazon.com.au" };

            var sites = new Site[]
            {
                guf,
                dungeonCrawl,
                oneFourThreeGames,
                gameology,
                adventGames,
                boardGameMaster,
                milsims,
                gamerholic,
                polymorphGames,
                amazon
            };

            context.Sites.AddRange(sites);
            context.SaveChanges();
        }
    }
}
