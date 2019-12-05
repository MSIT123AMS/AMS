using AMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Controllers
{
    public class HomeController : Controller
    {
        Entities db = new Entities();

        public ActionResult Index()
        {
          
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult LoadCalender()
        {
            string User = Convert.ToString(Session["UserName"]);  //從Session抓UserID
            //string User = "MSIT1230016";
            DateTime today = DateTime.Now;
            
            //抓員工出缺勤
            var query = db.Attendances.AsEnumerable().Where(att => att.EmployeeID == User);
            
            //上班
            
            //var quertB = db.WorkingDaySchedule;
            var queryA = from workingday in db.WorkingDaySchedule.AsEnumerable()
                         join x in query on workingday.Date.Date equals x.Date.Date into all
                         from x in all.DefaultIfEmpty()
                         where workingday.WorkingDay== "工作日"&&workingday.Date<=today.Date
                         select new  calendar{
                             title = $"上班: {(x!=null?x.OnDuty.Value.ToShortTimeString():"未打卡")}",
                             start = CalendarDate(workingday.Date),
                             backgroundColor = (x != null ? "#A5DEE4" : "#F4A7B9")
                         };

            //下班
            var query1 = db.Attendances.AsEnumerable().Where(att => att.EmployeeID == User).Select(att => new calendar
            {
                title = $"下班: {((att.OffDuty.HasValue == true) ? (att.OffDuty.Value.ToShortTimeString()) : "未打卡")}",
                start = CalendarDate(att.OnDuty.Value),
                backgroundColor = ((att.OffDuty.HasValue == true) ? "#A5DEE4" : "#F4A7B9")

            });
            //請假
            var query2 = db.LeaveRequests.AsEnumerable().Where(lv => lv.EmployeeID == User).Select(lv => new calendar
            {
                title=$"{lv.LeaveType}",
                start=CalendarDate(lv.StartTime),
                end=CalendarDate(lv.EndTime),
                backgroundColor= "#ECB88A"
            });
            queryA = queryA.Concat(query1);
            queryA = queryA.Concat(query2);

            //出勤 請假
            var Myname = db.Employees.Where(emp => emp.EmployeeID == User).Select(x => x.EmployeeName).First();
            var Department = db.Departments.Where(dep => dep.Manager == Myname).Select(x => x.DepartmentID).ToList();
            if (Department.Count() != 0)
            {


                var member = db.Employees.AsEnumerable().Where(emp => emp.DepartmentID == Department[0]).Select(x => x.EmployeeID).ToList();

                var attend = db.Attendances.Where(att => member.Contains(att.EmployeeID)).ToList();
                var DayOff = db.LeaveRequests.Where(off => member.Contains(off.EmployeeID)).ToList();
                var everyday = db.WorkingDaySchedule.Where(day=>day.WorkingDay=="工作日" && day.Date <= today.Date).Select(days => days.Date).ToList();
                List<calendar> A = new List<calendar>();
                foreach (var date in everyday)
                {
                    calendar a = new calendar();
                    var duty = attend.Where(att => att.Date == date);
                    var lve = DayOff.Where(lv => lv.StartTime.Date <= date && lv.EndTime.Date >= date);
                    bool test = (duty.ToList().FirstOrDefault() != null);
                    var Onduty = test ? duty.Count() : 0;
                    var leave = lve.FirstOrDefault() != null ? lve.Count() : 0;
                    a.title = $"上班{Onduty}人  請假{leave}人";
                    a.start = CalendarDate(date);
                    a.backgroundColor = "#B5CAA0";
                    A.Add(a);
                }

                queryA = queryA.Concat(A);

            }


            return Json(queryA, JsonRequestBehavior.AllowGet);

        }
        //時間改字串
        public string CalendarDate(DateTime StartTime)
        {
            string getDate = StartTime.Date.ToString("yyyy-MM-dd");
            return getDate; 
        }

        //缺勤明細
        
        public ActionResult DayDutyList(string id)
        {
            DateTime date = DateTime.Parse(id);
            string User = Convert.ToString(Session["UserName"]);  //從Session抓UserID
            //string User = "MSIT1230016";
            //抓員工出缺勤
            var query = db.Attendances.AsEnumerable().Where(att => att.EmployeeID == User);

            var IDtoName = db.Employees.ToDictionary(n => n.EmployeeID, n => n.EmployeeName);

            var Myname = db.Employees.Where(emp => emp.EmployeeID == User).Select(x => x.EmployeeName).First();
            var Department = db.Departments.Where(dep => dep.Manager == Myname).Select(x => x.DepartmentID).ToList();
            
           

                var member = db.Employees.AsEnumerable().Where(emp => emp.DepartmentID == Department[0]).Select(x => x.EmployeeID).ToList();

                var attend = db.Attendances.Where(att => member.Contains(att.EmployeeID)).ToList();
                var DayOff = db.LeaveRequests.Where(off => member.Contains(off.EmployeeID)).ToList();
                var duty = attend.Where(att => att.Date == date).Select(n=>new CalendarAttendance{
                     AbsenceType="打卡",
                     Name=$"{IDtoName[n.EmployeeID]}"
                });
                var lve = DayOff.Where(lv => lv.StartTime.Date <= date && lv.EndTime.Date >= date).Select(n => new CalendarAttendance
                {
                    AbsenceType = "請假",
                    Name = $"{IDtoName[n.EmployeeID]}"
                }); 
                             
                duty = duty.Concat(lve);
                return PartialView("_DayDutyList", duty);
         
        }

    }
}