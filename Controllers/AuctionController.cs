using OnlineAuctions.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OnlineAuctions.Controllers
{
    public class AuctionController : Controller
    {
        private Model myDB = new Model();

        // GET: Auction
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Create()
        {
            if (Session["User"] != null && Session["Admin"] == null)
            {

                return View();
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Duration,startingPrice,Image")] AuctionViewModel auctionView)
        {
            if (Session["User"] == null || Session["Admin"] != null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                Auction auction = new Auction();
                auction.Name = auctionView.Name;
                auction.Duration = auctionView.Duration;
                auction.startingPrice = auctionView.startingPrice;
                

                if (auctionView.Image != null)
                {
                 
                    auction.Img = new byte[auctionView.Image.ContentLength];
                    auctionView.Image.InputStream.Read(auction.Img, 0, auctionView.Image.ContentLength);

                }

                auction.createdOn = DateTime.Now;

              


                auction.State = "READY";
                myDB.Auctions.Add(auction);
                myDB.SaveChanges();
                return RedirectToAction("UserPanel", "Accounts");
            }
            return View();
           
        }


        


    }
}