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
        //public ActionResult Index5()
        //{
        //    var q = db.LeaveRequests.AsEnumerable().Join(db.Employees, e => e.EmployeeID, d => d.EmployeeID, (e, d) => new LeaveReviewViewModels
        //    {
        //        EmployeeID = d.EmployeeID,
        //        EmployeeName = d.EmployeeName,
        //        LeaveType = e.LeaveType,
        //        StartTime = e.StartTime,
        //        EndTime=e.EndTime,
        //        RequestTime = e.RequestTime,
        //        LeaveReason = e.LeaveReason,
        //        ReviewStatusID = e.ReviewStatusID

        //    });

        //    return View(db.LeaveRequests);
        //}

        //public ActionResult Index22()
        //{
        //    var q1=from l in db.LeaveRequests
        //    join e in db.Employees on l.EmployeeID equals e.EmployeeID
        //    join r in db.ReviewStatus on l.ReviewStatusID equals r.ReviewStatusID

           
        //           select new LeaveReviewViewModels
        //    {
        //        EmployeeID = l.EmployeeID,
        //        EmployeeName = e.EmployeeName,
        //        LeaveType = l.LeaveType,
        //        StartTime = l.StartTime,
        //        EndTime = l.EndTime,
        //        RequestTime = l.RequestTime,
        //        LeaveReason = l.LeaveReason,
        //        ReviewStatus = r.ReviewStatus1,
        //        ReviewStatusID = l.ReviewStatusID,
        //        LeaveRequestID = l.LeaveRequestID

        //    };
        //    return View(q1);
        //}


        //public ActionResult Index3(string id)
        //{
        //    LeaveRequests r = db.LeaveRequests.Find(id);
        //    return PartialView("_LeavePartial", r);
        //}
        //public ActionResult Index22(string id="2")
        //{
        //    int i = int.Parse(id);
        //    var q1 = from l in db.LeaveRequests
        //             join e in db.Employees on l.EmployeeID equals e.EmployeeID
        //             join r in db.ReviewStatus on l.ReviewStatusID equals r.ReviewStatusID
        //             where l.ReviewStatusID == i
        //             select new LeaveReviewViewModels
        //             {
        //                 EmployeeID = l.EmployeeID,
        //                 EmployeeName = e.EmployeeName,
        //                 LeaveType = l.LeaveType,
        //                 StartTime = l.StartTime,
        //                 EndTime = l.EndTime,
        //                 RequestTime = l.RequestTime,
        //                 LeaveReason = l.LeaveReason,
        //                 ReviewStatus = r.ReviewStatus1,
        //                 ReviewStatusID = l.ReviewStatusID,
        //                 LeaveRequestID = l.LeaveRequestID

        //             };
        //    return View(q1);
        //}

        public ActionResult Index(string id = "1")
        {
            string User = Convert.ToString(Session["UserName"]);
            if (User != "")
            {
                db.Employees.Where(e => e.EmployeeName == User);
                var departID = from e in db.Employees
                        join d in db.Departments on e.DepartmentID equals d.DepartmentID
                        where e.EmployeeID == User
                        select d.DepartmentID;
            }
            int i = int.Parse(id);
            ViewBag.Customers = new SelectList(db.ReviewStatus, "ReviewStatusID", "ReviewStatus1");
            var q1 = from l in db.LeaveRequests
                     join e in db.Employees on l.EmployeeID equals e.EmployeeID
                     join r in db.ReviewStatus on l.ReviewStatusID equals r.ReviewStatusID
                     where l.ReviewStatusID == i
                     select new LeaveReviewViewModels
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
            //return PartialView("_LeavePartial", q1);
        }




        public ActionResult Ajax(string id = "1")
        {
            int i = int.Parse(id);
            //Entities db = new Entities();
            ViewBag.Customers = new SelectList(db.ReviewStatus, "ReviewStatusID", "ReviewStatus1");
            var q1 = from l in db.LeaveRequests
                     join e in db.Employees on l.EmployeeID equals e.EmployeeID
                     join r in db.ReviewStatus on l.ReviewStatusID equals r.ReviewStatusID
                     where l.ReviewStatusID == i
                     select new LeaveReviewViewModels
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
    
            return PartialView("_LeavePartial", q1);

        }


        public ActionResult AjaxLeave(string id = "1")
        {
            int i = int.Parse(id);
            //Entities db = new Entities();
            ViewBag.Customers = new SelectList(db.ReviewStatus, "ReviewStatusID", "ReviewStatus1");
            var q1 = from l in db.LeaveRequests
                     join e in db.Employees on l.EmployeeID equals e.EmployeeID
                     join r in db.ReviewStatus on l.ReviewStatusID equals r.ReviewStatusID
                     where l.ReviewStatusID == i
                     select new LeaveReviewViewModels
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

            return PartialView("_LeavePartial", q1);

        }

        //public ActionResult AjaxClockInApply(string id = "1")
        //{//todo
            //int i = int.Parse(id);
            ////Entities db = new Entities();
            //ViewBag.Customers = new SelectList(db.ReviewStatus, "ReviewStatusID", "ReviewStatus1");
            //var q1 = from c in db.ClockInApply
            //         join e in db.Employees on c.EmployeeID equals e.EmployeeID
            //         join r in db.ReviewStatus on c.ReviewStatusID equals r.ReviewStatusID
            //         where c.ReviewStatusID == i
            //         select new LeaveReviewViewModels
            //         {
            //             EmployeeID = c.EmployeeID,
            //             EmployeeName = e.EmployeeName,
            //             StartTime = c.OnDuty,
            //             EndTime = c.OffDuty,
            //             RequestTime = c.RequestDate,
            //             ReviewStatus = r.ReviewStatus1,
            //             ReviewStatusID = int.Parse(c.ReviewStatusID),
            //             LeaveRequestID = c.LeaveRequestID

            //         };

            //return PartialView("_LeavePartial", q1);

        //}


        public ActionResult AjaxOvertime(string id = "1")
        {
            int i = int.Parse(id);
            //Entities db = new Entities();
            ViewBag.Customers = new SelectList(db.ReviewStatus, "ReviewStatusID", "ReviewStatus1");
            var q1 = from l in db.OverTimeRequest
                     join e in db.Employees on l.EmployeeID equals e.EmployeeID
                     join r in db.ReviewStatus on l.ReviewStatusID equals r.ReviewStatusID
                     where l.ReviewStatusID == i
                     select new OverTimeReviewViewModels
                     {
                         EmployeeID = l.EmployeeID,
                         EmployeeName = e.EmployeeName,
                         OverTimePay = l.OverTimePay,
                         StartTime = l.StartTime,
                         EndTime = l.EndTime,
                         RequestTime = l.RequestTime,
                         OverTimeReason = l.OverTimeReason,
                         ReviewStatus = r.ReviewStatus1,
                         ReviewStatusID = l.ReviewStatusID,
                         OverTimeRequestID = l.OverTimeRequestID

                     };

            return PartialView("_OverTimePartial", q1);

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
                    leaveRequests.ReviewTime = DateTime.Now;
                    db.SaveChanges();
                }
            }
            TempData["Status"] ="UpdateSuccess";
            return RedirectToAction("Index");
        }

        public ActionResult Edit3(string[] Checkboxxx)
        {

            foreach (var item in Checkboxxx)
            {
                if (item != "false")
                {
                    OverTimeRequest overTimeRequests = db.OverTimeRequest.Find(item);
                    overTimeRequests.ReviewStatusID = 2;
                    overTimeRequests.ReviewTime = DateTime.Now;
                    db.SaveChanges();
                }
            }
            ViewBag.Status = "updatesuccess";
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
