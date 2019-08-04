﻿using BoardGameShopper.Domain;
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
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            using (var dataContext = new DataContext(optionsBuilder.Options))
            {
                var games = new GameologyCrawler(dataContext).GetGames(2, true);
                InsertGames(games, dataContext);
                games = new AdventGamesCrawler(dataContext).GetGames(2, true);
                InsertGames(games, dataContext);
                //games = new BoardGameMasterCrawler(dataContext).GetGames(2, true);
                //InsertGames(games, dataContext);
                
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
