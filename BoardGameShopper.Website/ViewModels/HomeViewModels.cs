using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoardGameShopper.Website.ViewModels
{
    public class HomeViewModels
    {
        public class Index
        {
            public IEnumerable<GameItem> Games { get; set; }

            public class GameItem
            {
                public Guid Id { get; set; }

                public string Name { get; set; }

                public double Price { get; set; }

                public string SiteName { get; set; }
            }
        }
    }
}
