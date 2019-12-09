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
    public class AnnouuncementsController : Controller
    {
        private Entities db = new Entities();

        // GET: Annouuncements
        public ActionResult Index()
        {
            return PartialView("index",db.Annouuncements);
        }

        public ActionResult IndexTakeFive()
        {
            var ann = db.Annouuncements.OrderByDescending(n=>n.AnnouuncementID).Take(5);
            return PartialView("IndexTakeFive", ann);
        }

        // GET: Annouuncements/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Annouuncements annouuncements = db.Annouuncements.Find(id);
            if (annouuncements == null)
            {
                return HttpNotFound();
            }
            return View(annouuncements);
        }

        // GET: Annouuncements/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Annouuncements/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,AnnounceTime,Title,Detail,Importance,AnnouuncementID")] Annouuncements annouuncements)
        {
            if (ModelState.IsValid)
            {
                db.Annouuncements.Add(annouuncements);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(annouuncements);
        }

        // GET: Annouuncements/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Annouuncements annouuncements = db.Annouuncements.Find(id);
            if (annouuncements == null)
            {
                return HttpNotFound();
            }
            return View(annouuncements);
        }

        // POST: Annouuncements/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,AnnounceTime,Title,Detail,Importance,AnnouuncementID")] Annouuncements annouuncements)
        {
            if (ModelState.IsValid)
            {
                db.Entry(annouuncements).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(annouuncements);
        }

        // GET: Annouuncements/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Annouuncements annouuncements = db.Annouuncements.Find(id);
            if (annouuncements == null)
            {
                return HttpNotFound();
            }
            return View(annouuncements);
        }

        // POST: Annouuncements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Annouuncements annouuncements = db.Annouuncements.Find(id);
            db.Annouuncements.Remove(annouuncements);
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
