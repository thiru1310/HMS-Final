using HSM_PROJECT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HSM_PROJECT.Controllers
{
    public class UserDetailController : Controller
    {
        // GET: UserDetail
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Register()
        {
            tblUserDetail user = new tblUserDetail();
            return View(user);
        }

        [HttpPost]
        public ActionResult Register(tblUserDetail user)
        {

            using (HospitalScheduleManagementEntities db = new HospitalScheduleManagementEntities())
            {
                if (db.tblUserDetails.Any(x => x.UserName == user.UserName))
                {
                    ViewBag.DuplicateMessage = "User Name Already Exists.";
                    return View("Register", user);
                }
                else
                {
                    db.tblUserDetails.Add(user);
                    db.SaveChanges();
                }
            }
            ModelState.Clear();
            ViewBag.SuccessMessage = "Registration Successfull.";
            return RedirectToAction("Login", "UserDetail");
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(tblUserDetail user)
        {
            using (HospitalScheduleManagementEntities db = new HospitalScheduleManagementEntities())
            {
                var result = db.tblUserDetails.Where(x => x.Email == user.Email && x.PassWord == user.PassWord);

                if (result.Count() != 0)
                {
                    Session["Email"] = user.Email;
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    ViewData["msg"] = "Incorrect Email/Password";
                }
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "UserDetail");
        }

    }
}