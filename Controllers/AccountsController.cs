using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Text;
using System.Net;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using OnlineAuctions.Models;
using System.Collections;
using System.Collections.Generic;
using PagedList;
using System.Net.Mail;

namespace OnlineAuctions.Controllers
{

    public class AccountsController : Controller
    {
        private Model myDB = new Model();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Accounts
        public ActionResult Index()
        {
            return View();
        }


        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }


        public ActionResult Login()
        {
            logger.Info("GET/Accounts/Login action has been called");
            return View();
        }


        public ActionResult UserPanel()
        {
            logger.Info("GET/Accounts/UserPanel action has been called");
            if(Session["User"]==null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "Firstname, Lastname, Email, Password, TokensNumber, Role, IdUser")] User user)
        {
            logger.Info("POST/Accounts/Login action has been called");
            user.Password = CreateMD5(user.Password);
            var found = myDB.Users.Any(x => x.Email == user.Email && x.Password == user.Password);

            if (found)
            {
              
                var ourUser = myDB.Users.First(x=> x.Email==user.Email && x.Password==user.Password);
               
                if(ourUser.Role=="Admin")
                {
                    Session["Admin"] = ourUser;
                    return View("AdminPanel");
                }
                else if(ourUser.Role=="User")
                {
                    Session["User"] = ourUser;
                    myDB.Entry(ourUser).State = EntityState.Detached;
                    return View("UserPanel");
                }
                
            }
            ViewBag.Message = "User with this Email and Password doesn't exists!";
            return View("Login");

        }


        public ActionResult Register()
        {
            logger.Info("GET/Accounts/Register action has been called");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "Firstname, Lastname, Email, Password, TokensNumber, Role, IdUser")] User user)
        {
            logger.Info("POST/Accounts/Register action has been called");
            if (ModelState.IsValid)
            {
                var found = myDB.Users.Any(x => x.Email == user.Email);
                if (!found)
                {
                    user.TokensNumber = 0;
                    user.Role = "User";
                    user.Password = CreateMD5(user.Password);
                    myDB.Users.Add(user);
                    myDB.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.Message = "User with this Email already exists.";
                    return View("Register");
                }
            }
            ViewBag.Message = "Error occured somewhere in registration.";
            return View("Register");
        }


        public ActionResult Logout()
        {
            logger.Info("GET/Accounts/Logout action has been called");
            if(Session["User"]==null && Session["Admin"]==null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Session.Clear();
            return RedirectToAction("Index", "Auction");
        }


        public ActionResult UserInformation()
        {
            logger.Info("GET/Accounts/UserInformation action has been called");
            if (Session["User"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int idUser = ((User)Session["User"]).IdUser;


            User user = myDB.Users.Find(idUser);
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            return View(user);
        }

        public ActionResult EditProfile(int? id)
        {
            logger.Info("GET/Accounts/EditProfile(?id) action has been called");
            if (id == null || Session["User"] == null || Session["Admin"] != null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            User user = myDB.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile([Bind(Include = "Firstname, Lastname, Email, Password, TokensNumber, Role, IdUser")] User user)
        {
            logger.Info("POST/Accounts/EditProfile action has been called");
            if (Session["User"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!myDB.Users.Any(x => x.Email == user.Email && x.IdUser != user.IdUser))
            {
                user.Password = CreateMD5(user.Password);
                myDB.Entry(user).State = EntityState.Modified;
                bool saveFailed;
                do
                {
                    saveFailed = false;
                    try { myDB.SaveChanges();
                        
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        saveFailed = true;
                        // Update original values from the database 
                        var entry = ex.Entries.Single();
                        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    }
                } while (saveFailed);


                Session["User"] = user;
                myDB.Entry(user).State = EntityState.Detached;
                return RedirectToAction("UserInformation");
            }
            ViewBag.Message = "User with this mail exists!";
            return View(user);

        }

        public ActionResult OrderTokens()
        {
            logger.Info("GET/Accounts/OrderTokens action has been called");
            if (Session["User"]==null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);


            ViewBag.PlatniumPrice = (decimal)myDB.SystemParameters.First().Platnium * myDB.SystemParameters.First().TokensValue;
            ViewBag.GoldPrice= (decimal)myDB.SystemParameters.First().Gold * myDB.SystemParameters.First().TokensValue;
            ViewBag.SilverPrice=(decimal)myDB.SystemParameters.First().Silver * myDB.SystemParameters.First().TokensValue;
            ViewBag.Gold = myDB.SystemParameters.First().Gold;
            ViewBag.Platnium = myDB.SystemParameters.First().Platnium;
            ViewBag.Silver = myDB.SystemParameters.First().Silver;
            ViewBag.Currency = myDB.SystemParameters.First().Currency;
            return View();
        }

       
        public ActionResult OrderToken(int num)
        {
            logger.Info("GET/Accounts/OrderTokens(num) action has been called");
            if (Session["User"]==null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TokenOrder order = new TokenOrder();
            int tokens = 0;
            if(num==1)
            {
                tokens = (int)myDB.SystemParameters.First().Platnium;
            }
            else if(num==2)
            {
                tokens = (int)myDB.SystemParameters.First().Gold;
               
            }
            else
            {
                tokens = (int)myDB.SystemParameters.First().Silver;
                
            }
            order.TokensNumber = tokens;
            order.Price = order.TokensNumber * myDB.SystemParameters.First().TokensValue;

            order.IdUser = ((User)Session["User"]).IdUser;
            order.State = "SUBMITTED";
            order.OrderTime = DateTime.UtcNow;
            
            myDB.TokenOrders.Add(order);
            myDB.SaveChanges();



            return Redirect("http://stage.centili.com/payment/widget?apikey=f56c4e470aa1e62148c5c14ac2f45007&country=rs&reference=" + order.Id);
        }



      
        public ActionResult ProcessOrder(string clientid, string status)
        {

            logger.Info("GET/Accounts/ProcessOrder action has been called");
            TokenOrder order = myDB.TokenOrders.Find(int.Parse(clientid));
            if (order == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (status == "failed" || status == "canceled")
            {
                order.State = "CANCELED";
                TempData["OrderMessage"] = "Order failed!";
            }
            else
            {
                order.State = "COMPLETED";
                
                   
                User user = myDB.Users.Find(order.IdUser);
                if (user == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                user.TokensNumber += order.TokensNumber;
                myDB.Entry(user).State = EntityState.Modified;
                myDB.SaveChanges();
                myDB.Entry(user).State = EntityState.Detached;


                
                       
                SendEmail(user.Email);
                        
                       
                        
                    
                   
            }


            myDB.Entry(order).State = EntityState.Modified;

            myDB.SaveChanges();
            myDB.Entry(order).State = EntityState.Detached;


            return RedirectToAction("UserInformation");
        }
    

       

        public ActionResult EditParameters()
        {
            logger.Info("GET/Accounts/EditParameters action has been called");
            if (Session["Admin"]==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                return View();
            }
        }


        


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditParameters([Bind(Include = "Currency,Silver,Gold,Platnium,TokensValue,ItemsPerPage")] SystemParameter parameter)
        {
            logger.Info("POST/Accounts/EditParameters action has been called");
            if (Session["Admin"]==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var dbParam = myDB.SystemParameters.First();

            if (dbParam == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            if(parameter.Silver!=null)
                dbParam.Silver = parameter.Silver;

            if (parameter.Gold != null)
                dbParam.Gold = parameter.Gold;

            if (parameter.Platnium != null)
                dbParam.Platnium = parameter.Platnium;

            dbParam.ItemsPerPage = parameter.ItemsPerPage;

            if (parameter.Currency != null)
                dbParam.Currency = parameter.Currency;

            if(parameter.TokensValue!=null)
                dbParam.TokensValue = parameter.TokensValue;

            myDB.Entry(dbParam).State = EntityState.Modified;
            myDB.SaveChanges();
            myDB.Entry(dbParam).State = EntityState.Detached;


            return View("AdminPanel");
            
           

        }

        public ActionResult AdminPanel()
        {
            return View();
        }

        public ActionResult WonAuctions()
        {
            if (Session["User"] == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            User user = (User)Session["User"];
           

            var auctions = myDB.Auctions.OrderByDescending(x=>x.createdOn).ToList();

            

            var auction = auctions.Where(x=>x.LastbidderId.Equals(user.IdUser)).ToList();
           

            auction = auction.Where(x => x.State.Equals("COMPLETED")).ToList();


            return View(auction);
        }


        public ActionResult ShowTokenOrders()
        {

            if (Session["User"] == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            User user = (User)Session["User"];

            
            int pageSize = myDB.SystemParameters.First().ItemsPerPage;
            int pageNumber = 1;


            OrderViewModel order = new OrderViewModel();
            order.page = pageNumber;

            var orders = myDB.TokenOrders.OrderByDescending(x=>x.OrderTime).Where(x => x.IdUser.Equals(user.IdUser)).ToPagedList(pageNumber, pageSize);
            order.orders = orders;

            return View(order);
        }


        public ActionResult ShowOrders(OrderViewModel orders)
        {
            if (Session["User"] == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);


            User user = (User)Session["User"];

            int pageSize = myDB.SystemParameters.First().ItemsPerPage;
            int pageNumber = (orders.page??1);

            var order=myDB.TokenOrders.OrderByDescending(x => x.OrderTime).Where(x => x.IdUser.Equals(user.IdUser)).ToPagedList(pageNumber, pageSize);

            orders.orders = order;

            return View("ShowTokenOrders",orders);
            


        }


        private void SendEmail(string email)
        {
            MailMessage mm = new MailMessage("chevu14@gmail.com", email);
            {
                mm.Subject = "Payment Succeeded";
                string body = "Thank you for your purchase!";
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential("chevu14", "vukstefaniep");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }



    }
}
