using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HSM_PROJECT.Models;

namespace HSM_PROJECT.Controllers
{
    public class DoctorController : Controller
    {
        HospitalScheduleManagementEntities db = new HospitalScheduleManagementEntities();
        // GET: DoctorLogin
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Autherize(HSM_PROJECT.Models.tblDoctor doctorModel)
        {
            using (HospitalScheduleManagementEntities db = new HospitalScheduleManagementEntities())
            {
                var DoctorDetails = db.tblDoctors.Where(x => x.EmailId == doctorModel.EmailId && x.Password == doctorModel.Password).FirstOrDefault();
                if (DoctorDetails == null)
                {
                    ModelState.AddModelError("", "Invalid Email Or Password");
                    return View("Index", doctorModel);
                }
                else
                {
                    Session["DoctorId"] = DoctorDetails.DoctorId;
                    Session["DoctorName"] = DoctorDetails.DoctorName;
                    int doctorid = (int)Session["DoctorId"];
                    return RedirectToAction("DoctorAppointment", "Doctor", new { id = doctorid });

                }
            }
        }
        public ActionResult LogOut()
        {
            int DoctorId = (int)Session["DoctorId"];
            Session.Abandon();
            return RedirectToAction("Index", "Doctor");
        }
        [HttpGet]
        public ActionResult DoctorAppointment(int id)
        {
                var Appointment = db.tblAppointments.SqlQuery("sp_DoctorsAppointment @DctorId", new SqlParameter("@DctorId", id)).ToList();
                return View(Appointment);
        }    
    }
}