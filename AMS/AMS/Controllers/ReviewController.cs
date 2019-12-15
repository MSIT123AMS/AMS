using System;
using System.Collections;
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
        [AllowAnonymous]
        public ActionResult SubmitViewMobile()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult LeaveViewMobile()
        {
            return View(ReadLeaveRequests(12,1));
        }
        [AllowAnonymous]
        public ActionResult OverTimeViewMobile()
        {
            return View(ReadOverTimeRequests(12, 1));
        }

        public ActionResult FindEmployeeByDepartment(int departmentID=1)
        {
            var result = db.Employees.Where(e => e.DepartmentID == departmentID).Select(e=>e.EmployeeName);
            //var k= result.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public int calLeaveTime(DateTime end, DateTime start)
        {
            var spread = (end - start);
            if (end.Date == start.Date)
            {
               return(spread.Hours - 1);
            }
            else
            {
                Random r = new Random();
                return (spread.Days * 8) -r.Next(8);
            }
        }

        public ActionResult ReadLeaveRequestsForChart(int id)
        {//id=month
           
            var result = from l in db.LeaveRequests.AsEnumerable()
                         join e in db.Employees.AsEnumerable() on l.EmployeeID equals e.EmployeeID
                         join r in db.ReviewStatus.AsEnumerable() on l.ReviewStatusID equals r.ReviewStatusID
                         where l.ReviewStatusID == 2 && l.StartTime.Month == id
                         select new
                         {
                             EmployeeName = e.EmployeeName,
                             LeaveType = l.LeaveType,
                             Spread =calLeaveTime(l.EndTime, l.StartTime),
                         };
            //var y = result.ToList();
            var group = from r in result
                        group r by new { r.EmployeeName,r.LeaveType } into g
                        select new
                        {
                            g.Key,
                            sum = g.Sum(k => k.Spread)
                        };

            //var h = group.ToList();
            return Json(group, JsonRequestBehavior.AllowGet);

        }
        public ActionResult OverTimeRequestForChart(int id)
        {
            var overTimeRequest = (from ot in db.OverTimeRequest.AsEnumerable()
                                   join emp in db.Employees.AsEnumerable() on ot.EmployeeID equals emp.EmployeeID
                                   join rev in db.ReviewStatus.AsEnumerable() on ot.ReviewStatusID equals rev.ReviewStatusID
                                   join date in db.WorkingDaySchedule.AsEnumerable() on ot.StartTime.Date equals date.Date
                                   where ot.ReviewStatusID == 2 && ot.StartTime.Month==id
                                   select new
                                   {
                                       EmployeeName = emp.EmployeeName,
                                       SummaryTime = double.Parse(OvertimeObj.Summary(ot.StartTime, ot.EndTime, date.WorkingDay, ot.OverTimePay)),
                                   });

            var b = from o in overTimeRequest
                    group o by o.EmployeeName into g
                    select new
                    {
                        g.Key,
                        cou = g.Sum(k => k.SummaryTime)
                    };

            return Json(b,JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Index(int type ,int status,int month)
        {
            if (status==1)
            {
                month = -1;
            }

            TempData["RequestType"] = type;
            TempData["ReviewStatus"] = status;
            TempData["Month"] = month;
            
            switch (type)
            {
                case 3://加班
                    return PartialView("_OverTimePartial", ReadOverTimeRequests(month, status));

                case 1://請假
                    return PartialView("_LeavePartial", ReadLeaveRequests(month, status));

                default:
                    return null;
            }
        }
        public IEnumerable<AMS.Models.OverTimeReviewViewModels> ReadOverTimeRequests(int month, int reviewStatus)
        {
            if (month==-1)
            {
                return from l in db.OverTimeRequest
                       join e in db.Employees on l.EmployeeID equals e.EmployeeID
                       join r in db.ReviewStatus on l.ReviewStatusID equals r.ReviewStatusID
                       where l.ReviewStatusID == reviewStatus
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
            }
            else
            {
            return  from l in db.OverTimeRequest
                         join e in db.Employees on l.EmployeeID equals e.EmployeeID
                         join r in db.ReviewStatus on l.ReviewStatusID equals r.ReviewStatusID
                         where l.ReviewStatusID == reviewStatus&&l.StartTime.Month==month
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


            }

            
        }

        [AllowAnonymous]
        public IEnumerable<AMS.Models.LeaveReviewViewModels> ReadLeaveRequests(int month,int reviewStatus)
        {
            if (month == -1)
            {
                var result = from l in db.LeaveRequests
                             join e in db.Employees on l.EmployeeID equals e.EmployeeID
                             join r in db.ReviewStatus on l.ReviewStatusID equals r.ReviewStatusID
                             where l.ReviewStatusID == reviewStatus 
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
                return result;

            }
            else
            {
                var result = from l in db.LeaveRequests
                             join e in db.Employees on l.EmployeeID equals e.EmployeeID
                             join r in db.ReviewStatus on l.ReviewStatusID equals r.ReviewStatusID
                             where l.ReviewStatusID == reviewStatus && l.StartTime.Month == month
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
                return result;

            }

        }



        private OverTimeClassLibrary.OverTime OvertimeObj = new OverTimeClassLibrary.OverTime();
        public ActionResult OverTimeDetails(string id)
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

            return PartialView("_ReviewOverTimeDetails", overTimeRequest);
        }

        // GET: Review/Details/5
        public ActionResult Details(string id)
        {
            LeaveRequests leaveRequests = db.LeaveRequests.Find(id);
            if (leaveRequests == null)
            {
                return HttpNotFound();
            }
            return PartialView("_ReviewDetails", leaveRequests);
        }

        public ActionResult DetailsJson(string id)
        {
            LeaveRequests leaveRequests = db.LeaveRequests.Find(id);
            if (leaveRequests == null)
            {
                return HttpNotFound();
            }
            return Json(leaveRequests,JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult EditLeave(string[] checkItem)
        {

            foreach (var item in checkItem)
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
            return RedirectToAction("Index","Review",new { type = 1, status = 1,month=-1 });
        }

        public ActionResult EditOverTime(string[] checkItem)
        {

            foreach (var item in checkItem)
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
            return RedirectToAction("Index", new { type = 3, status = 1, month = -1 });
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
