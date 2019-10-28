using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using AMS.Models;

namespace AMS.Controllers
{
    public class LeaveRequestsController : Controller
    {
        private Entities db = new Entities();

        // GET: LeaveRequests
        public ActionResult Index()
        {
            return View(db.LeaveRequests);
        }

        // GET: LeaveRequests/Details/5

        public ActionResult LeaveIndexView()
        {
            var LeaveTimeRequest = (from lt in db.LeaveRequests.AsEnumerable()
                                    join emp in db.Employees.AsEnumerable() on lt.EmployeeID equals emp.EmployeeID
                                    join rev in db.ReviewStatus.AsEnumerable() on lt.ReviewStatusID equals rev.ReviewStatusID
                                    select new LeaveIndexViewModel
                                    {
                                        LeaveRequestID = lt.LeaveRequestID,
                                        EmployeeName = emp.EmployeeName,
                                        LeaveType = lt.LeaveType,
                                        RequestTime = lt.RequestTime,
                                        StartTime = lt.StartTime,
                                        EndTime = lt.EndTime,
                                        LeaveReason = lt.LeaveReason,
                                        Review = rev.ReviewStatus1,
                                        ReviewTime = lt.ReviewTime,
                                        Attachment = lt.Attachment
                                    });
    
                return View(LeaveTimeRequest);

        }
        public ActionResult Leavetable(string id, string id2)
        {
            DateTime startime = Convert.ToDateTime(id.Substring(0, 4) + "/" + id.Substring(4, 2) + "/" + id.Substring(6, 2) + " 00:00:00");
            DateTime endtime = Convert.ToDateTime(id2.Substring(0, 4) + "/" + id2.Substring(4, 2) + "/" + id2.Substring(6, 2) + " 23:59:59");

            var LeaveTimeRequest = (from lt in db.LeaveRequests.AsEnumerable()
                                    join emp in db.Employees.AsEnumerable() on lt.EmployeeID equals emp.EmployeeID
                                    join rev in db.ReviewStatus.AsEnumerable() on lt.ReviewStatusID equals rev.ReviewStatusID
                                    where lt.StartTime >= startime && lt.EndTime <= endtime
                                    select new LeaveIndexViewModel
                                    {
                                        LeaveRequestID = lt.LeaveRequestID,
                                        EmployeeName = emp.EmployeeName,
                                        LeaveType = lt.LeaveType,
                                        RequestTime = lt.RequestTime,
                                        StartTime = lt.StartTime,
                                        EndTime = lt.EndTime,
                                        LeaveReason = lt.LeaveReason,
                                        Review = rev.ReviewStatus1,
                                        ReviewTime = lt.ReviewTime,
                                        Attachment = lt.Attachment
                                    });
            if (LeaveTimeRequest != null)
            {
                return PartialView("_LeaveIndexPartialView", LeaveTimeRequest);

            }
            else
            {
                return HttpNotFound();
            }
        }
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

        // GET: LeaveRequests/Create

        public class LeaveCombobox
        {
            public string comtext { get; set; }
            public string text { get; set; }
        }
        public ActionResult Create()
        {
            ViewBag.LeaveType = new SelectList(new List<LeaveCombobox> {
                new LeaveCombobox {comtext="事假",text="事假" },
                new LeaveCombobox {comtext="病假",text="病假"},
                new LeaveCombobox {comtext="公假",text="公假"},
                new LeaveCombobox {comtext="喪假",text="喪假"},
                new LeaveCombobox {comtext="產假",text="產假"},
                new LeaveCombobox {comtext="陪產假",text="陪產假"},
                new LeaveCombobox {comtext="生理假",text="生理假"},
                new LeaveCombobox {comtext="補休假",text="補休假"},
                new LeaveCombobox {comtext="家庭照顧假",text="家庭照顧假"},
            }, "text", "comtext");

            return View();
        }

        // POST: LeaveRequests/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LeaveRequestID,EmployeeID,RequestTime,StartTime,EndTime,LeaveType,LeaveReason,ReviewStatusID,ReviewTime,Attachment")] LeaveRequests leaveRequests)
        {
            if (ModelState.IsValid)
            {
                //新增先給這幾項直(登入還沒做)
                leaveRequests.LeaveRequestID = db.LeaveRequests.Count().ToString();
                leaveRequests.EmployeeID = "MSIT1230001";
                leaveRequests.RequestTime = DateTime.Now;
                leaveRequests.ReviewStatusID = 1;
                if (Request.Files["LeaveFile"].ContentLength != 0)
                {
                    byte[] data = null;
                    using (BinaryReader br = new BinaryReader(
                        Request.Files["LeaveFile"].InputStream))
                    {
                        data = br.ReadBytes(Request.Files["LeaveFile"].ContentLength);
                    }
                    leaveRequests.Attachment = data;
                }

                db.LeaveRequests.Add(leaveRequests);
                db.SaveChanges();
                return RedirectToAction("LeaveIndexView");
            }

            return View(leaveRequests);
        }

        // GET: LeaveRequests/Edit/5
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

        // POST: LeaveRequests/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LeaveRequestID,EmployeeID,RequestTime,StartTime,EndTime,LeaveType,LeaveReason,ReviewStatusID,ReviewTime,Attachment")] LeaveRequests leaveRequests)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files["LeaveFile"].ContentLength != 0)
                {
                    byte[] data = null;
                    using (BinaryReader br = new BinaryReader(
                        Request.Files["LeaveFile"].InputStream))
                    {
                        data = br.ReadBytes(Request.Files["LeaveFile"].ContentLength);
                    }
                    leaveRequests.Attachment = data;
                }
                else
                {
                    LeaveRequests L = db.LeaveRequests.Find(leaveRequests.LeaveRequestID);
                    L.EmployeeID = leaveRequests.EmployeeID;
                    L.RequestTime = leaveRequests.RequestTime;
                    L.StartTime = leaveRequests.StartTime;
                    L.EndTime = leaveRequests.StartTime;
                    L.LeaveType = leaveRequests.LeaveType;
                    L.LeaveReason = leaveRequests.LeaveReason;
                    L.LeaveRequestID = leaveRequests.LeaveRequestID;
                    L.RequestTime = leaveRequests.RequestTime;
                    leaveRequests = L;
                }


                db.Entry(leaveRequests).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(leaveRequests);
        }

        // GET: LeaveRequests/Delete/5
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

        // POST: LeaveRequests/Delete/5
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
        public FileResult ShowPhoto(string id)
        {
            byte[] content = db.LeaveRequests.Find(id).Attachment;
            return File(content, "image/jpeg");
        }


    }
}
