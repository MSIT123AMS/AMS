using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AMS.Models;
using static System.Net.Mime.MediaTypeNames;

namespace AMS.Controllers
{
    public class OverTimeRequestsController : Controller
    {
        private Entities db = new Entities();
        private OverTimeClassLibrary.OverTime OvertimeObj = new OverTimeClassLibrary.OverTime();

        // GET: OverTimeRequests
        public ActionResult Index()
        {
            return View(db.OverTimeRequest.ToList());
        }

        // GET: OverTimeRequests/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            var overTimeRequest = (from ot in db.OverTimeRequest.AsEnumerable()
                                   join emp in db.Employees.AsEnumerable() on ot.EmployeeID equals emp.EmployeeID
                                   join rev in db.ReviewStatus.AsEnumerable() on ot.ReviewStatusID equals rev.ReviewStatusID
                                   join date in db.WorkingDaySchedule.AsEnumerable() on ot.StartTime.Date equals date.Date
                                   where ot.OverTimeRequestID == id
                                   select new OverTimeViewModel
                                   {
                                       RequestID = ot.OverTimeRequestID,
                                       EmployeeName = emp.EmployeeName,
                                       RequestTime = ot.RequestTime,
                                       StartTime = ot.StartTime,
                                       EndTime = ot.EndTime,
                                       PayorOFF = OvertimeObj.PayorOff(ot.OverTimePay),
                                       OTDateType = date.WorkingDay,
                                       SummaryTime = OvertimeObj.Summary(ot.StartTime, ot.EndTime, date.WorkingDay, ot.OverTimePay),
                                       Reason = ot.OverTimeReason,
                                       Review = rev.ReviewStatus1,
                                       ReviewTime = ot.ReviewTime
                                   }).First();

            if (overTimeRequest == null)
            {
                return HttpNotFound();
            }

            return View(overTimeRequest);
        }
            public class PayOff
        {
            public String text { get; set; }
            public bool value { get; set; }
        }
        // GET: OverTimeRequests/Create
        public ActionResult Create()
        {


            var dropdownlist = new List<PayOff>
            {
                new PayOff{ text="加班費",value=true},
                new PayOff{ text="補休",value=false}

            };
            ViewBag.list = new SelectList(dropdownlist, "value", "text");
            return View();
        }

        // POST: OverTimeRequests/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StartTime,EndTime,OverTimePay,OverTimeReason")] OverTimeRequest overTimeRequest)
        {
            if (ModelState.IsValid)
            {
                overTimeRequest.OverTimeRequestID = db.OverTimeRequest.Count().ToString();
                overTimeRequest.EmployeeID = "MSIT0001";
                overTimeRequest.RequestTime = DateTime.Now;
                overTimeRequest.ReviewStatusID = 1;
                
                db.OverTimeRequest.Add(overTimeRequest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(overTimeRequest);
        }

        // GET: OverTimeRequests/Edit/5
        //public ActionResult Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    OverTimeRequest overTimeRequest = db.OverTimeRequest.Find(id);
        //    if (overTimeRequest == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(overTimeRequest);
        //}

        // POST: OverTimeRequests/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "OverTimeRequestID,EmployeeID,RequestTime,StartTime,EndTime,OverTimePay,OverTimeReason,ReviewStatusID,ReviewTime")] OverTimeRequest overTimeRequest)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(overTimeRequest).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(overTimeRequest);
        //}

        //// GET: OverTimeRequests/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    OverTimeRequest overTimeRequest = db.OverTimeRequest.Find(id);
        //    if (overTimeRequest == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(overTimeRequest);
        //}

        // POST: OverTimeRequests/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    OverTimeRequest overTimeRequest = db.OverTimeRequest.Find(id);
        //    db.OverTimeRequest.Remove(overTimeRequest);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
