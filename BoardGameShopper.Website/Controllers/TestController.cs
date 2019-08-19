using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoardGameShopper.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoardGameShopper.Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ApiBaseController
    {
        public TestController(Domain.DataContext context) : base(context)
        {
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Game>>> GetGames()
        {
            return await dataContext.Games.ToListAsync();
        }
    }
}