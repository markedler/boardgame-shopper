using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoardGameShopper.Domain;
using BoardGameShopper.Website.ViewModels;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace BoardGameShopper.Website.Controllers
{
    public class ListController : BaseController
    {
        public ListController(DataContext context) : base(context)
        {
        }

        public IActionResult Index(int page = 1, int pageSize = 30)
        {
            var model = new ListViewModels.Index();
            model.Games = dataContext.Games.OrderBy(x => x.Name).Select(x => new ListViewModels.Index.GameItem
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.CurrentPrice,
                SiteName = x.Site.Name,
                Image = x.Image,
                Url = x.Url
            }).ToPagedList(page, pageSize);

            model.PageSize = pageSize;
            model.Page = page;
            model.TotalCount = model.Games.TotalItemCount;
            model.TotalPages = model.Games.PageCount;

            return View(model);
        }
    }
}