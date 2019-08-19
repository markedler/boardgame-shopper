using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoardGameShopper.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameShopper.Website.Controllers
{
    public class StoreController : BaseController
    {
        public StoreController(DataContext context) : base(context)
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail()
        {
            return View();
        }
    }
}