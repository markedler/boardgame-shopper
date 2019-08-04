using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoardGameShopper.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameShopper.Website.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly DataContext dataContext;

        public BaseController(DataContext context)
        {
            dataContext = context;
        }
    }
}