using AMS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace AMS.Controllers
{
    //[Authorize(Roles = "Employee,Leader,HR,FaceAccount")]
    public class HomeController : Controller
    {
        Entities db = new Entities();

        public ActionResult Index()
        {
            if (Session["UserName"]==null)
            {
                return RedirectToAction("Login1", "Account");
            }
            
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> LoginFace(string imageData)
        {
            byte[] data = Convert.FromBase64String(imageData);
            var q = db.Employees.Select(emp => new { emp.EmployeeID, emp.EmployeeName, emp.FaceID });
            foreach (var item in q)
            {
                if (item.FaceID != null)
                {

                    if (await class1.MakeRequest(item.FaceID, await class1.MakeAnalysisRequest(data, "")))
                    {
                        //return Json(new { EmployeeID = item.EmployeeID, }, JsonRequestBehavior.AllowGet);
                        //Session["UserFullName"] = db.Employees.Find(item.EmployeeID).EmployeeName;
                        //Session["UserName"] = item.EmployeeID;
                        //return RedirectToAction("Index","Home");
                        return Json(new { newUrl = Url.Action("Index", "Home") });
                    }


                }
            }

            return Json(new { msg = "XX" }, JsonRequestBehavior.AllowGet);
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

                var attend = db.Attendances.Where(att => member.Contains(att.EmployeeID) ).Select(n=>n.Date ).ToList() ;
                var DayOff = db.LeaveRequests.Where(off => member.Contains(off.EmployeeID)).Select(n=>n).ToList();
                //var everyday = db.WorkingDaySchedule.Where(day=>day.WorkingDay=="工作日" && day.Date == today.Date).Select(days => days.Date).ToList();
                //List<calendar> A = new List<calendar>();
                //foreach (var date in everyday)
                //{
                //    calendar a = new calendar();
                //    var aaa = attend.Where(att => att.Date == date).Select(n => n);
                //    var Onduty = (aaa.FirstOrDefault() != null) ? aaa.Count() : 0;
                //    var leave = DayOff.Where(lv => lv.StartTime.Date <= date && lv.EndTime.Date >= date).FirstOrDefault() != null ? DayOff.Where(lv => lv.StartTime.Date <= date && lv.EndTime.Date >= date).Count() : 0;
                //    a.title = $"上班{Onduty}人  請假{leave}人";
                //    a.start = CalendarDate(date);
                //    a.backgroundColor = "#B5CAA0";
                //    A.Add(a);
                //}
                //queryA = queryA.Concat(A);

                var everyday = db.WorkingDaySchedule.AsEnumerable().Where(day => day.WorkingDay == "工作日" && day.Date <= today.Date).Select(days => new calendar {
                    title = $"上班{GetAttd(attend, days.Date)}人  請假{GetOff(DayOff, days.Date)}人",
                    start = CalendarDate(days.Date),
                    backgroundColor = "#B5CAA0"

                });
                queryA = queryA.Concat(everyday);


            }
            string JsonPath = Server.MapPath("~/App_Data/NationalHoliday.json");
            StreamReader ReadJson = new StreamReader(JsonPath);
            string JsonString = ReadJson.ReadToEnd();

            JavaScriptSerializer JS = new JavaScriptSerializer();

            var MyJson = JS.Deserialize<HolidayFirst>(JsonString);
            var NationalDays = MyJson.result.records.Where(n => n.isHoliday == "是").Select(n => new calendar
            {
                start = CalendarDate(Convert.ToDateTime(n.date)),
                rendering = "background",
                backgroundColor = "#F8C3CD"
            });
            queryA = queryA.Concat(NationalDays);


            return Json(queryA, JsonRequestBehavior.AllowGet);

        }
        //時間改字串
        public string CalendarDate(DateTime StartTime)
        {
            string getDate = StartTime.Date.ToString("yyyy-MM-dd");
            return getDate; 
        }

        public int GetAttd(IEnumerable<DateTime> list, DateTime date )
        {
            var aaa = list.Where(att => att == date);
            return (aaa.FirstOrDefault() != null) ? aaa.Count() : 0;
        }

        public int GetOff (IEnumerable<LeaveRequests> list, DateTime date)
        {
            return list.Where(lv => lv.StartTime.Date <= date && lv.EndTime.Date >= date).FirstOrDefault() != null ? list.Where(lv => lv.StartTime.Date <= date && lv.EndTime.Date >= date).Count() : 0;
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