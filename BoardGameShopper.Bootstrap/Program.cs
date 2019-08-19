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
using System.Threading.Tasks;

namespace BoardGameShopper.Bootstrap
{
    public class Program
    {
        public static ServiceProvider serviceProvider;

        private static readonly int? NumPages = 5;
        private const bool Trace = true;

        public class Options {
            [Option('c', "clean", Required = false, HelpText = "Cleans the database before importing.")]
            public bool Clean {get;set;}
        }

        public static async Task Main(string[] args)
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
                
                var tasks = new Task[]
                {
                    GetGames(new AmazonCrawler(new DataContext(optionsBuilder.Options))),
                    GetGames(new PolymorphyGamesCrawler(new DataContext(optionsBuilder.Options))),
                    GetGames(new GufCrawler(new DataContext(optionsBuilder.Options))),
                    GetGames(new DungeonCrawlCrawler(new DataContext(optionsBuilder.Options))),
                    GetGames(new OneFourThreeGamesCrawler(new DataContext(optionsBuilder.Options))),
                    GetGames(new GamerholicCrawler(new DataContext(optionsBuilder.Options))),
                    GetGames(new MilSimsCrawler(new DataContext(optionsBuilder.Options))),
                    GetGames(new GameologyCrawler(new DataContext(optionsBuilder.Options))),
                    GetGames(new AdventGamesCrawler(new DataContext(optionsBuilder.Options))),
                    GetGames(new BoardGameMasterCrawler(new DataContext(optionsBuilder.Options)))
                };

                await Task.WhenAll(tasks);

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine($"Completed in {elapsedMs/1000} seconds");
                //Console.ReadLine();
            }
        }

        private static async Task GetGames(ISiteCrawler crawler)
        {
            var games = await crawler.GetGames(NumPages, Trace);
            crawler.DataContext.Games.AddRange(games);
            await crawler.DataContext.SaveChangesAsync();
        }
    }
}
