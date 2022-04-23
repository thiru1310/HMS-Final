using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HSM_PROJECT.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                if (Session["Email"]==null)
                {
                    try
                    {
                           // this is thiruppathy comment
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    
                    return RedirectToAction("Login", "UserDetail");

                }
                int j = 0;
                int i = 10 / j;

            }
            catch (DivideByZeroException e)

            {

                return RedirectToAction("Login", "UserDetail");
            }
            catch(Exception e)
            {

            }
            return View();

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}