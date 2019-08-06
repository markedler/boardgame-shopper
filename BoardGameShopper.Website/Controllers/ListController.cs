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

        public async Task<IActionResult> Index(int page = 1, int pageSize = 30, string sort = "name", bool desc = false)
        {
            ViewData["NameSort"] = sort == "name" ? "name_desc": "name";
            ViewData["PriceSort"] = sort == "price" ? "price_desc": "price";
            ViewData["StoreSort"] = sort == "store" ? "store_desc": "store";

            var model = new ListViewModels.Index();
            var query = dataContext.Games
            .Select(x => new ListViewModels.Index.GameItem
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.CurrentPrice,
                Store = x.Site.Name,
                Image = x.Image,
                Url = x.Url
            }).AsQueryable();

            var orderBy = string.Format("{0}_{1}", sort, desc ? "desc" : "asc");
            switch (orderBy) {
                case "name_desc":
                    query = query.OrderByDescending(x => x.Name);
                    break;
                case "price_asc":
                    query = query.OrderBy(x => x.Price);
                    break;
                case "price_desc": 
                    query = query.OrderByDescending(x => x.Price).ThenBy(x => x.Name);
                    break;
                case "store_asc":
                    query = query.OrderBy(x => x.Store).ThenBy(x => x.Name);
                    break;
                case "store_desc": 
                    query = query.OrderByDescending(x => x.Store).ThenBy(x => x.Name);
                    break;
                default:
                    query = query.OrderBy(x => x.Name);
                    break;
            } 

            model.Games = await query.ToPagedListAsync(page, pageSize);

            model.PageSize = pageSize;
            model.Page = page;
            model.TotalCount = model.Games.TotalItemCount;
            model.TotalPages = model.Games.PageCount;
            model.Sort = sort;
            model.Desc = desc;

            return View(model);
        }

    }
}