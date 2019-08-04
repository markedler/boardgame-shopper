using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using BoardGameShopper.Domain.Constants;

namespace BoardGameShopper.Domain.Models
{
    public class Game
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public double CurrentPrice { get; set; }

        public double PreviousPrice { get; set; }

        public StockStatus StockStatus { get; set; }

        public Guid SiteId { get; set; }

        [ForeignKey(nameof(SiteId))]
        public Site Site { get; set; }

        public string Image { get; set; }

        public string Url { get; set; }
    }
}
