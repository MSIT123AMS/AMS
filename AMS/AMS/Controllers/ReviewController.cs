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
    public class ReviewController : Controller
    {
        private Entities db = new Entities();

        // GET: Review
        public ActionResult Index()
        {
            return View(db.LeaveRequests);
        }

        // GET: Review/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LeaveRequests leaveRequests = db.LeaveRequests.Find(id);
            if (leaveRequests == null)
            {
                return HttpNotFound();
            }
            return View(leaveRequests);
        }

        // GET: Review/Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Edit2(string[] Checkboxxx)
        {

            foreach (var item in Checkboxxx)
            {
                if (item != "false")
                {
                    LeaveRequests leaveRequests = db.LeaveRequests.Find(item);
                    leaveRequests.ReviewStatusID = 2;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }
        // POST: Review/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LeaveRequestID,EmployeeID,RequestTime,StartTime,EndTime,LeaveType,LeaveReason,ReviewStatusID,ReviewTime,Attachment")] LeaveRequests leaveRequests)
        {
            if (ModelState.IsValid)
            {
                db.LeaveRequests.Add(leaveRequests);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(leaveRequests);
        }

        // GET: Review/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LeaveRequests leaveRequests = db.LeaveRequests.Find(id);
            if (leaveRequests == null)
            {
                return HttpNotFound();
            }
            return View(leaveRequests);
        }

        // POST: Review/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LeaveRequestID,EmployeeID,RequestTime,StartTime,EndTime,LeaveType,LeaveReason,ReviewStatusID,ReviewTime,Attachment")] LeaveRequests leaveRequests)
        {
            if (ModelState.IsValid)
            {
                db.Entry(leaveRequests).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(leaveRequests);
        }

        // GET: Review/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LeaveRequests leaveRequests = db.LeaveRequests.Find(id);
            if (leaveRequests == null)
            {
                return HttpNotFound();
            }
            return View(leaveRequests);
        }

        // POST: Review/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            LeaveRequests leaveRequests = db.LeaveRequests.Find(id);
            db.LeaveRequests.Remove(leaveRequests);
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
