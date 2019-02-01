using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using OnlineAuctions.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OnlineAuctions.Controllers
{
    public class AuctionController : Controller
    {
        private Model myDB = new Model();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        // GET: Auction
        public ActionResult Index(AuctionViewModelForm auction)
        {
            logger.Info("GET/Auction/Index action has been called");
            var auctions = from Auction in myDB.Auctions select Auction;

            

            int pageSize = myDB.SystemParameters.First().ItemsPerPage;
            int pageNumber = (auction.page ?? 1);
            ViewBag.Page = "Index";

            auction.Auctions = auctions.OrderByDescending(x=>x.createdOn).ToPagedList(pageNumber, pageSize);
            

            return View(auction);
        }


        public ActionResult Create()
        {
            logger.Info("GET/Auction/Create action has been called");
            if (Session["User"]!=null)
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
            logger.Info("POST/Auction/Create action has been called");
            if (Session["User"] == null)
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

                auction.createdOn = DateTime.UtcNow;

              


                auction.State = "READY";
                myDB.Auctions.Add(auction);
                myDB.SaveChanges();
                return RedirectToAction("UserPanel", "Accounts");
            }
            return View();
           
        }


        public ActionResult OpenAuction()
        {
            logger.Info("GET/Auction/OpenAuction action has been called");
            if (Session["Admin"]==null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);


            return View(myDB.Auctions.Where(x =>x.State!=null && x.State.Equals("READY")).ToList());
        }


        public ActionResult Open(int id)
        {
            logger.Info("GET/Auction/Open(id) action has been called");
            if (Session["Admin"] == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var auction = myDB.Auctions.Where(x => x.Id.Equals(id)).First();

            if (auction == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            auction.openingTime = DateTime.UtcNow;
            auction.closingTime = DateTime.UtcNow.AddSeconds(auction.Duration);
            auction.State = "OPENED";
            auction.currentPrice = auction.startingPrice;
            myDB.Entry(auction).State =EntityState.Modified;
            myDB.SaveChanges();
            myDB.Entry(auction).State = EntityState.Detached;

            return View("OpenAuction", myDB.Auctions.Where(x => x.State != null && x.State.Equals("READY")).ToList());
        }

        
        public ActionResult Search([Bind(Include = "Name,minprice,highprice,status,page")] AuctionViewModelForm auction)
        {
            logger.Info("GET/Auction/Search action has been called");
            var auctions = from Auction in myDB.Auctions select Auction;
            if(auction.Name!=null && auction.Name!="")
            {
                auctions = auctions.Where(x => x.Name.Contains(auction.Name));
            }
            
            if(auction.minprice!=null)
            {

                auctions = auctions.Where(x => x.currentPrice >= auction.minprice);
            }

            if(auction.highprice!=null)
            {
                auctions = auctions.Where(x => x.currentPrice <= auction.highprice);
            }

            if(auction.status != null && auction.status!="")
            {
                auctions = auctions.Where(x =>x.State.Equals(auction.status));
            }
           
            int pageSize = myDB.SystemParameters.First().ItemsPerPage;
            int pageNumber = (auction.page ?? 1);
            auction.Auctions =auctions.OrderByDescending(x=>x.createdOn).ToPagedList(pageNumber,pageSize);
            
            return View("Index", auction);
        }


        public ActionResult bla()
        {
            return View();
        }


        public ActionResult Details(int det)
        {
            logger.Info("GET/Auction/Details(det) action has been called");

            Auction auction;
            auction = myDB.Auctions.Find(det);
            if(auction==null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View(auction);
        }


        [HttpPost]
        public async Task<ActionResult> Bid(int AuctionId,int tokensnum)
        {
            logger.Info("POST/Auction/Bid(auctionId,tokensnum) action has been called");
            if (Session["User"] == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            using (var transaction = myDB.Database.BeginTransaction())
            {
                try
                {


                    User lastuser = null;
                    var auction = myDB.Auctions.Find(AuctionId);
                    if (auction == null)
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);

                    Bid lastbid = null;
                    if(auction.Bids.Count>0)
                         lastbid = auction.Bids.Last();


                    User user = (User)Session["User"];

                    int prevtokensnum;
                    if (lastbid == null)
                    {
                        
                        prevtokensnum = (int)auction.currentPrice;
                        if (tokensnum + auction.currentPrice > user.TokensNumber)
                        {
                            TempData["Message"]= "Not enough tokens!";
                            return RedirectToAction("Search");
                        }
                        else
                        {
                            user.TokensNumber -= (prevtokensnum + tokensnum);
                            myDB.Entry(user).State =EntityState.Modified;
                            

                        }



                    }
                    else
                    {
                        prevtokensnum = (int)lastbid.TokensNumber;
                        if (lastbid.idUser == user.IdUser)
                        {

                            if (tokensnum > user.TokensNumber)
                            {
                                TempData["Message"] = "Not enough tokens!";
                                return RedirectToAction("Search");
                            }
                            else
                            {
                                user.TokensNumber -= tokensnum;
                                myDB.Entry(user).State = EntityState.Modified;
                                
                            }
                        }
                        else
                        { 
                            
                            if (lastbid.TokensNumber + tokensnum > user.TokensNumber)
                            {
                                TempData["Message"] = "Not enough tokens!";
                                return RedirectToAction("Search");
                            }
                            else
                            {
                                user.TokensNumber -= (prevtokensnum + tokensnum);
                                myDB.Entry(user).State = EntityState.Modified;
                                
                                 lastuser = myDB.Users.Find(lastbid.idUser);
                                lastuser.TokensNumber += lastbid.TokensNumber;
                                myDB.Entry(lastuser).State = EntityState.Modified;
                                
                            }
                        }
                    }




                    Bid newbid = new Bid();

                    newbid.idAuction = AuctionId;
                    newbid.idUser = user.IdUser;
                    newbid.TokensNumber = prevtokensnum + tokensnum;
                    newbid.BiddingTime = DateTime.UtcNow;
                    int tokk = (int)newbid.TokensNumber;
                    auction.Bids.Add(newbid);
                    auction.currentPrice = tokk;
                    auction.Lastbidder=user.Firstname + " " + user.Lastname;
                    auction.LastbidderId = user.IdUser;

                    myDB.Bids.Add(newbid);


                    await myDB.SaveChangesAsync();
                    myDB.Entry(user).State = EntityState.Detached;
                    if (lastuser != null)
                    {
                        myDB.Entry(lastuser).State = EntityState.Detached;
                    }
                    transaction.Commit();


                    
                    

                    Hubs.MyHub.BidRefresh(AuctionId, tokk, user.Firstname + " " + user.Lastname);
                  //  hubContext.Clients.All.Hubfunc(AuctionId, tokk, user.Firstname+" "+user.Lastname);
                    return RedirectToAction("Search");


                }
                catch(Exception e)
                {
                    
                    transaction.Rollback();
                    return new HttpStatusCodeResult(HttpStatusCode.NotAcceptable, e.Message);
                }

            }

        }


        public ActionResult Close(int AuctionId)
        {
            logger.Info("GET/Auction/Close(AuctionId) action has been called");
            Auction auction = myDB.Auctions.Find(AuctionId);
            
                   
            auction.State = "COMPLETED";
            


            myDB.SaveChanges();
            

                    
            return RedirectToAction("Index");
                
        }


    }
}