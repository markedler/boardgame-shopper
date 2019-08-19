using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BoardGameShopper.Website.Models;
using BoardGameShopper.Domain;
using BoardGameShopper.Website.ViewModels;

namespace BoardGameShopper.Website.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(DataContext context) : base(context)
        {
        }

        public IActionResult Index()
        {
            var model = new HomeViewModels.Index
            {
                Games = dataContext.Games.OrderBy(x => Guid.NewGuid()).Take(20).Select(x => new HomeViewModels.Index.GameItem
                {
                    Id = x.Id,
                    Image = x.Image,
                    Name = x.Name,
                    Price = x.CurrentPrice,
                    SiteName = x.Site.Name
                })
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
