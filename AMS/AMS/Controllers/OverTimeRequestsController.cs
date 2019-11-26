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
            string User = Convert.ToString(Session["UserName"]);  //從Session抓UserID

            var overTimeRequest = (from ot in db.OverTimeRequest.AsEnumerable()
                                   join emp in db.Employees.AsEnumerable() on ot.EmployeeID equals emp.EmployeeID
                                   join rev in db.ReviewStatus.AsEnumerable() on ot.ReviewStatusID equals rev.ReviewStatusID
                                   join date in db.WorkingDaySchedule.AsEnumerable() on ot.StartTime.Date equals date.Date
                                   where ot.EmployeeID==User
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
                                   });

            

            return PartialView("_Index",overTimeRequest);
        }

        
        public ActionResult Overtimetable (string id, string id2)
        {
            string User = Convert.ToString(Session["UserName"]);
            DateTime Starttime = Convert.ToDateTime(id.Substring(0,4)+"/"+id.Substring(4,2)+"/"+id.Substring(6,2)+" 00:00:00");
            DateTime Endtime = Convert.ToDateTime(id2.Substring(0, 4) + "/" + id2.Substring(4, 2) + "/" + id2.Substring(6, 2)+" 23:59:59");
            var Query =  (from ot in db.OverTimeRequest.AsEnumerable()
                           join emp in db.Employees.AsEnumerable() on ot.EmployeeID equals emp.EmployeeID
                           join rev in db.ReviewStatus.AsEnumerable() on ot.ReviewStatusID equals rev.ReviewStatusID
                           join date in db.WorkingDaySchedule.AsEnumerable() on ot.StartTime.Date equals date.Date
                           where ot.StartTime >= Starttime && ot.EndTime<= Endtime&& ot.EmployeeID == User
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
                           });


            
            if(Query!=null)
            {
                return PartialView("_OverTimeIndexPartialView", Query);
            }
            else
            {
                return HttpNotFound();
            }


            

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

            return PartialView("_Details",overTimeRequest);
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
            ViewBag.OverTimePay = new SelectList(dropdownlist, "value", "text");
            return PartialView("_Create");
        }

        // POST: OverTimeRequests/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StartTime,EndTime,OverTimePay,OverTimeReason")] OverTimeRequest overTimeRequest)
        {

            string User = Convert.ToString(Session["UserName"]);
            if (ModelState.IsValid)
            {
                var Query = (from ot in db.OverTimeRequest.AsEnumerable()
                                       join emp in db.Employees.AsEnumerable() on ot.EmployeeID equals emp.EmployeeID
                                       join rev in db.ReviewStatus.AsEnumerable() on ot.ReviewStatusID equals rev.ReviewStatusID
                                       join date in db.WorkingDaySchedule.AsEnumerable() on ot.StartTime.Date equals date.Date
                                       where ot.EmployeeID == User
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
                                       });
                var LeaveQuery = from leave in db.LeaveRequests
                                 where leave.EmployeeID == User
                                 select leave;
                                 
                if(Query.Any(n=>n.StartTime.Date==overTimeRequest.StartTime.Date&&!(n.Review=="3")))
                {
                    Response.StatusCode = 500;
                    return Json(new { Success = false, Message = "今天已經申請過加班" },JsonRequestBehavior.AllowGet);

                }
                if(LeaveQuery.Any(n=>n.StartTime== overTimeRequest.StartTime.Date &&!(n.ReviewStatusID==3)))
                {
                    Response.StatusCode = 500;
                    return Json(new { Success = false, Message = "今天已經申請過請假" }, JsonRequestBehavior.AllowGet);
                }


                overTimeRequest.OverTimeRequestID = Convert.ToString(db.OverTimeRequest.Count()+1);
                overTimeRequest.EmployeeID = User;
                overTimeRequest.RequestTime = DateTime.Now;
                overTimeRequest.ReviewStatusID = 1;
                
                db.OverTimeRequest.Add(overTimeRequest);
                db.SaveChanges();
                return Json(new { Success = false, Message = "申請成功" }, JsonRequestBehavior.AllowGet);
            }

            return RedirectToAction("Index", "Home");
        }

  

        

    }


}
