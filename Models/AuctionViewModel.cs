using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineAuctions.Models
{
    public class AuctionViewModel
    {

        public AuctionViewModel()
        {

        }

        [Required]
        public string Name { get; set; }

        public int Duration { get; set; }

        [Required]
        public int startingPrice { get; set; }

        
        public HttpPostedFileBase Image { get; set; }
    }
}