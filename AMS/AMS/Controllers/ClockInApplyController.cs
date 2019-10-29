﻿using System;
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
    public class ClockInApplyController : Controller
    {
        private Entities db = new Entities();

        // GET: ClockInApply
        [HttpPost]
        public ActionResult Index()
        {
            return View();
        }

        // GET: ClockInApply/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClockInApply clockInApply = db.ClockInApply.Find(id);
            if (clockInApply == null)
            {
                return HttpNotFound();
            }
            return View(clockInApply);
        }

        // GET: ClockInApply/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClockInApply/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,OnDuty,OffDuty,ReviewStatusID,RequestDate,ReviewTime")] ClockInApply clockInApply)
        {
            if (ModelState.IsValid)
            {

                //var query = db.ClockInApply.Select(p => new CreateViewModel
                //{
                //   OnDuty= p.OnDuty ,
                //   OffDuty= p.OffDuty,
                //   RequestDate = p.RequestDate,
                //   EmployeeID= p.EmployeeID
                //});
               clockInApply.EmployeeID= "MSIT1230001";               

                db.ClockInApply.Add(clockInApply);
                try
                {
                    db.SaveChanges();
                    
                }
                catch {

                }

                
                return RedirectToAction("Index","Home");
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: ClockInApply/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClockInApply clockInApply = db.ClockInApply.Find(id);
            if (clockInApply == null)
            {
                return HttpNotFound();
            }
            return View(clockInApply);
        }

        // POST: ClockInApply/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,OnDuty,OffDuty,ReviewStatusID,RequestDate,ReviewTime")] ClockInApply clockInApply)
        {
            if (ModelState.IsValid)
            {
               
                db.Entry(clockInApply).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clockInApply);
        }

        // GET: ClockInApply/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClockInApply clockInApply = db.ClockInApply.Find(id);
            if (clockInApply == null)
            {
                return HttpNotFound();
            }
            return View(clockInApply);
        }

        // POST: ClockInApply/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ClockInApply clockInApply = db.ClockInApply.Find(id);
            db.ClockInApply.Remove(clockInApply);
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
