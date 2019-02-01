using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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


    public class AuctionViewModelForm
    {

        

        

        public IPagedList<Auction> Auctions { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        
       

        [Display(Name = "Low Price")]
        public decimal? minprice { get; set; }

        [Display(Name = "High price")]
        public decimal? highprice { get; set; }

        public int? page { get; set; }

        [Display(Name = "Status")]
        public string status { get; set; }

        public int first=1;

    }


    
    


}