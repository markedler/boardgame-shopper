using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BoardGameShopper.Domain.Models;
using HtmlAgilityPack;

namespace BoardGameShopper.Domain
{
    public interface ISiteCrawler
    {
        List<Game> GetGames(int? maxPages = null, bool trace = false);
    }
}
