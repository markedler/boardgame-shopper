using System;
using System.Collections.Generic;
using System.Text;

namespace BoardGameShopper.Domain.Constants
{
    public enum StockStatus
    {
        InStock = 1,
        OutOfStock = 2,
        PreOrder = 3,
        Unknown = 99
    }
}
