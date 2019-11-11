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
        public ActionResult SerchAttendances()
        {
            string EmployeeID = "MSIT1230005";
            var query = db.Attendances.Where(Att => Att.EmployeeID == EmployeeID ).Join(db.Employees, Attendances => Attendances.EmployeeID, Employees => Employees.EmployeeID, (Attendances, Employees) => new AttendancesViewModel
            {
                EmployeeID = Attendances.EmployeeID,
                EmployeeName = Employees.EmployeeName,
                Date = Attendances.Date,
                OnDuty = Attendances.OnDuty,
                OffDuty = Attendances.OffDuty,
                station = Attendances.station
            });





            return PartialView("_SerchAttendances", query);

            //return View();
        }

      
        [HttpPost]
        public ActionResult SerchAttendances(DateTime time1,DateTime time2)
        {
            string EmployeeID = "MSIT1230005";
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

    public class ExecuteTaskServiceCallScheduler
    {
        private static readonly string ScheduleCronExpression = ConfigurationManager.AppSettings["ExecuteTaskScheduleCronExpression"];

        public static async System.Threading.Tasks.Task StartAsync()
        {
            try
            {
                var scheduler = await StdSchedulerFactory.GetDefaultScheduler();

                if (!scheduler.IsStarted)
                {
                    await scheduler.Start();
                }

                var job = JobBuilder.Create<ExecuteTaskServiceCallJob>()
                    .WithIdentity("ExecuteTaskServiceCallJob1", "group1")
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity("ExecuteTaskServiceCallTrigger1", "group1")
                    .WithCronSchedule(ScheduleCronExpression)
                    .Build();

                await scheduler.ScheduleJob(job, trigger);
            }
            catch (Exception ex)
            {

            }
        }
    }
    public class ExecuteTaskServiceCallJob : IJob
    {
        internal Entities db = new Entities();
        public const string channelAccessToken = @"wJvLiDuDsJpYsgTqSPXQwu35UoXbtmVPXn8Q1/oWN8REU5mbLG0qBffnpgSlNWH3yncYUa3OAgyWoe8gPb8F1nFveUGakkBJ2UHqUKSXElkHhypyGWz7Ndhojww+2P0+ikiFbIIkz6nhMQwetqG1gwdB04t89/1O/w1cDnyilFU=
";

        public static readonly string SchedulingStatus = ConfigurationManager.AppSettings["ExecuteTaskServiceCallSchedulingStatus"];
        public Task Execute(IJobExecutionContext context)
        {
            var task = Task.Run(() =>
            {
                if (SchedulingStatus.Equals("ON"))
                {
                    LineBotWebHookController LineBot = new LineBotWebHookController();
                    string EmployeeID = "MSIT1230005";
                    DateTime d = new DateTime();                 
                                    
                    //設定ChannelAccessToken(或抓取Web.Config)
                    LineBot.ChannelAccessToken = channelAccessToken;
                    //var query = db.Attendances.Where(p => p.Date == d.Date && p.EmployeeID == EmployeeID).First();
                    try
                    {
                        
                        var bot = new Bot(channelAccessToken);
                        //var LineEvent = LineBot.ReceivedMessage.events.FirstOrDefault();
                        bot.PushMessage("U169cd14d449bd344525284f52fec1d6b", "測試");
                        //if(query.station!=null)
                        using (var message = new MailMessage("wingrovepank@gmail.com", "hauwei.pong@gmail.com"))
                        {
                            message.Subject = "Message Subject test";
                            message.Body = "Message body test at " + DateTime.Now;
                            using (SmtpClient client = new SmtpClient
                            {
                                EnableSsl = true,
                                Host = "smtp.gmail.com",
                                Port = 587,
                                Credentials = new NetworkCredential("wingrovepank@gmail.com", "sss22040")
                            })
                            {
                                client.Send(message);
                            }
                        } //Do whatever stuff you want
                    }
                    catch (Exception ex)
                    {

                    }

                }

            });

            return task;
        }

    }


}
