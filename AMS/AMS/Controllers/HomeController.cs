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
            ViewBag.on = remleave();  //剩餘特休天數
            ViewBag.Off = Days();      //特休天數共幾天
            ViewBag.over = SumoverTime() - SumLeave();//計算當年度需補修時數
            ViewBag.Leave = SumLeave();//計算當年度補修申時數
            ViewBag.show1 = monthlyhours();
            ViewBag.show2 = AttendanceDays();
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
        public int monthlyhours()////統計本月應到天數
        {
            string EmployeeID = Convert.ToString(Session["UserName"]);
            DateTime FirstDay = DateTime.Now.AddDays(-DateTime.Now.Day + 1);//////取每月第一天
            DateTime LastDay = DateTime.Now.AddMonths(1).AddDays(-DateTime.Now.AddMonths(1).Day);//////取每月最後一天
            var monthly = db.WorkingDaySchedule.Where(p => p.Date >= FirstDay && p.Date <= LastDay && p.WorkingDay == "工作日");
            var CountLeaveDays = db.LeaveRequests.Where(p => p.EmployeeID == EmployeeID && p.StartTime >= FirstDay && p.EndTime <= LastDay && p.ReviewStatusID == 2);
            int sum = monthly.Count();
            return sum;
        }
        public int AttendanceDays()////統計目前出勤天數
        {
            string EmployeeID = Convert.ToString(Session["UserName"]);
            DateTime FirstDay = DateTime.Now.AddDays(-DateTime.Now.Day + 1);//////取每月第一天
            DateTime LastDay = DateTime.Now.AddMonths(1).AddDays(-DateTime.Now.AddMonths(1).Day);//////取每月最後一天
            var monthly = db.Attendances.Where(p =>p.EmployeeID==EmployeeID && p.Date >= FirstDay && p.Date <= LastDay );           
            int sum = monthly.Count();
            return sum;
        }
        //public int totalhours()////統計本月出勤時數
        //{

        //    string EmployeeID = Convert.ToString(Session["UserName"]);
        //    //int hours = 0;
        //    DateTime FirstDay = DateTime.Now.AddDays(-DateTime.Now.Day + 1);//////取每月第一天
        //    DateTime LastDay = DateTime.Now.AddMonths(1).AddDays(-DateTime.Now.AddMonths(1).Day);//////取每月最後一天
        //    var monthly = db.Attendances.Where(p => p.EmployeeID == EmployeeID && p.Date >= FirstDay && p.Date <= LastDay).Select(n => new { n.EmployeeID, n.savehours });
        //    int sum = 0;
        //    foreach (var test in monthly)
        //    {
        //        if (test.savehours.HasValue)
        //        {
        //            sum += test.savehours.Value;
        //        }


        //    }
        //    return sum;
        //}
        #region 算補修時數總和
        public int SumoverTime()
        {
            OverTimeRequest Overtime = new OverTimeRequest();
            string User = Convert.ToString(Session["UserName"]);

            var dbover = db.OverTimeRequest;
            var time1 = dbover.AsEnumerable().Where(n => n.EmployeeID == User && n.StartTime.Year == DateTime.Now.Year && n.OverTimePay == false).Sum(n => (n.EndTime - n.StartTime).Hours);


            return time1;
        }

        #endregion

        #region 算剩餘的補修時數 目前沒用到
        public int removertime()
        {
            var a = DateTime.Parse("2019/12/04 08:00:00");
            var a1 = DateTime.Parse("2019/12/09 18:00:00");
            var aa1 = GetLeaveDay(a, a1);
            var a2 = DateTime.Parse("2019/12/04 18:30:00");
            var a22 = DateTime.Parse("2019/12/04 12:00:00");
            var aa22 = a - a1;
            LeaveRequests l = new LeaveRequests();
            /*db.LeaveRequests.AsEnumerable().Where(n=>n*/
            var x = (DateTime.Now - DateTime.Parse($"{DateTime.Now.Year}/{DateTime.Now.Month}/{DateTime.Now.Day} 12:00:00")).Hours;


            return x;
        }
        #endregion


        #region 判斷是否為補休假
        public int SumLeave()
        {
            string User = Convert.ToString(Session["UserName"]);


            var aa = db.LeaveRequests.AsEnumerable().Where(n => n.EmployeeID == User && n.LeaveType == "補休假").ToList().Sum(n => GetLeaveDay(n.StartTime, n.EndTime));
            //foreach(var item in aa)
            //{
            //    sumleave+=GetLeaveDay(item.StartTime, item.EndTime);

            //}
            return aa;
        }
        #endregion


        #region 判斷是否為工作天  套用在GetLeaveDay中
        /// 判断是否是工作日| true：工作 | flase：休息
        /// </summary>
        /// <param name="date">時間</param>
        /// <returns>true：工作 | flase:休息</returns>
        public bool IsWorkDay(DateTime date)
        {

            var Workday = db.WorkingDaySchedule.AsEnumerable().Where(n => n.Date == date).Select(n => n.WorkingDay).FirstOrDefault();//今年的工作假期table

            if (Workday == "工作日")
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region 任何計算時數方法 排除假日
        ///上班時間 08:00~17:00
        ///中午時間 12:00~13:00

        public int GetLeaveDay(DateTime dtStart, DateTime dtEnd)
        {

            DateTime dtFirstDayGoToWork = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 08, 00, 0);//請假第一天的上班時間
            DateTime dtFirstDayGoOffWork = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 17, 00, 0);//請假第一天的下班時間

            DateTime dtLastDayGoToWork = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 08, 00, 0);//請假最後一天的上班時間
            DateTime dtLastDayGoOffWork = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 17, 00, 0);//請假最後一天的下班時間

            DateTime dtFirstDayRestStart = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 12, 00, 0);//請假第一天的午休開始時間
            DateTime dtFirstDayRestEnd = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 13, 00, 0);//請假第一天的午休結束時間

            DateTime dtLastDayRestStart = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 12, 00, 0);//請假最後一天的午休开始时间
            DateTime dtLastDayRestEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 13, 00, 0);//請假最後一天的午休结束时间

            if (dtStart < dtFirstDayGoToWork)//開始時間早於請假第一天上班时间(當天還沒上班就請假)
            {
                dtStart = dtFirstDayGoToWork;
            }
            if (dtStart >= dtFirstDayGoOffWork)//如果請假開始晚於第一天下班时间(下班時間才請假)
            {
                while (dtStart < dtEnd) //正確繼續做  因為請假開始晚於當天下班所以new一個隔天新的隔天8點開始請假
                {
                    dtStart = new DateTime(dtStart.AddDays(1).Year, dtStart.AddDays(1).Month, dtStart.AddDays(1).Day, 08, 00, 00);    //隔日的早上八點開始上班=請假時間重上班開始算
                    if (IsWorkDay(dtStart))
                    {
                        dtFirstDayGoToWork = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 08, 00, 00);//請假第一天的上班时间
                        dtFirstDayGoOffWork = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 17, 00, 00);//請假第一天的下班时间
                        dtFirstDayRestStart = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 12, 00, 00);//請假第一天的午休开始时间
                        dtFirstDayRestEnd = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 13, 00, 00);//請假第一天的午休结束时间

                        break;
                    }
                }
            }

            if (dtEnd > dtLastDayGoOffWork)//結束時間晚於最後一天下班時間
            {
                dtEnd = dtLastDayGoOffWork;
            }
            if (dtEnd <= dtLastDayGoToWork)//結束時間早於當天上班時間
            {
                while (dtEnd > dtStart) //正確直接執行以下
                {
                    dtEnd = new DateTime(dtEnd.AddDays(-1).Year, dtEnd.AddDays(-1).Month, dtEnd.AddDays(-1).Day, 17, 00, 00);  //因為結束時間早於上班時間,所以要用前一天的下班時間來做計算
                    if (IsWorkDay(dtEnd))
                    {
                        dtLastDayGoToWork = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 08, 00, 0);//請假最後一天的上班時間
                        dtLastDayGoOffWork = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 17, 00, 0);//請假最後一天的下班時間
                        dtLastDayRestStart = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 12, 00, 0);//請假最後一天的午休開始時間
                        dtLastDayRestEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 13, 00, 0);//請假最後一天的午休結束時間
                        break;
                    }
                }
            }

            //計算請假第一天和最後一天的小时合計算並換成分鐘數           
            double iSumMinute = dtFirstDayGoOffWork.Subtract(dtStart).TotalMinutes + dtEnd.Subtract(dtLastDayGoToWork).TotalMinutes;//計算獲得剩餘的分鐘數   

            if (dtStart > dtFirstDayRestStart && dtStart < dtFirstDayRestEnd)
            {//開始休假時間正好是在午休時間内的，需要扣除掉
                iSumMinute = iSumMinute - dtFirstDayRestEnd.Subtract(dtStart).Minutes;
            }
            if (dtStart < dtFirstDayRestStart)
            {//如果是在午休前開始休假的就自動減去午休的60分鐘
                iSumMinute = iSumMinute - 60;
            }
            if (dtEnd > dtLastDayRestStart && dtEnd < dtLastDayRestEnd)
            {//如果結束休假是在午休時間内的，例如“請假截止日是1月31日 12:00分”的話那休假時間計算只到 11:30分為止。
                iSumMinute = iSumMinute - dtEnd.Subtract(dtLastDayRestStart).Minutes;
            }
            if (dtEnd > dtLastDayRestEnd)
            {//如果是在午休後结束請假的就自動減去午休的60分鐘
                iSumMinute = iSumMinute - 60;
            }

            int leaveday = 0;//實際請假的天數
            double countday = 0;//獲取兩個日期間的總天數

            DateTime tempDate = dtStart;//臨時參數
            while (tempDate < dtEnd)
            {
                countday++;
                tempDate = new DateTime(tempDate.AddDays(1).Year, tempDate.AddDays(1).Month, tempDate.AddDays(1).Day, 0, 0, 0);
            }
            //循環用来扣除雙周休、法定假日 和 添加調休上班
            for (int i = 0; i < countday; i++)
            {
                DateTime tempdt = dtStart.Date.AddDays(i);
                bool a = IsWorkDay(tempdt);
                if (IsWorkDay(tempdt))
                    leaveday++;
            }

            //去掉請假第一天和請假的最後一天，其餘時間全部已8小時計算。
            //SumMinute/60： 獨立計算 請假第一天和請假最後一天總共請了多少小時的假
            double doubleSumHours = ((leaveday - 2) * 8) + iSumMinute / 60;
            int intSumHours = Convert.ToInt32(doubleSumHours);

            if (doubleSumHours > intSumHours)//如果請假時間不足1小时話自動算為1小时
                intSumHours++;

            return intSumHours;

        }





        #endregion

        #region 算剩餘的特休天數(已審核)
        public int remleave()
        {
            string User = Convert.ToString(Session["UserName"]);  //從Session抓UserID

            var t = DateTime.Now.Year;
            var dm = db.Employees.Find(User).Hireday.Month;
            var dd = db.Employees.Find(User).Hireday.Day;
            DateTime t1 = DateTime.Parse($"{t}-{dm}-{dd}");
            DateTime t2 = DateTime.Parse($"{t + 1}-{dm}-{dd}");
            var a = (db.LeaveRequests.AsEnumerable().Where(n => (n.StartTime >= t1 && n.EndTime <= t2 && n.EmployeeID == User && n.LeaveType == "特休假" && n.ReviewStatusID == 2)).Sum(x => (x.EndTime.Date - x.StartTime.Date).Days - 1));
            int Remain = Days() + a;
            return Remain;
        }
        #endregion

        #region 算特休天數
        public int Days()
        {

            string User = Convert.ToString(Session["UserName"]);  //從Session抓UserID
            var d = db.Employees.Find(User).Hireday;
            int days = (DateTime.Now - d).Days;
            if (days >= 182 && days < 365)// 半年~1年
            {
                return 3;
            }
            else if (days >= 365 && days < 730)// 1年~2年
            {
                return 7;
            }
            else if (days >= 730 && days < 1095)// 2年~3年
            {
                return 10;
            }
            else if (days >= 1095 && days < 1825)// 3年~5年
            {
                return 14;
            }
            else if (days >= 1825 && days < 3650)// 5年~10年
            {
                return 15;
            }
            else if (days >= 3650 && days < 4015)// 11年
            {
                return 16;
            }
            else if (days >= 4015 && days < 4380)// 12年
            {
                return 17;
            }
            else if (days >= 4380 && days < 4745)// 13年
            {
                return 18;
            }
            else if (days >= 4745 && days < 5110)// 14年
            {
                return 19;
            }
            else if (days >= 5110 && days < 5475)// 15年
            {
                return 20;
            }
            else if (days >= 5475 && days < 5840)// 16年
            {
                return 21;
            }
            else if (days >= 5840 && days < 6205)// 17年
            {
                return 22;
            }
            else if (days >= 6205 && days < 6570)// 18年
            {
                return 23;
            }
            else if (days >= 6570 && days < 6935)// 19年
            {
                return 24;
            }
            else if (days >= 6935 && days < 7300)// 20年
            {
                return 25;
            }
            else if (days >= 7300 && days < 7665)// 21年
            {
                return 26;
            }
            else if (days >= 7665 && days < 8030)// 22年
            {
                return 27;
            }
            else if (days >= 8030 && days < 8395)// 23年
            {
                return 28;
            }
            else if (days >= 8395 && days < 8760)// 24年
            {
                return 29;
            }
            else
            {
                return 30;// 25年以上
            }
        }
        #endregion
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