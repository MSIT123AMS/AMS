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
    public class ReviewController : Controller
    {
        private Entities db = new Entities();

        // GET: Review
        public ActionResult Index()
        {
            var q = db.LeaveRequests.AsEnumerable().Join(db.Employees, e => e.EmployeeID, d => d.EmployeeID, (e, d) => new ReviewViewModels
            {
                EmployeeID = d.EmployeeID,
                EmployeeName = d.EmployeeName,
                LeaveType = e.LeaveType,
                StartTime = e.StartTime,
                EndTime=e.EndTime,
                RequestTime = e.RequestTime,
                LeaveReason = e.LeaveReason,
                ReviewStatusID = e.ReviewStatusID

            });

            return View(db.LeaveRequests);
        }

        public ActionResult Index2()
        {
            var q1=from l in db.LeaveRequests
            join e in db.Employees on l.EmployeeID equals e.EmployeeID
            join r in db.ReviewStatus on l.ReviewStatusID equals r.ReviewStatusID
           
                   select new ReviewViewModels
            {
                EmployeeID = l.EmployeeID,
                EmployeeName = e.EmployeeName,
                LeaveType = l.LeaveType,
                StartTime = l.StartTime,
                EndTime = l.EndTime,
                RequestTime = l.RequestTime,
                LeaveReason = l.LeaveReason,
                ReviewStatus = r.ReviewStatus1,
                ReviewStatusID = l.ReviewStatusID,
                LeaveRequestID = l.LeaveRequestID

            };


            return View(q1);
        }


        public ActionResult Index3(string id)
        {
            LeaveRequests r = db.LeaveRequests.Find(id);
            return PartialView("_LeavePartial", r);
        }


        public ActionResult Ajax()
        {
            Entities db = new Entities();
            ViewBag.Customers = new SelectList(db.ReviewStatus, "ReviewStatusID", "ReviewStatus1");
            return View();

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


        public ActionResult Edit3(string[] Checkboxxx)
        {
            return View();
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
