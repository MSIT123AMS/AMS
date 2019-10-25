using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AMS.Models;

namespace AMS.Controllers
{
    public class AttendancesController : Controller
    {
        private Entities db = new Entities();

        // GET: Attendances
        public ActionResult Index()
        {
            return View(db.Attendances.ToList());
        }

        // GET: Attendances/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attendances attendances = db.Attendances.Find(id);
            if (attendances == null)
            {
                return HttpNotFound();
            }
            return View(attendances);
        }

        // GET: Attendances/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Attendances/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LineID,Date,Onduty,Offduty,Station,Localtion")] Attendances attendances)
        {
            if (ModelState.IsValid)
            {
                db.Attendances.Add(attendances);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(attendances);
        }

        // GET: Attendances/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attendances attendances = db.Attendances.Find(id);
            if (attendances == null)
            {
                return HttpNotFound();
            }
            return View(attendances);
        }

        // POST: Attendances/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LineID,Date,Onduty,Offduty,Station,Localtion")] Attendances attendances)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attendances).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(attendances);
        }

        // GET: Attendances/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attendances attendances = db.Attendances.Find(id);
            if (attendances == null)
            {
                return HttpNotFound();
            }
            return View(attendances);
        }

        // POST: Attendances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Attendances attendances = db.Attendances.Find(id);
            db.Attendances.Remove(attendances);
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
