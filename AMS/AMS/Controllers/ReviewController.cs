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
        //[AllowAnonymous]
        //public ActionResult ReviewIndexForMobile()
        //{
        //   // return View(ReadLeaveRequests(1));
        //}

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
                return (spread.Days * 8* 24) - (spread.Days * 16);
            }
        }

        public ActionResult ReadLeaveRequestsForChart(string id)
        {
            int month=int.Parse(id);
            var result = from l in db.LeaveRequests.AsEnumerable()
                         join e in db.Employees.AsEnumerable() on l.EmployeeID equals e.EmployeeID
                         join r in db.ReviewStatus.AsEnumerable() on l.ReviewStatusID equals r.ReviewStatusID
                         where l.ReviewStatusID == 1 && l.StartTime.Month == month
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
        public ActionResult OverTimeRequestForChart(string id)
        {
            int month=int.Parse(id);
            var result = ReadLeaveRequests(month,1);
            result.Select(r => r.EmployeeName);
            var leaveType = result.Select(r => r.LeaveType);
            var a = from e in result
                    where e.LeaveType == "病假" && e.EmployeeID == "MIST0001"
                    select e;

            var overTimeRequest = (from ot in db.OverTimeRequest.AsEnumerable()
                                   join emp in db.Employees.AsEnumerable() on ot.EmployeeID equals emp.EmployeeID
                                   join rev in db.ReviewStatus.AsEnumerable() on ot.ReviewStatusID equals rev.ReviewStatusID
                                   join date in db.WorkingDaySchedule.AsEnumerable() on ot.StartTime.Date equals date.Date
                                   where ot.ReviewStatusID == 1
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
        public ActionResult Index(string id = "1",string id2="1")
        {//id=>1 休假; id=3加班 
         // ie2=>reviewStatus 1=未審核,2已審核
            //string User = Convert.ToString(Session["UserName"]);
            //if (User != "")
            //{
            //    db.Employees.Where(e => e.EmployeeName == User);
            //    var departID = from e in db.Employees
            //                   join d in db.Departments on e.DepartmentID equals d.DepartmentID
            //                   where e.EmployeeID == User
            //                   select d.DepartmentID;
            //}
            int i = int.Parse(id);
            int i2 = int.Parse(id2);
            int month = DateTime.Now.Month;
            IEnumerable result = null;
            switch (i)
            {
                case 3:
                    result = (IEnumerable)ReadOverTimeRequests(month,i2);
                    break;
                case 1:
                    result = (IEnumerable)ReadLeaveRequests(month,i2);
                    break;

                default:
                    break;
            }

            TempData["RequestType"] = id;
            TempData["ReviewStatus"] = id2;
            return PartialView("_ReviewIndex", result);
        }


        public ActionResult AjaxLeave( string id = "1")
        {
            int i = int.Parse(id);
            int m =DateTime.Now.Month;
            TempData["RequestType"] = 1;
            TempData["ReviewStatus"] = id;
            TempData["Month"] = m;
            return PartialView("_LeavePartial", ReadLeaveRequests(m, i));
        }

        public ActionResult AjaxLeave( string month,string id = "1")
        {
            int i = int.Parse(id);
            int m=int.Parse(month);
            TempData["RequestType"] = 1;
            TempData["ReviewStatus"] = id;
            TempData["Month"] = m;
            return PartialView("_LeavePartial", ReadLeaveRequests(m,i));
        }

        //public ActionResult AjaxClockInApply(string id = "1")
        //{//todo
        //    int i = int.Parse(id);
        //    return PartialView("_LeavePartial", ReadClockInApply(i));

        //}

        public ActionResult AjaxOvertime( string id = "1")
        {
            int i = int.Parse(id);
            int m = DateTime.Now.Month;
            TempData["RequestType"] = 3;
            TempData["ReviewStatus"] = id;
            TempData["Month"] = m;
            return PartialView("_OverTimePartial", ReadOverTimeRequests(m, i));
        }

        public ActionResult AjaxOvertime(string month, string id = "1")
        {
            int i = int.Parse(id);
            int m = int.Parse(month);
            TempData["RequestType"] = 3;
            TempData["ReviewStatus"] = id;
            TempData["Month"] = m;
            return PartialView("_OverTimePartial", ReadOverTimeRequests(m, i));
        }

        //public ActionResult ClockInForChart()
        //{
        //    var result=ReadClockInApply(1);
        //    var s = from r in result
        //            group r by r.EmployeeName into g
        //            select new
        //            {
        //                g.Key,
        //                count = g.Count()
        //            };
        //    return Json(s, JsonRequestBehavior.AllowGet);
        //}
        //public IEnumerable<AMS.Models.ClockInApplyViewModel> ReadClockInApply(int id)
        //{
        //    var result = from c in db.ClockInApply
        //                 join e in db.Employees on c.EmployeeID equals e.EmployeeID
        //                 join r in db.ReviewStatus on c.ReviewStatusID equals r.ReviewStatusID
        //                 where c.ReviewStatusID == id
        //                 select new ClockInApplyViewModel
        //                 {
        //                     EmployeeID = c.EmployeeID,
        //                     EmployeeName = e.EmployeeName,
        //                     OnDuty = c.OnDuty,
        //                     OffDuty = c.OffDuty,
        //                     RequestDate = c.RequestDate,
        //                     ReviewStatus1 = c.ReviewStatusID.ToString()

        //                 };
        //    return result;
        //}


        public IEnumerable<AMS.Models.OverTimeReviewViewModels> ReadOverTimeRequests(int month, int reviewStatus)
        {
            var result = from l in db.OverTimeRequest
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
            return result;
        }

        [AllowAnonymous]
        public IEnumerable<AMS.Models.LeaveReviewViewModels> ReadLeaveRequests(int month,int reviewStatus)
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
        public ActionResult EditOverTime(string[] Checkboxxx)
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
            return RedirectToAction("Index",new { id = 3, id2 = 1 });
        }

        public ActionResult EditLeave(string[] Checkboxxx)
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
            return RedirectToAction("Index", new { id = 1, id2 = 1 });
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
