using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_Forum_cSharp.Models;
using System.Web.Security;

namespace Web_Forum_cSharp.Controllers
{
    public class AccountController : Controller
    {
        private ThreadEntities db = new ThreadEntities();

        // GET : Account/Login
        public ActionResult Login(String returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        // POST : Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include="UserName,Password")] User user, String returnUrl)
        {


            if (ModelState.IsValid)
            {
                //check if user pw matches
                
                User currentUser = (from u in db.Users
                                    where u.UserName.Equals(user.UserName) && u.Password.Equals(user.Password)
                                    select u).FirstOrDefault<User>();

                if (currentUser != null)
                    FormsAuthentication.RedirectFromLoginPage(user.UserName, false);
            }
            ModelState.AddModelError("", "Invalid information provided");
            return View();
        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut(); 
            return RedirectToAction("Index", "Home");
        }



        // GET: /Account/Create
        [ActionName("Register")]
        public ActionResult Register()
        {
            User user = new User();
            UserDetail detail = new UserDetail();
            detail.ImgPath = "";
            detail.Interest = "none";

            user.UserDetail = detail; 
            return View(user);
        }

        // POST: /Account/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "UserName,Password,UserDetail")] User user)
        {

            if (ModelState.IsValid)
            {
                if (db.Users.Find(user.UserName) != null)
                {
                    //fill the obj with the previous fields, except username and password

                    ModelState.AddModelError("", "Cannot make user with the given info");
                    return View(user);
                }

                db.Users.Add(user);
                db.UserDetails.Add(user.UserDetail);
                db.SaveChanges();
                return RedirectToAction("Login", "Account");
            }

            //ViewBag.UserName = new SelectList(db.UserDetails, "UserName", "Firstname", user.UserName);
            return View(user);
        }
        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
