using System;
using System.Collections.Generic;
using System.Text;

namespace BoardGameShopper.Domain.Models
{
    public class Site
    {
        public Guid Id { get; set; }

        public string UniqueCode { get; set;
        }
        public string Name { get; set; }

        public string Url { get; set; }

        public IEnumerable<Game> Games { get; set; }

    }
}
