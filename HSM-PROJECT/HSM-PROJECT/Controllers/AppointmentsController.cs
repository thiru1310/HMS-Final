using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HSM_PROJECT.Models;

namespace HSM_PROJECT.Controllers
{
    public class AppointmentsController : Controller
    {
        private HospitalScheduleManagementEntities db = new HospitalScheduleManagementEntities();

        // GET: Appointments
        public ActionResult Index()
        {
            var tblAppointments = db.tblAppointments.Include(t => t.tblDiseaseCategory).Include(t => t.tblDoctor).Include(t => t.tblPatient);
            return View(tblAppointments.ToList());
        }

        // GET: Appointments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAppointment tblAppointment = db.tblAppointments.Find(id);
            if (tblAppointment == null)
            {
                return HttpNotFound();
            }
            return View(tblAppointment);
        }

        // GET: Appointments/Create
        public ActionResult Create()
        {
            ViewBag.DiseaseCategory = new SelectList(db.tblDiseaseCategories, "CategoryId", "CategoryName");
            ViewBag.DoctorId = new SelectList(db.tblDoctors, "DoctorId", "DoctorName");
            ViewBag.PatientId = new SelectList(db.tblPatients, "PatientId", "FirstName");
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AppointmentId,Symptoms,PatientId,DoctorId,DiseaseCategory,StartTime,EndTime")] tblAppointment tblAppointment)
        {
            if (ModelState.IsValid)
            {
                db.tblAppointments.Add(tblAppointment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DiseaseCategory = new SelectList(db.tblDiseaseCategories, "CategoryId", "CategoryName", tblAppointment.DiseaseCategory);
            ViewBag.DoctorId = new SelectList(db.tblDoctors, "DoctorId", "DoctorName", tblAppointment.DoctorId);
            ViewBag.PatientId = new SelectList(db.tblPatients, "PatientId", "FirstName", tblAppointment.PatientId);
            return View(tblAppointment);
        }

        // GET: Appointments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAppointment tblAppointment = db.tblAppointments.Find(id);
            if (tblAppointment == null)
            {
                return HttpNotFound();
            }
            ViewBag.DiseaseCategory = new SelectList(db.tblDiseaseCategories, "CategoryId", "CategoryName", tblAppointment.DiseaseCategory);
            ViewBag.DoctorId = new SelectList(db.tblDoctors, "DoctorId", "DoctorName", tblAppointment.DoctorId);
            ViewBag.PatientId = new SelectList(db.tblPatients, "PatientId", "FirstName", tblAppointment.PatientId);
            return View(tblAppointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AppointmentId,Symptoms,PatientId,DoctorId,DiseaseCategory,StartTime,EndTime")] tblAppointment tblAppointment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblAppointment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DiseaseCategory = new SelectList(db.tblDiseaseCategories, "CategoryId", "CategoryName", tblAppointment.DiseaseCategory);
            ViewBag.DoctorId = new SelectList(db.tblDoctors, "DoctorId", "DoctorName", tblAppointment.DoctorId);
            ViewBag.PatientId = new SelectList(db.tblPatients, "PatientId", "FirstName", tblAppointment.PatientId);
            return View(tblAppointment);
        }

        // GET: Appointments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAppointment tblAppointment = db.tblAppointments.Find(id);
            if (tblAppointment == null)
            {
                return HttpNotFound();
            }
            return View(tblAppointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblAppointment tblAppointment = db.tblAppointments.Find(id);
            db.tblAppointments.Remove(tblAppointment);
            db.SaveChanges();
            return RedirectToAction("Index");
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
