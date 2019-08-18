using BoardGameShopper.Domain;
using BoardGameShopper.Domain.Crawlers;
using BoardGameShopper.Domain.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using CommandLine;

namespace BoardGameShopper.Bootstrap
{
    public class Program
    {
        public static ServiceProvider serviceProvider;

        private static readonly int? NumPages = null;
        private const bool Trace = true;

        public class Options {
            [Option('c', "clean", Required = false, HelpText = "Cleans the database before importing.")]
            public bool Clean {get;set;}
        }

        public static void Main(string[] args)
        {
            var clean = true;
            //Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o => {
            //    clean = o.Clean;
            //});
            
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var services = new ServiceCollection();
            services.AddDbContext<DataContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            serviceProvider = services.BuildServiceProvider();

            var watch = Stopwatch.StartNew();
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));


            using (var dataContext = new DataContext(optionsBuilder.Options))
            {
                if (clean) {
                    Console.WriteLine("Deleting database...");
                    dataContext.Database.EnsureDeleted();
                    Console.WriteLine("Creating database...");
                    DbInitializer.Initialize(dataContext);
                }

                var crawlers = new List<ISiteCrawler>
                {
                    new GufCrawler(dataContext),
                    new DungeonCrawlCrawler(dataContext),
                    new OneFourThreeGamesCrawler(dataContext),
                    new GamerholicCrawler(dataContext),
                    new MilSimsCrawler(dataContext),
                    new GameologyCrawler(dataContext),
                    new AdventGamesCrawler(dataContext),
                    new BoardGameMasterCrawler(dataContext)
                };

                foreach (var crawler in crawlers)
                {
                    var games = crawler.GetGames(NumPages, Trace);
                    InsertGames(games, dataContext);
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine($"Completed in {elapsedMs/1000} seconds");
                //Console.ReadLine();
            }
        }

        private static void InsertGames(IEnumerable<Game> games, DataContext dataContext)
        {
            dataContext.Games.AddRange(games);
            dataContext.SaveChanges();
        }
    }
}
