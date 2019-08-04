﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace BoardGameShopper.Website.ViewModels
{
    public class ListViewModels
    {
        public class Index
        {
            public IPagedList<GameItem> Games { get; set; }

            public int PageSize { get; set; }

            public int Page { get; set; }

            public int TotalCount { get; set; }

            public int TotalPages { get; set; }

            public class GameItem
            {
                public Guid Id { get; set; }

                public string Name { get; set; }

                public double Price { get; set; }

                public string SiteName { get; set; }

                public string Image { get; set; }

                public string Url { get; set; }
            }
        }
    }
}