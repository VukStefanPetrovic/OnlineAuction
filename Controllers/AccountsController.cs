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

namespace OnlineAuctions.Controllers
{

    public class AccountsController : Controller
    {
        private Model myDB = new Model();


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
            return View();
        }


        public ActionResult UserPanel()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "Firstname, Lastname, Email, Password, TokensNumber, Role, IdUser")] User user)
        {
            user.Password = CreateMD5(user.Password);
            var found = myDB.Users.Any(x => x.Email == user.Email && x.Password == user.Password);

            if (found)
            {
              
                var ourUser = myDB.Users.First(x=> x.Email==user.Email && x.Password==user.Password);
                Session["User"] = ourUser;
                return View("UserPanel");
            }
            ViewBag.Message = "User with this Email and Password doesn't exists!";
            return View("Login");

        }


        public ActionResult Register()
        {
            if (Session["User"] != null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "Firstname, Lastname, Email, Password, TokensNumber, Role, IdUser")] User user)
        {
            if (ModelState.IsValid)
            {
                var found = myDB.Users.Any(x => x.Email == user.Email);
                if (!found)
                {
                    user.TokensNumber = 0;
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
            Session.Clear();
            return RedirectToAction("Index", "Auctions");
        }


        public ActionResult UserInformation()
        {
            if (Session["User"] == null || Session["Admin"] != null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int idUser = ((User)Session["User"]).IdUser;

            User user = myDB.Users.Find(idUser);
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(user);
        }

        public ActionResult EditProfile(int? id)
        {
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
            if (Session["User"] == null || Session["Admin"] != null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!myDB.Users.Any(x => x.Email == user.Email && x.IdUser != user.IdUser))
            {
                myDB.Entry(user).State = EntityState.Modified;
                bool saveFailed;
                do
                {
                    saveFailed = false;
                    try { myDB.SaveChanges(); }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        saveFailed = true;
                        // Update original values from the database 
                        var entry = ex.Entries.Single();
                        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                    }
                } while (saveFailed);


                Session["User"] = user;
                return RedirectToAction("UserInformation");
            }
            ViewBag.Message = "User with this mail exists!";
            return View(user);

        }




    }
}
