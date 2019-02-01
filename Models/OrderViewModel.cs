using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineAuctions.Models
{
    public class OrderViewModel
    {
        public IPagedList<TokenOrder> orders;

        public int? page { get; set; }
    }
}