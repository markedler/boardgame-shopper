using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using BoardGameShopper.Domain;
using BoardGameShopper.Website.ViewModels;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using BoardGameShopper.Website.App;
using Microsoft.EntityFrameworkCore;

namespace BoardGameShopper.Website.Controllers
{
    public class ListController : BaseController
    {
        public ListController(DataContext context) : base(context)
        {
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 30, string sort = "name", bool desc = false, string search = null)
        {
            ViewData["NameSort"] = sort == "name" ? "name_desc": "name";
            ViewData["PriceSort"] = sort == "price" ? "price_desc": "price";
            ViewData["StoreSort"] = sort == "store" ? "store_desc": "store";

            var model = new ListViewModels.Index();
            var query = dataContext.Games.AsQueryable();

            if (search != null)
            {
                query = query.Where(x => x.Name.Contains(search));
            }

            var games = query.Select(x => new ListViewModels.Index.GameItem
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.CurrentPrice,
                Store = x.Site.Name,
                StoreId = x.SiteId,
                Image = x.Image,
                Url = x.Url
            }).AsQueryable();

            var orderBy = string.Format("{0}_{1}", sort, desc ? "desc" : "asc");
            switch (orderBy) {
                case "name_desc":
                    games = games.OrderByDescending(x => x.Name);
                    break;
                case "price_asc":
                    games = games.OrderBy(x => x.Price);
                    break;
                case "price_desc": 
                    games = games.OrderByDescending(x => x.Price).ThenBy(x => x.Name);
                    break;
                case "store_asc":
                    games = games.OrderBy(x => x.Store).ThenBy(x => x.Name);
                    break;
                case "store_desc": 
                    games = games.OrderByDescending(x => x.Store).ThenBy(x => x.Name);
                    break;
                default:
                    games = games.OrderBy(x => x.Name);
                    break;
            } 

            model.Games = await games.ToPagedListAsync(page, pageSize);

            model.PageSize = pageSize;
            model.Page = page;
            model.TotalCount = model.Games.TotalItemCount;
            model.TotalPages = model.Games.PageCount;
            model.Sort = sort;
            model.Desc = desc;
            model.Search = search;

            return View(model);
        }

    }
}