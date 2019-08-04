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

namespace BoardGameShopper.Bootstrap
{
    public class Program
    {
        public static ServiceProvider serviceProvider;

        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var services = new ServiceCollection();
            services.AddDbContext<DataContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            serviceProvider = services.BuildServiceProvider();

            var watch = Stopwatch.StartNew();

            var games = new GameologyCrawler(null).GetGames(1);
            InsertGames(games);
            //var games = new AdventGamesCrawler().GetGames(1);
            //games = new BoardGameMasterCrawler().GetGames(1);
            //InsertGames(conn, games);
            //games = new GameologyCrawler().GetGames(1);
            //InsertGames(conn, games);


            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine("Done!");
            //Console.ReadLine();
        }

        private static void InsertGames(IEnumerable<Game> games)
        {
            using (var context = serviceProvider.GetService<DataContext>())
            {
                context.Games.AddRange(games);
                context.SaveChanges();
            }
        }
    }
}
