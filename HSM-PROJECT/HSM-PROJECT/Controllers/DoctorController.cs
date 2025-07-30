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

        /*creating a branch
          in our project        */
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

        [HttpPost]
        public JsonResult SaveNotes(int appointmentId, string notes, string action)
        {
            try
            {
                var appointment = db.tblAppointments.Find(appointmentId);
                if (appointment == null)
                {
                    return Json(new { success = false, message = "Appointment not found." });
                }

                // Validate that notes are provided
                if (string.IsNullOrWhiteSpace(notes))
                {
                    return Json(new { success = false, message = "Notes are mandatory for this expertise." });
                }

                // Update appointment with notes
                appointment.Notes = notes;
                
                // Set action based on button clicked
                if (action == "confirm")
                {
                    appointment.Action = 1; // Confirmed
                }
                else if (action == "cancel")
                {
                    appointment.Action = 2; // Canceled
                }

                db.SaveChanges();

                return Json(new { success = true, message = "Notes saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error saving notes: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ValidateAllNotes(int doctorId)
        {
            try
            {
                // Get all pending appointments for this doctor
                var pendingAppointments = db.tblAppointments
                    .Where(a => a.DoctorId == doctorId && a.Action == 0)
                    .ToList();

                var appointmentsWithoutNotes = pendingAppointments
                    .Where(a => string.IsNullOrWhiteSpace(a.Notes))
                    .Count();

                return Json(new { 
                    success = true, 
                    canExit = appointmentsWithoutNotes == 0,
                    missingNotesCount = appointmentsWithoutNotes 
                });
            }
            catch (Exception ex)
            {
                                 return Json(new { success = false, message = ex.Message });
             }
        }

        [HttpGet]
        public ActionResult NotesConfiguration()
        {
            var doctors = db.tblDoctors.ToList();
            return View(doctors);
        }

        [HttpPost]
        public JsonResult SaveNotesConfiguration(string configuration)
        {
            try
            {
                // In a real application, you would save this to a database table
                // For now, we'll just return success since the frontend handles localStorage
                
                // You could create a NotesConfiguration table and save it there
                // Example:
                // var config = new NotesConfiguration
                // {
                //     DoctorId = (int)Session["DoctorId"],
                //     Configuration = configuration,
                //     UpdatedDate = DateTime.Now
                // };
                // db.NotesConfigurations.Add(config);
                // db.SaveChanges();
                
                return Json(new { success = true, message = "Configuration saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error saving configuration: " + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult GetNotesConfiguration()
        {
            try
            {
                // In a real application, you would load this from the database
                // For now, return a default configuration
                var defaultConfig = new
                {
                    globalRequired = true,
                    minLength = 10,
                    expertises = new Dictionary<string, object>()
                };

                return Json(defaultConfig, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }    
    }
}