using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using AMS.Metadata;
using AMS.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace AMS.Controllers
{
    public class LeaveRequestsController : Controller
    {

        private Entities db = new Entities();


       

        public ActionResult LeaveIndexView()
        {
            string User = Convert.ToString(Session["UserName"]);  //從Session抓UserID
            var LeaveTimeRequest = (from lt in db.LeaveRequests.AsEnumerable()
                                    join emp in db.Employees.AsEnumerable() on lt.EmployeeID equals emp.EmployeeID
                                    join rev in db.ReviewStatus.AsEnumerable() on lt.ReviewStatusID equals rev.ReviewStatusID
                                    where lt.EmployeeID == User
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
            

            return PartialView("LeaveIndexView", LeaveTimeRequest);

        }

        
        [HttpPost]
        public ActionResult Leave11(string id,string id2, string[] value) //傳入開始時間,結束時間,假別
        {
            string User = Convert.ToString(Session["UserName"]);  //從Session抓UserID
            DateTime startime = Convert.ToDateTime(id);
            DateTime endtime = Convert.ToDateTime(id2);
            string[] LeaveValue = value;

           
            var LeaveTimeRequest = (from lt in db.LeaveRequests.AsEnumerable()
                                    join emp in db.Employees.AsEnumerable() on lt.EmployeeID equals emp.EmployeeID
                                    join rev in db.ReviewStatus.AsEnumerable() on lt.ReviewStatusID equals rev.ReviewStatusID
                                    where lt.StartTime >= startime && lt.EndTime <= endtime && lt.EmployeeID == User&&LeaveValue.Any(x=>x==lt.LeaveType)
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
      // GET: LeaveRequests/Details/5
        public ActionResult Details(string id)
        {

            string User = Convert.ToString(Session["UserName"]);  //從Session抓UserID
            var LeaveTimeRequest = (from lt in db.LeaveRequests.AsEnumerable()
                                    join emp in db.Employees.AsEnumerable() on lt.EmployeeID equals emp.EmployeeID
                                    join rev in db.ReviewStatus.AsEnumerable() on lt.ReviewStatusID equals rev.ReviewStatusID
                                    where lt.EmployeeID == User&&lt.LeaveRequestID==id
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
                                    }).First();

            return PartialView("Details", LeaveTimeRequest);
        }

        // GET: LeaveRequests/Create

        public class LeaveCombobox
        {
            public string comtext { get; set; }
            public string text { get; set; }
        }


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

        #region 算當月請假時數總和
        public int AllLeaveSun()
        {
            string User = Convert.ToString(Session["UserName"]);
            LeaveRequests L = new LeaveRequests();
            //var dbLeave = db.LeaveRequests;
            var dbseday = db.LeaveRequests.AsEnumerable().ToList().Where(n => n.EmployeeID == User&&((n.StartTime.Month == DateTime.Now.Month)|| (n.EndTime.Month == DateTime.Now.Month))).Sum(n=> AllMonth(n.StartTime,n.EndTime));
        
            //var All = dbLeave.Where(n => n.EmployeeID == User&&DateTime.Now.Year==n.StartTime);



            return dbseday;
        }
        #endregion

        #region 算當月請假時數總和
        public int AllMonth(DateTime start ,DateTime end)
        {

            if ((start.Month == DateTime.Now.Month) && (end.Month == DateTime.Now.Month))  //開始時間和結束時間的月份都等於當月份
            {
                var sum = GetLeaveDay(start, end);
                return sum;

            }
            else if ((start.Month < DateTime.Now.Month) && (end.Month == DateTime.Now.Month))//開始時間小於當月份，結束時間等於當月
            {
                var FirstDay = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
                var yesday = DateTime.Parse($"{FirstDay.Year}-{FirstDay.Month}-{FirstDay.Day} 08:00:00");
                var sum = GetLeaveDay(yesday, end);
                return sum;

            }
            else if ((start.Month == DateTime.Now.Month) && (end.Month > DateTime.Now.Month))//開始時間等於當月，結束時間大於當月份
            {
                var LastDay = DateTime.Now.AddMonths(1).AddDays(-DateTime.Now.AddMonths(1).Day);
                var yesday = DateTime.Parse($"{LastDay.Year}-{LastDay.Month}-{LastDay.Day} 17:00:00");
                var sum = GetLeaveDay(end, yesday);

                return sum;
            }
            else//開始時間和結束時間都不等於當月
            {
                return 0;
            }




        }
        #endregion


        #region 判斷是否為補休假
        public int SumLeave()
        {
            string User = Convert.ToString(Session["UserName"]);


            var aa = db.LeaveRequests.AsEnumerable().Where(n => n.EmployeeID == User && n.LeaveType == "補休假").ToList().Sum(n=> GetLeaveDay(n.StartTime,n.EndTime));
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

            DateTime dtFirstDayGoToWork = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 08, 30, 0);//請假第一天的上班時間
            DateTime dtFirstDayGoOffWork = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 17, 30, 0);//請假第一天的下班時間

            DateTime dtLastDayGoToWork = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 08, 30, 0);//請假最後一天的上班時間
            DateTime dtLastDayGoOffWork = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 17, 30, 0);//請假最後一天的下班時間

            DateTime dtFirstDayRestStart = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 12, 00, 0);//請假第一天的午休開始時間
            DateTime dtFirstDayRestEnd = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 13, 00, 0);//請假第一天的午休結束時間

            DateTime dtLastDayRestStart = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 12, 00, 0);//請假最後一天的午休开始时间
            DateTime dtLastDayRestEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 13, 00, 0);//請假最後一天的午休结束时间

            //if (!IsWorkDay(dtStart) && !IsWorkDay(dtEnd))
            //    return 0;
            //if (dtStart >= dtFirstDayGoOffWork && dtEnd <= dtLastDayGoToWork && (dtEnd - dtStart).TotalDays < 1)
            //    return 0;
            //if (dtStart >= dtFirstDayGoOffWork && !IsWorkDay(dtEnd) && (dtEnd - dtStart).TotalDays < 1)
            //    return 0;


            if (dtStart < dtFirstDayGoToWork)//開始時間早於請假第一天上班时间(當天還沒上班就請假)
            {
                dtStart = dtFirstDayGoToWork;
            }
            if (dtStart >= dtFirstDayGoOffWork)//如果請假開始晚於第一天下班时间(下班時間才請假)
            {
                while (dtStart < dtEnd) //正確繼續做  因為請假開始晚於當天下班所以new一個隔天新的隔天8點開始請假
                {
                    dtStart = new DateTime(dtStart.AddDays(1).Year, dtStart.AddDays(1).Month, dtStart.AddDays(1).Day, 08, 30, 00);    //隔日的早上八點開始上班=請假時間重上班開始算
                    if (IsWorkDay(dtStart))
                    {
                        dtFirstDayGoToWork = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 08, 30, 00);//請假第一天的上班时间
                        dtFirstDayGoOffWork = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 17, 30, 00);//請假第一天的下班时间
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
                    dtEnd = new DateTime(dtEnd.AddDays(-1).Year, dtEnd.AddDays(-1).Month, dtEnd.AddDays(-1).Day, 17, 30, 00);  //因為結束時間早於上班時間,所以要用前一天的下班時間來做計算
                    if (IsWorkDay(dtEnd))
                    {
                        dtLastDayGoToWork = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 08, 30, 0);//請假最後一天的上班時間
                        dtLastDayGoOffWork = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 17, 30, 0);//請假最後一天的下班時間
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
            double doubleSumHours = ((leaveday-2) * 8) + iSumMinute / 60;
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
            var a = (db.LeaveRequests.AsEnumerable().Where(n => (n.StartTime >= t1 && n.EndTime <= t2 && n.EmployeeID == User && n.LeaveType == "特休假"&&n.ReviewStatusID==2)).Sum(x => (x.EndTime.Date - x.StartTime.Date).Days));
            int Remain = Days() - a;
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



        public ActionResult Create()
        {

            string User = Convert.ToString(Session["UserName"]);  //從Session抓UserID
            ViewBag.LeaveType = new SelectList(new List<LeaveCombobox> {
                new LeaveCombobox {comtext="事假",text="事假" },
                new LeaveCombobox {comtext="病假",text="病假"},
                new LeaveCombobox {comtext="公假",text="公假"},
                new LeaveCombobox {comtext="喪假",text="喪假"},
                new LeaveCombobox {comtext="特休假",text="特休假"},
                new LeaveCombobox {comtext="產假",text="產假"},
                new LeaveCombobox {comtext="陪產假",text="陪產假"},
                new LeaveCombobox {comtext="生理假",text="生理假"},
                new LeaveCombobox {comtext="補休假",text="補休假"},
                new LeaveCombobox {comtext="家庭照顧假",text="家庭照顧假"},
            }, "text", "comtext");

           ViewBag.on = remleave();  //剩餘特休天數
           ViewBag.Off= Days();      //特休天數共幾天
           ViewBag.over = SumoverTime()- SumLeave();//計算當年度需補修時數
           ViewBag.Leave = SumLeave();//計算當年度補修申時數
            ViewBag.Leave1 = AllLeaveSun();
            return PartialView("_Create");
        }

        // POST: LeaveRequests/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LeaveFile,LeaveRequestID,EmployeeID,RequestTime,StartTime,EndTime,LeaveType,LeaveReason,ReviewStatusID,ReviewTime,Attachment")] LeaveRequestsViewModel leaveR)
        {
            LeaveRequests leaveRequests = new LeaveRequests();
            if (ModelState.IsValid)
            {
                string User = Convert.ToString(Session["UserName"]);  //從Session抓UserID
                leaveRequests.LeaveRequestID = db.LeaveRequests.Count().ToString();
                leaveRequests.EmployeeID = User;
                leaveRequests.RequestTime = DateTime.Now.AddHours(8);
                leaveRequests.ReviewStatusID = 1;
                leaveRequests.LeaveReason = leaveR.LeaveReason;

                leaveRequests.StartTime = leaveR.StartTime;
                leaveRequests.EndTime = leaveR.EndTime;
                leaveRequests.LeaveType = leaveR.LeaveType;




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

                //判斷是不是有申請過()
                if (db.LeaveRequests.Any(n=>n.EmployeeID==User&&!(n.LeaveRequestID=="3")&&((n.StartTime<=leaveRequests.StartTime&&n.EndTime>=leaveRequests.StartTime) ||(n.StartTime <= leaveRequests.EndTime && n.EndTime>= leaveRequests.EndTime))))
                {


                    Response.TrySkipIisCustomErrors = true;
                    //throw new Exception("此日期已申請過, 請先確認日期");
                    Response.StatusCode = 500;
                    return Json(new { msg = "此日期已申請過, 請先確認日期" }, JsonRequestBehavior.AllowGet);
                    //return Content(("<script>alert('此日期已申請過,請先確認日期');window.location.href ='http://localhost:64643/Home/Index'</script>"));
                
                }
           
                    db.LeaveRequests.Add(leaveRequests);
                    db.SaveChanges();
                //return RedirectToAction("LeaveIndexView");
                //return RedirectToAction("Index");
                return Json(new { msg = "送出申請，待主管審核" }, JsonRequestBehavior.AllowGet);
                //return Content(("<script>alert();window.location.href ='http://localhost:64643/Home/Index'</script>"));
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

        [HttpPost]
        public ActionResult DownLoadExcel(string id, string id2, string[] value)
        {
            #region 查詢資料
 
            string User = Convert.ToString(Session["UserName"]);  //從Session抓UserID
            DateTime startime = Convert.ToDateTime(id);
            DateTime endtime = Convert.ToDateTime(id2);
            string[] LeaveValue = value;


            var LeaveTimeRequest = from lt in db.LeaveRequests.AsEnumerable()
                                    join emp in db.Employees.AsEnumerable() on lt.EmployeeID equals emp.EmployeeID
                                    join rev in db.ReviewStatus.AsEnumerable() on lt.ReviewStatusID equals rev.ReviewStatusID
                                    where lt.StartTime >= startime && lt.EndTime <= endtime && lt.EmployeeID == User && LeaveValue.Any(x => x == lt.LeaveType)
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
                                    };

            string[] titleList = new string[] { "序號", "假單編號", "員工姓名", "假別", "申請時間", "申請開始時間", "申請結束時間", "事由", "申請狀態"};
            List<LeaveIndexViewModel> list = LeaveTimeRequest.ToList();/*OrderInfoBLL.GetOrdersInfoListByTime(OrderStartTime, OrderEndTime);*/
            #endregion

            #region NPOI資料匯出
            //建立Excel檔案的物件
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //新增一個sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //給sheet1新增第一行的頭部標題
            IRow headerrow = sheet1.CreateRow(0);
            //給sheet1欄寬
            sheet1.SetColumnWidth(0, 5 * 256);
            sheet1.SetColumnWidth(1, 10 * 256);
            sheet1.SetColumnWidth(2, 10 * 256);
            sheet1.SetColumnWidth(3, 15 * 256);
            sheet1.SetColumnWidth(4, 15 * 256);
            sheet1.SetColumnWidth(5, 20 * 256);
            sheet1.SetColumnWidth(6, 20 * 256);
            sheet1.SetColumnWidth(7, 25 * 256);
            sheet1.SetColumnWidth(8, 8 * 256);

            //標題樣式
            HSSFCellStyle headStyle = (HSSFCellStyle)book.CreateCellStyle();
            HSSFFont font = (HSSFFont)book.CreateFont();
            font.FontHeightInPoints = 10;
            font.Boldweight = 700;
            headStyle.Alignment = HorizontalAlignment.Left;


            headStyle.SetFont(font);
            
            for (int i = 0; i < titleList.Length; i++)
            {
                ICell cell = headerrow.CreateCell(i);
                cell.CellStyle = headStyle;
                cell.SetCellValue(titleList[i]);
                

            }
            //設定寫入資料的樣式
            HSSFCellStyle ContentStyle = (HSSFCellStyle)book.CreateCellStyle();
            ContentStyle.Alignment= HorizontalAlignment.Right;
            //將資料逐步寫入sheet1各個行
            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(i + 1);

                var cell1 = rowtemp.CreateCell(1);
                cell1.SetCellValue(list[i].LeaveRequestID);
                cell1.CellStyle = ContentStyle;
                var cell2 = rowtemp.CreateCell(2);
                cell2.SetCellValue(list[i].EmployeeName);
                cell2.CellStyle = ContentStyle;
                var cell3 = rowtemp.CreateCell(3);
                cell3.SetCellValue(list[i].LeaveType);
                cell3.CellStyle = ContentStyle;
                var cell4 = rowtemp.CreateCell(4);
                cell4.SetCellValue(list[i].RequestTime.ToString("yyyy-MM-dd"));
                cell4.CellStyle = ContentStyle;
                var cell5 = rowtemp.CreateCell(5);
                cell5.SetCellValue(list[i].StartTime.ToString("yyyy-MM-dd HH:mm"));
                cell5.CellStyle = ContentStyle;
                var cell6 = rowtemp.CreateCell(6);
                cell6.SetCellValue(list[i].EndTime.ToString("yyyy-MM-dd HH:mm"));
                cell6.CellStyle = ContentStyle;
                var cell7 = rowtemp.CreateCell(7);
                cell7.SetCellValue(list[i].LeaveReason);
                cell7.CellStyle = ContentStyle;
                var cell8 = rowtemp.CreateCell(8);
                cell8.SetCellValue(list[i].Review == "未審核" ? "未審核" : list[i].Review == "已審核" ? "已審核" : list[i].Review == "駁回" ? "駁回" : "取消稽核不通過");
                cell8.CellStyle = ContentStyle;
                //rowtemp.CreateCell(9).SetCellValue(list[i].ReviewTime.ToString("yyyy-MM-dd HH:mm"));

            }
            var guid = Guid.NewGuid().ToString();
            DownLoadHelper.SetCache(guid, book);
            return Json(guid, JsonRequestBehavior.AllowGet);
            #endregion
        }


        public void OutExcel(string guid)
        {
            object obj = DownLoadHelper.GetCache(guid);
            NPOI.HSSF.UserModel.HSSFWorkbook book = obj as NPOI.HSSF.UserModel.HSSFWorkbook;
            if (book != null)
            {
                //寫入到客戶端
                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}.xls", DateTime.Now.ToString("yyyyMMddHHmmssfff")));
                Response.BinaryWrite(ms.ToArray());
                book = null;
                ms.Close();
                ms.Dispose();
            }
            DownLoadHelper.RemoveAllCache(guid);
        }


    }
}
