using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AMS.Models;
using isRock.LineBot;
using Quartz;
using Quartz.Impl;
using WebApplication5.Controllers;

namespace AMS.Controllers
{
    public class AttendancesController : Controller
    {
        internal Entities db = new Entities();

        // GET: Attendances
        public ActionResult Index()
        {
            return View();
        }
        
          public ActionResult LineSerchAttendances()
        {
            string EmployeeID = Convert.ToString(Session["UserName"]);
            var query = db.Attendances.Where(Att => Att.EmployeeID == EmployeeID).Join(db.Employees, Attendances => Attendances.EmployeeID, Employees => Employees.EmployeeID, (Attendances, Employees) => new AttendancesViewModel
            {
                EmployeeID = Attendances.EmployeeID,
                EmployeeName = Employees.EmployeeName,
                Date = Attendances.Date,
                OnDuty = Attendances.OnDuty,
                OffDuty = Attendances.OffDuty,
                station = Attendances.station
            });
            return View(query);
        }
        public ActionResult TakeFive_Uncheck()//////取五筆未出勤紀錄
        {
            string EmployeeID = Convert.ToString(Session["UserName"]);
            var searchall = db.Attendances.Where(p => p.EmployeeID == EmployeeID && (p.station == "上班未打卡"||p.station=="下班未打卡"||p.station=="整日未打卡")).Select(Attendances => new AttendancesViewModel
            {
                EmployeeID = Attendances.EmployeeID,               
                Date = Attendances.Date,
                OnDuty = Attendances.OnDuty,
                OffDuty = Attendances.OffDuty,
                station = Attendances.station
            });
            int takefive;

            if (searchall.Count() <= 5)
            {
                takefive = searchall.Count();
            }
            else
            {
                takefive = 5;
            }
            var searchfive = searchall.Take(takefive);
            if (searchfive.FirstOrDefault() == null)
            {

                ViewBag.flag = true;
            }
            else
            {
                ViewBag.flag = false;
            }
            return PartialView("TakeFive_Uncheck", searchfive);
        }
        public int monthly_uncheck()////統計本月未出勤天數
        {
            string EmployeeID = Convert.ToString(Session["UserName"]);
            DateTime FirstDay = DateTime.Now.AddDays(-DateTime.Now.Day + 1);//////取每月第一天
            DateTime LastDay = DateTime.Now.AddMonths(1).AddDays(-DateTime.Now.AddMonths(1).Day);//////取每月最後一天
            var monthly_uncheck = db.Attendances.Where(p => p.Date >= FirstDay && p.Date <= LastDay && (p.station=="上班未打卡"||p.station=="下班未打卡"||p.station=="整日未打卡"));
            //var CountLeaveDays = db.LeaveRequests.Where(p => p.EmployeeID == EmployeeID && p.StartTime >= FirstDay && p.EndTime <= LastDay && p.ReviewStatusID == 2);
            int sum = monthly_uncheck.Count();
            return sum;
        }
        public int totalhours()////統計本月出勤時數
        {

            string EmployeeID = Convert.ToString(Session["UserName"]);
            //int hours = 0;
            DateTime FirstDay = DateTime.Now.AddDays(-DateTime.Now.Day + 1);//////取每月第一天
            DateTime LastDay = DateTime.Now.AddMonths(1).AddDays(-DateTime.Now.AddMonths(1).Day);//////取每月最後一天
            var monthly = db.Attendances.Where(p => p.EmployeeID == EmployeeID && p.Date >= FirstDay && p.Date <= LastDay).Select(n => new { n.EmployeeID, n.savehours });
            int sum = 0;
            foreach (var test in monthly)
            {
                if (test.savehours.HasValue)
                {
                    sum += test.savehours.Value;
                }


            }
            return sum;
        }
        public int AttendanceDays()////統計目前出勤天數
        {
            string EmployeeID = Convert.ToString(Session["UserName"]);
            DateTime FirstDay = DateTime.Now.AddDays(-DateTime.Now.Day + 1);//////取每月第一天
            DateTime LastDay = DateTime.Now.AddMonths(1).AddDays(-DateTime.Now.AddMonths(1).Day);//////取每月最後一天
            var monthly = db.Attendances.Where(p => p.EmployeeID == EmployeeID && p.Date >= FirstDay && p.Date <= LastDay);
            int sum = monthly.Count();
            return sum;
        }
        public ActionResult SerchAttendances()
        {
            string EmployeeID = Convert.ToString(Session["UserName"]);
            DateTime FirstDay = DateTime.Now.AddDays(-DateTime.Now.Day + 1);//////取每月第一天
            DateTime LastDay = DateTime.Now.AddMonths(1).AddDays(-DateTime.Now.AddMonths(1).Day);//////取每月最後一天           
            var query = db.Attendances.Where(Att => Att.EmployeeID == EmployeeID).Join(db.Employees, Attendances => Attendances.EmployeeID, Employees => Employees.EmployeeID, (Attendances, Employees) => new AttendancesViewModel
            {
                EmployeeID = Attendances.EmployeeID,
                EmployeeName = Employees.EmployeeName,
                Date = Attendances.Date,
                OnDuty = Attendances.OnDuty,
                OffDuty = Attendances.OffDuty,
                station = Attendances.station
            }).Where(p => p.Date >= FirstDay && p.Date <= LastDay && (p.station == "上班未打卡" || p.station == "下班未打卡" || p.station == "整日未打卡"));
            ViewBag.show_uncheck = monthly_uncheck();
            ViewBag.show_dutyDays = AttendanceDays();



            return PartialView("_SerchAttendances", query);

            //return View();
        }

      
        [HttpPost]
        public ActionResult SerchAttendances(DateTime time1,DateTime time2)
        {
            string EmployeeID = Convert.ToString(Session["UserName"]);
            var query = db.Attendances.Join(db.Employees, Attendances => Attendances.EmployeeID, Employees => Employees.EmployeeID, (Attendances, Employees) => new AttendancesViewModel
            {
                EmployeeID = Attendances.EmployeeID,
                EmployeeName = Employees.EmployeeName,
                Date = Attendances.Date,
                OnDuty = Attendances.OnDuty,
                OffDuty = Attendances.OffDuty,
                station = Attendances.station
            }).Where(Att => Att.EmployeeID == EmployeeID&&Att.Date>=time1&&Att.Date<=time2);





            return PartialView("_SerchAttendances",query);

            //return Json("'Success':'true'");
        }

        // GET: Attendances/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attendances attendances = db.Attendances.Find(id);
            if (attendances == null)
            {
                return HttpNotFound();
            }
            return View(attendances);
        }

        // GET: Attendances/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Attendances/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LineID,Date,Onduty,Offduty,Station,Localtion")] Attendances attendances)
        {
            if (ModelState.IsValid)
            {
                db.Attendances.Add(attendances);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(attendances);
        }

        // GET: Attendances/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attendances attendances = db.Attendances.Find(id);
            if (attendances == null)
            {
                return HttpNotFound();
            }
            return View(attendances);
        }

        // POST: Attendances/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LineID,Date,Onduty,Offduty,Station,Localtion")] Attendances attendances)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attendances).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(attendances);
        }

        // GET: Attendances/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attendances attendances = db.Attendances.Find(id);
            if (attendances == null)
            {
                return HttpNotFound();
            }
            return View(attendances);
        }

        // POST: Attendances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Attendances attendances = db.Attendances.Find(id);
            db.Attendances.Remove(attendances);
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
