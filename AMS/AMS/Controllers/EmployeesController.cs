using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AMS.Models;

using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

using System.Collections;
using WebApplication5.Controllers;
using NPOI.SS.Formula.Functions;

namespace AMS.Controllers
{
    public class EmployeesController : Controller
    {
        private Entities db = new Entities();
        private OverTimeClassLibrary.OverTime OvertimeObj = new OverTimeClassLibrary.OverTime();

         public async Task<ActionResult> GetFaceapi()
        {
             var q = db.Employees.Select(emp => new { emp.EmployeeID, emp.Photo });
            foreach (var item in q)
            {
                if (item.Photo != null)
                {

                    await class1.MakeAnalysisRequest(item.Photo, item.EmployeeID);

                }
            }

            return Json("", JsonRequestBehavior.AllowGet);

        }

        public async Task<ActionResult> GetFaceapiByEmployeeID(string EmployeeID)
        {
            //var q = db.Employees.Select(emp => new { emp.EmployeeID, emp.Photo });
            var Photo = db.Employees.Where(emp =>  emp.EmployeeID== EmployeeID).FirstOrDefault().Photo;


           await class1.MakeAnalysisRequest(Photo, EmployeeID);





            return RedirectToAction("Index");
        }

        //public async Task<ActionResult> GetFaceapiVerify(string id)
        //{

        //    if (await class1.MakeRequest(id))
        //    {
        //        return Json(new {msg = "同一人"}, JsonRequestBehavior.AllowGet);

        //    }
        //    else
        //    {
        //        return Json(new { msg="XX" }, JsonRequestBehavior.AllowGet);
        //    }



        //}
        [HttpPost]
        public async Task<ActionResult> LoginFace(string imageData)
        {
            byte[] data = Convert.FromBase64String(imageData);
            var q = db.Employees.Select(emp => new { emp.EmployeeID, emp.EmployeeName, emp.FaceID});
            foreach (var item in q)
            {
                if (item.FaceID != null)
                {
                  
                    if (await class1.MakeRequest(item.FaceID, await class1.MakeAnalysisRequest(data, "")))
                    {
                        //return Json(new { EmployeeID = item.EmployeeID, }, JsonRequestBehavior.AllowGet);
                        //Session["UserFullName"] = db.Employees.Find(item.EmployeeID).EmployeeName;
                        //Session["UserName"] = item.EmployeeID;
                        return RedirectToAction("Index", "Home");
                    }
                   

                }
            }

                return Json(new { msg = "XX" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LineIdBindView()
        {
            return View();
        }
        public ActionResult sView()
        {
            return View();
        }
        [HttpPost]
        public JsonResult LineIdBindView(LineIdBindViewModel data)
        {
            string result;
            var query = db.Employees.Find(data.EmployeeID);
            if (query.LineID == null)
            {
                query.LineID = data.LineID;
                db.Entry(query).State = EntityState.Modified;
                db.SaveChanges();
                result = "成功綁定";
            }
            else
            {
                result = "綁定失敗";
            }
            return  Json(new { msg = result }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetDailyStatistics()
        {
            DateTime dateTime = DateTime.Parse("2019/09/24");

            DateTime dateTime3 = DateTime.Parse("2019/09/24");
            DateTime dateTime2 = dateTime.AddDays(1);



            var query = (from e in db.Employees
                         join a in db.Attendances.Where(w => w.Date == dateTime) on e.EmployeeID equals a.EmployeeID into ae
                         from a2 in ae.DefaultIfEmpty()
                         select new
                         {
                             e.EmployeeID,
                             e.DepartmentID,
                             e.EmployeeName,
                             StartTime = a2 == null ? null : a2.OnDuty,
                             EndTime = a2 == null ? null : a2.OffDuty,
                         });
            var query2 = (from e in query
                          join Le in db.LeaveRequests.Where(w => w.StartTime <= dateTime2 && w.EndTime >= dateTime)
                          on e.EmployeeID equals Le.EmployeeID into Le2
                          from a3 in Le2.DefaultIfEmpty()
                          select new /*DailyStatisticsViewModel1*/
                          {
                              e.EmployeeID,
                              e.DepartmentID,
                              EmployeeName = e.EmployeeName,
                              StartTime = e.StartTime,
                              EndTime = e.EndTime,
                              LeaveType = a3 == null ? null : a3.LeaveType
                          });



            var q = query2.GroupBy(De => De.DepartmentID).Select(Emp => new Daily {部門= Emp.Key,人數 = Emp.Count(), 請假 = Emp.Where(g => g.LeaveType != null).Count(), 未到 = Emp.Where(g => g.StartTime == null && g.LeaveType == null && g.EndTime == null).Count() });


            return Json(q,JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDailyStatisticsDataByDepartment(int? id)
        {
            DateTime dateTime = DateTime.Parse("2019/09/24");

            DateTime dateTime3 = DateTime.Parse("2019/09/24");
            DateTime dateTime2 = dateTime.AddDays(1);



            var query = (from e in db.Employees
                         join a in db.Attendances.Where(w => w.Date == dateTime) on e.EmployeeID equals a.EmployeeID into ae
                         from a2 in ae.DefaultIfEmpty()
                         where e.DepartmentID == id
                         select new
                         {
                             e.EmployeeID,
                             e.DepartmentID,
                             e.EmployeeName,
                             StartTime = a2 == null ? null : a2.OnDuty,
                             EndTime = a2 == null ? null : a2.OffDuty,
                         });
            var query2 = (from e in query
                          join Le in db.LeaveRequests.Where(w => w.StartTime <= dateTime2 && w.EndTime >= dateTime)
                          on e.EmployeeID equals Le.EmployeeID into Le2
                          from a3 in Le2.DefaultIfEmpty()
                          select new /*DailyStatisticsViewModel1*/
                          {
                              e.EmployeeID,
                              e.DepartmentID,
                              EmployeeName = e.EmployeeName,
                              StartTime = e.StartTime,
                              EndTime = e.EndTime,
                              LeaveType = a3 == null ? null : a3.LeaveType
                          });



            var q = query2.GroupBy(De => De.DepartmentID).Select(Emp => new Daily { 部門 = Emp.Key, 人數 = Emp.Count(), 請假 = Emp.Where(g => g.LeaveType != null).Count(), 未到 = Emp.Where(g => g.StartTime == null && g.LeaveType == null && g.EndTime == null).Count() });


            return Json(q, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DailyStatistics()
        {

            return PartialView("_DailyStatistics");

            //return View();
        }
        public ActionResult GetDailyStatisticsByDepartment(int? id)
        {
            DateTime dateTime = DateTime.Parse("2019/09/24");

            DateTime dateTime3 = DateTime.Parse("2019/09/24");
            DateTime dateTime2 = dateTime.AddDays(1);

            //var query = (from e in db.Employees
            //             join a in db.Attendances on e.EmployeeID equals a.EmployeeID into ae
            //             from a2 in ae.DefaultIfEmpty()
            //             join Le in db.LeaveRequests on a2.EmployeeID equals Le.EmployeeID into Le2                                           
            //             from a3 in Le2.DefaultIfEmpty()
            //             where a2.Date== dateTime
            //             select new DailyStatisticsViewModel1
            //             {
            //                 EmployeeName = e.EmployeeName,                             
            //                 StartTime = a2 == null ? null : a2.OnDuty,
            //                 EndTime =a2== null ? null : a2.OffDuty,
            //                 LeaveType = a3 == null ? null : a3.LeaveType,

            //             });
            //var query2 = db.LeaveRequests.Where(w => w.StartTime <= dateTime2 && w.EndTime >= dateTime);

            var query = (from e in db.Employees
                         join a in db.Attendances.Where(w => w.Date== dateTime) on e.EmployeeID equals a.EmployeeID into ae
                         from a2 in ae.DefaultIfEmpty()
                         where e.DepartmentID== id
                         select new
                         {
                             e.EmployeeID,
                             //e.DepartmentID,
                             e.EmployeeName,
                             StartTime = a2 == null ? null : a2.OnDuty,
                             EndTime = a2 == null ? null : a2.OffDuty,                             
                         });
            var query2 = (from e in query
                          join Le in db.LeaveRequests.Where(w => w.StartTime <= dateTime2 && w.EndTime >= dateTime) 
                          on e.EmployeeID equals Le.EmployeeID into Le2
                          from a3 in Le2.DefaultIfEmpty()
                          select new DailyStatisticsViewModel1
                          {
                              //e.EmployeeID,
                              //e.DepartmentID,
                              EmployeeName = e.EmployeeName,
                              StartTime = e.StartTime,
                              EndTime = e.EndTime,
                              LeaveType = a3 == null ? null : a3.LeaveType
                          });



  //var q = query2.GroupBy(De => De.DepartmentID).Select(Emp => new { 人數 = Emp.Count(), 請假= Emp.Where(g=>g.LeaveType!=null).Count(),未到= Emp.Where(g => g.StartTime==null && g.LeaveType == null && g.EndTime == null).Count() });
  //          //var q = Context.Employees.Select(Emp => new { 部門 = Emp., 主管 = Emp.Key.Manager, 人數 = Emp.Count() });


            //return PartialView("_DailyStatistics", query);
            return PartialView("_DailyStatisticsPartial", query2);


            //return View();
        }
        [HttpGet]
        public ActionResult SerchOverTime()
        {
            var query = (from ot in db.OverTimeRequest.AsEnumerable()
                         join emp in db.Employees.AsEnumerable() on ot.EmployeeID equals emp.EmployeeID
                         join rev in db.ReviewStatus.AsEnumerable() on ot.ReviewStatusID equals rev.ReviewStatusID
                         join date in db.WorkingDaySchedule.AsEnumerable() on ot.StartTime.Date equals date.Date
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

            Entities dc = new Entities();
            ViewBag.Employees = new SelectList(dc.Employees, "EmployeeID", "EmployeeName");
            ViewBag.Department = new SelectList(dc.Departments, "DepartmentID", "DepartmentName");


            return PartialView("_SerchOverTime", query);

            //return View();
        }
        [HttpPost]
        public ActionResult SerchOverTime(DateTime time1, DateTime time2, int DepartmentID, string EmployeeID)
        {
            IEnumerable<OverTimeViewModel> query;
            if (EmployeeID == "")
            {
              query = (from ot in db.OverTimeRequest.AsEnumerable().Where(q => q.RequestTime >= time1 && q.RequestTime <= time2)
                             join emp in db.Employees.AsEnumerable().Where(emp => emp.DepartmentID == DepartmentID) on ot.EmployeeID equals emp.EmployeeID
                             join rev in db.ReviewStatus.AsEnumerable() on ot.ReviewStatusID equals rev.ReviewStatusID
                             join date in db.WorkingDaySchedule.AsEnumerable() on ot.StartTime.Date equals date.Date
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
            }
            else
            {
                query = (from ot in db.OverTimeRequest.AsEnumerable().Where(q => q.RequestTime >= time1 && q.RequestTime <= time2)
                         join emp in db.Employees.AsEnumerable().Where(emp => emp.EmployeeID == EmployeeID) on ot.EmployeeID equals emp.EmployeeID
                         join rev in db.ReviewStatus.AsEnumerable() on ot.ReviewStatusID equals rev.ReviewStatusID
                         join date in db.WorkingDaySchedule.AsEnumerable() on ot.StartTime.Date equals date.Date
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
             
            }



            return PartialView("_SerchOverTimePartial", query);

            //return View();
        }
        [HttpGet]
        public ActionResult SerchAttendances()
        {
            var query = (from ot in db.Attendances.AsEnumerable()
                         join emp in db.Employees.AsEnumerable() on ot.EmployeeID equals emp.EmployeeID
                         select new SerchAttendancesViewModel
                         {
                             EmployeeName = emp.EmployeeName,
                             Date = ot.Date.ToString("yyyy/MM/dd"),
                             StartTime = ot.OnDuty,
                             EndTime = ot.OffDuty,
                         });
            //var query = (from e in db.Employees.AsEnumerable()
            //             join a in db.Attendances.AsEnumerable() on e.EmployeeID equals a.EmployeeID into g
            //             from a in g.DefaultIfEmpty()
            //             select new SerchAttendancesViewModel
            //             {
            //                 EmployeeName = e.EmployeeName,
            //                 Date = a == null ? "沒打卡" : a.Date.ToString("yyyy/MM/dd"),
            //                 StartTime = a == null ? null : a.OnDuty,
            //                 EndTime = a == null ? null : a.OffDuty,
            //             });
            Entities dc = new Entities();
            ViewBag.Employees = new SelectList(dc.Employees, "EmployeeID", "EmployeeName");
            ViewBag.Department = new SelectList(dc.Departments, "DepartmentID", "DepartmentName");
            return PartialView("_SerchAttendances", query);
    

            //return View();
        }
        [HttpPost]
        public ActionResult SerchAttendances(DateTime time1, DateTime time2,int DepartmentID,string EmployeeID)
        {
            IEnumerable<SerchAttendancesViewModel> query;
            if (EmployeeID=="")
            {
                query = (from ot in db.Attendances.AsEnumerable().Where(q => q.Date >= time1 && q.Date <= time2)
                         join emp in db.Employees.AsEnumerable().Where(emp=>emp.DepartmentID== DepartmentID) on ot.EmployeeID equals emp.EmployeeID
                         select new SerchAttendancesViewModel
                         {
                             EmployeeName = emp.EmployeeName,
                             Date = ot.Date.ToString("yyyy/MM/dd"),
                             StartTime = ot.OnDuty,
                             EndTime = ot.OffDuty,
                         });
            }
            else
            {
                query = (from ot in db.Attendances.AsEnumerable().Where(q => q.Date >= time1 && q.Date <= time2)
                         join emp in db.Employees.AsEnumerable().Where(emp => emp.EmployeeID== EmployeeID) on ot.EmployeeID equals emp.EmployeeID
                         select new SerchAttendancesViewModel
                         {
                             EmployeeName = emp.EmployeeName,
                             Date = ot.Date.ToString("yyyy/MM/dd"),
                             StartTime = ot.OnDuty,
                             EndTime = ot.OffDuty,
                         });
            }


           

            return PartialView("_SerchAttendancesPartial", query);

        }
        [HttpGet]
        public ActionResult SerchLeave()
        {
            var query = (from ot in db.LeaveRequests.AsEnumerable()
                                   join emp in db.Employees.AsEnumerable() on ot.EmployeeID equals emp.EmployeeID
                                   join rev in db.ReviewStatus.AsEnumerable() on ot.ReviewStatusID equals rev.ReviewStatusID
                                   select new SerchLeaveViewModel
                                   {
                                     LeaveRequestID= ot.LeaveRequestID,
                                       EmployeeName = emp.EmployeeName,
                                       LeaveType = ot.LeaveType,
                                       RequestTime = ot.RequestTime,
                                       StartTime = ot.StartTime,
                                       EndTime = ot.EndTime,
                                       Review = rev.ReviewStatus1 
                            
                                   });

            Entities dc = new Entities();
            ViewBag.Employees = new SelectList(dc.Employees, "EmployeeID", "EmployeeName");
            ViewBag.Department = new SelectList(dc.Departments, "DepartmentID", "DepartmentName");

            return PartialView("_SerchLeave", query);
            //return View();
        }
        [HttpPost]
        public ActionResult SerchLeave(DateTime time1, DateTime time2, int DepartmentID, string EmployeeID)
        {
            IEnumerable<SerchLeaveViewModel> query;
            if (EmployeeID == "")
            {
                query = (from ot in db.LeaveRequests.AsEnumerable().Where(q => q.RequestTime >= time1 && q.RequestTime <= time2)
                         join emp in db.Employees.AsEnumerable().Where(emp => emp.DepartmentID == DepartmentID) on ot.EmployeeID equals emp.EmployeeID
                         join rev in db.ReviewStatus.AsEnumerable() on ot.ReviewStatusID equals rev.ReviewStatusID
                         select new SerchLeaveViewModel
                         {
                             LeaveRequestID = ot.LeaveRequestID,
                             EmployeeName = emp.EmployeeName,
                             LeaveType = ot.LeaveType,
                             RequestTime = ot.RequestTime,
                             StartTime = ot.StartTime,
                             EndTime = ot.EndTime,
                             Review = rev.ReviewStatus1

                         });

            }
            else
            {
                query = (from ot in db.LeaveRequests.AsEnumerable().Where(q => q.RequestTime >= time1 && q.RequestTime <= time2)
                         join emp in db.Employees.AsEnumerable().Where(emp => emp.EmployeeID == EmployeeID) on ot.EmployeeID equals emp.EmployeeID
                         join rev in db.ReviewStatus.AsEnumerable() on ot.ReviewStatusID equals rev.ReviewStatusID
                         select new SerchLeaveViewModel
                         {
                             LeaveRequestID = ot.LeaveRequestID,
                             EmployeeName = emp.EmployeeName,
                             LeaveType = ot.LeaveType,
                             RequestTime = ot.RequestTime,
                             StartTime = ot.StartTime,
                             EndTime = ot.EndTime,
                             Review = rev.ReviewStatus1

                         });
            }



            return PartialView("_SerchLeaveListPartial", query);


            //return View();
        }


        // GET: Employees
        public ActionResult Index()
        {
            var query = db.Employees.AsEnumerable().Join(db.Departments, e => e.DepartmentID, d => d.DepartmentID, (e, d) => new EmployeesViewModel
            {
                
                EmployeeID = e.EmployeeID,
                EmployeeName = e.EmployeeName,
                DepartmentName = d.DepartmentName,
                JobTitle = e.JobTitle,
                Manager = d.Manager,
                Hireday = e.Hireday.ToString("yyyy/MM/dd"),
                JobStaus = e.JobStaus
            });

            Entities dc = new Entities();
            ViewBag.Employees = new SelectList(dc.Employees, "EmployeeID", "EmployeeName");
            ViewBag.Department = new SelectList(dc.Departments, "DepartmentID", "DepartmentName");
            return PartialView("_Index", query);

            //return View();
        }

     


        public ActionResult GetDdlandListemp(int? id)
        {
            Entities dc = new Entities();

            var query = dc.Employees.Where(emp => emp.DepartmentID == id);       
            ViewBag.Employees = new SelectList(query, "EmployeeID", "EmployeeName");
            if (query != null)
            {
                return PartialView("_GetDdlandListempPartial"/*, new SelectList(query, "EmployeeID", "EmployeeName")*/);
            }
            else
            {
                return HttpNotFound();
            }


        }
       
        public ActionResult UpdateEmployeesByDepartment(int? id)
        {
            Entities dc = new Entities();


                var c = dc.Employees.Where(emp => emp.DepartmentID == id).AsEnumerable().Join(dc.Departments, e => e.DepartmentID, d => d.DepartmentID, (e, d) => new EmployeesViewModel
                {
                    EmployeeID = e.EmployeeID,
                    EmployeeName = e.EmployeeName,
                    DepartmentName = d.DepartmentName,
                    JobTitle = e.JobTitle,
                    Manager = d.Manager,
                    Hireday = e.Hireday.ToString("yyyy/MM/dd"),
                    JobStaus = e.JobStaus
                });


            if (c != null)
            {
                return PartialView("_ListempPartial", c);
            }
            else
            {
                return HttpNotFound();
            }

        }
        public ActionResult UpdateEmployeesByEmployee(string id)
        {
            Entities dc = new Entities();


            var c = dc.Employees.Where(emp => emp.EmployeeID == id).AsEnumerable().Join(dc.Departments, e => e.DepartmentID, d => d.DepartmentID, (e, d) => new EmployeesViewModel
            {
                EmployeeID = e.EmployeeID,
                EmployeeName = e.EmployeeName,
                DepartmentName = d.DepartmentName,
                JobTitle = e.JobTitle,
                Manager = d.Manager,
                Hireday = e.Hireday.ToString("yyyy/MM/dd"),
                JobStaus = e.JobStaus
            });


            if (c != null)
            {
                return PartialView("_ListempPartial", c);
            }
            else
            {
                return HttpNotFound();
            }

        }

       



        // GET: Employees/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees emp = db.Employees.Find(id);

            EmployeesDetailsViewModel employees = new EmployeesDetailsViewModel
            {

                EmployeeID = emp.EmployeeID,
                EmployeeName = emp.EmployeeName,
                IDNumber = emp.IDNumber,
                DeputyPhone = emp.DeputyPhone,
                Deputy = emp.Deputy,
                 Marital=emp.Marital,
                Email = emp.Email,
                Birthday = emp.Birthday,
                Hireday = emp.Hireday,
                Address = emp.Address,

                JobTitle = emp.JobTitle,
                EnglishName = emp.EnglishName,
                
                Notes = emp.Notes,
                Education = emp.Education
                   //, Photo = null
                   //, LineID = ""
                   //, Leaveday = null
                   ,
                Phone = emp.Phone,
                JobStaus = emp.JobStaus
                , Photo=emp.Photo
            };
            if (emp.gender==true)
            {
                employees.gender = "男生";
            }
            else
            {
                employees.gender = "女生";
            }
            employees.DepartmentName = db.Departments.Where(e => e.DepartmentID == emp.DepartmentID).First().DepartmentName;
            employees.Manager = db.Departments.Where(e => e.DepartmentID == emp.DepartmentID).First().Manager;


            return PartialView("_DetailsPartial", employees);

            if (employees == null)
            {
                return HttpNotFound();
            }
            return View(employees);
        }

        public class Gender
        {
            public String text { get; set; }
            public bool value { get; set; }
        }
        // GET: Employees/Create
        public ActionResult Create()
        {
            var dropdownlist = new List<Gender>
            {
                new Gender{ text="男生",value=true},
                new Gender{ text="女生",value=false}

            };
            ViewBag.gender = new SelectList(dropdownlist, "value", "text");
            return PartialView("_CreatePartial");
        }

        // POST: Employees/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,Password,EmployeeName,IDNumber,DeputyPhone,Deputy,Marital,Email,Birthday,Leaveday,Hireday,Address,DepartmentID,PositionID,Phone,Photo,JobStaus,JobTitle,EnglishName,gender,Notes,LineID,Education,DepartmentName,empFile")] EmployeesCreateViewModel emp)
        {
            if (ModelState.IsValid)
            {

                Employees employees = new Employees
                {

                    EmployeeID = emp.EmployeeID,
                    EmployeeName = emp.EmployeeName,
                    IDNumber = emp.IDNumber,
                    DeputyPhone = emp.DeputyPhone,
                    Deputy = emp.Deputy,
                    //Marital = emp.Manager,
                    Email = emp.Email,
                    Birthday = emp.Birthday,
                    Hireday = emp.Hireday,
                    Address = emp.Address,

                    JobTitle = emp.JobTitle,
                    EnglishName = emp.EnglishName,
                    gender = emp.gender,
                    Notes = emp.Notes,
                    Education = emp.Education
                    //, Photo = null
                    //, LineID = ""
                    //, Leaveday = null
                    ,
                    Phone = emp.Phone

                };
                if (emp.Photo!=null && Request.Files["Photo"].ContentLength != 0)
                {
                    byte[] data = null;
                    using (BinaryReader br = new BinaryReader(
                        Request.Files["Photo"].InputStream))
                    {
                        data = br.ReadBytes(Request.Files["Photo"].ContentLength);
                    }
                    employees.Photo = data;
                }




                employees.DepartmentID = db.Departments.Where(e => e.DepartmentName == emp.DepartmentName).First().DepartmentID;
                employees.JobStaus = "在職";

                db.Employees.Add(employees);
                db.SaveChanges();
                return RedirectToAction("Register", "Account",new { User= emp.EmployeeID , Password=emp.Password, ConfirmPassword=emp.Password});
              
                //return PartialView("_emplistPartial", query);
                //return Content("<script>alert('測試文字');</script>");
            }
            //throw new InvalidOperationException("Something went wrong");
            //return RedirectToAction("Create");
            throw new Exception("XXXXX");
            //else
            //{


            //}
            


        }
        // GET: Employees/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees emp = db.Employees.Find(id);

            EmployeesDetailsViewModel employees = new EmployeesDetailsViewModel
            {

                EmployeeID = emp.EmployeeID,
                EmployeeName = emp.EmployeeName,
                IDNumber = emp.IDNumber,
                DeputyPhone = emp.DeputyPhone,
                Deputy = emp.Deputy,
                Marital = emp.Marital,
                Email = emp.Email,
                Birthday = emp.Birthday,
                Hireday = emp.Hireday,
                Address = emp.Address,

                JobTitle = emp.JobTitle,
                EnglishName = emp.EnglishName,

                Notes = emp.Notes,
                Education = emp.Education
                   //, Photo = null
                   //, LineID = ""
                   //, Leaveday = null
                   ,
                Phone = emp.Phone,
                JobStaus = emp.JobStaus
                ,
                Photo = emp.Photo
            };
            if (emp.gender == true)
            {
                employees.gender = "男生";
            }
            else
            {
                employees.gender = "女生";
            }
            employees.DepartmentName = db.Departments.Where(e => e.DepartmentID == emp.DepartmentID).First().DepartmentName;
            employees.Manager = db.Departments.Where(e => e.DepartmentID == emp.DepartmentID).First().Manager;


            return PartialView("_EditPartial", employees);
       

            if (employees == null)
            {
                return HttpNotFound();
            }
            return View(employees);
        }

        // POST: Employees/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,EmployeeName,IDNumber,DeputyPhone,Deputy,Marital,Email,Birthday,Leaveday,Hireday,Address,DepartmentID,PositionID,Phone,Photo,JobStaus,JobTitle,EnglishName,gender,Notes,LineID,Education,DepartmentName,empFile")] EmployeesEditViewModel emp)
        {
            if (ModelState.IsValid)
            {
                Employees employees = new Employees
                {

                    EmployeeID = emp.EmployeeID,
                    EmployeeName = emp.EmployeeName,
                    IDNumber = emp.IDNumber,
                    DeputyPhone = emp.DeputyPhone,
                    Deputy = emp.Deputy,
                    Marital = emp.Manager,
                    Email = emp.Email,
                    Birthday = emp.Birthday,
                    Hireday = emp.Hireday,
                    Address = emp.Address,

                    JobTitle = emp.JobTitle,
                    EnglishName = emp.EnglishName,
                   
                    Notes = emp.Notes,
                    Education = emp.Education
                    //, Photo = null
                    //, LineID = ""
                    //, Leaveday = null
                    ,
                    Phone = emp.Phone

                };
                if (emp.Photo != null && Request.Files["Photo"].ContentLength != 0)
                {
                    byte[] data = null;
                    using (BinaryReader br = new BinaryReader(
                        Request.Files["Photo"].InputStream))
                    {
                        data = br.ReadBytes(Request.Files["Photo"].ContentLength);
                    }
                    employees.Photo = data;
                }
                else
                {
                    employees.Photo = employees.Photo;
                }
                        

                if (emp.gender=="男生")
                {
                    employees.gender = true;
                }
                else
                {
                    employees.gender = false;
                }
                

                employees.DepartmentID = db.Departments.Where(e => e.DepartmentName == emp.DepartmentName).First().DepartmentID;
                employees.JobStaus = "在職";

                db.Entry(employees).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("GetFaceapiByEmployeeID", "Employees", new { EmployeeID = emp.EmployeeID });
             

            }
            return RedirectToAction("Index");
            return View(emp);

        }

        // GET: Employees/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return HttpNotFound();
            }
            return View(employees);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Employees employees = db.Employees.Find(id);
            db.Employees.Remove(employees);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> MonthlyStatistics()
        {
            var OT = (from ot in db.OverTimeRequest.AsEnumerable().Where(ot => ot.ReviewStatusID == 2 && ot.RequestTime.Month ==12)
                      join date in db.WorkingDaySchedule.AsEnumerable() on ot.StartTime.Date equals date.Date
                      select new
                      {
                          EmployeeID = ot.EmployeeID,
                          SummaryTime = OvertimeObj.Summary(ot.StartTime, ot.EndTime, date.WorkingDay,false),
                          //ot.OverTimePay
                      }).ToList();

            var OT2 = OT.GroupBy(ot => ot.EmployeeID).Select(ot => new OverTimeHoursSumModel
            {
                EmployeeID = ot.Key,
                Q = ot.Sum(o => double.Parse(o.SummaryTime))
            }).ToList();

            var query = db.Employees.AsEnumerable().Join(db.Departments, e => e.DepartmentID, d => d.DepartmentID, (e, d) => new
            {
                EmployeeID = e.EmployeeID,
                EmployeeName = e.EmployeeName,
                DepartmentName = d.DepartmentName,
            }).ToList();
            var query2 = (from e in query
                          join d in OT2 on e.EmployeeID equals d.EmployeeID into ae
                          from d in ae.DefaultIfEmpty()
                              //where e.DepartmentID == id
                          select new MonthlyStatisticsViewModel
                          {
                              EmployeeID = e.EmployeeID,
                              EmployeeName = e.EmployeeName,
                              DepartmentName = e.DepartmentName,
                              WorkingDay = monthlyhours(DateTime.Now),
                              AttendanceDay = AttendanceDays(e.EmployeeID, DateTime.Now),
                              WorkingDayHours = 0,
                              AttendanceDayHours = 0,
                              AttendanceRate = 0.00,
                              //LeaveDayHours = AllLeaveSun(e.EmployeeID),

                              OverTimeHours = d == null ? null : d.Q,
                          }).ToList();





            foreach (var item in query2)
            {
                item.WorkingDayHours = item.WorkingDay * 8;
                item.AttendanceDayHours = item.AttendanceDay * 8;
                item.AttendanceRate = (item.AttendanceDay * 800) / (item.WorkingDay * 8);
                item.LeaveDayHours = await AllLeaveSun(item.EmployeeID, DateTime.Now);

            }

            return PartialView("_MonthlyStatistics", query2);
        }

        [HttpGet]
        public async Task<ActionResult> GetMonthStatisticsByMonth(int id)
        {
            var OT = (from ot in db.OverTimeRequest.AsEnumerable().Where(ot => ot.ReviewStatusID == 2 && ot.RequestTime.Month==id)
                      join date in db.WorkingDaySchedule.AsEnumerable() on ot.StartTime.Date equals date.Date
                      select new
                      {
                          EmployeeID = ot.EmployeeID,
                          SummaryTime = OvertimeObj.Summary(ot.StartTime, ot.EndTime, date.WorkingDay, false),
                          //ot.OverTimePay
                      }).ToList();

            var OT2 = OT.GroupBy(ot => ot.EmployeeID).Select(ot => new OverTimeHoursSumModel
            {
                EmployeeID = ot.Key,
                Q = ot.Sum(o => double.Parse(o.SummaryTime))
            }).ToList();

            var query = db.Employees.AsEnumerable().Join(db.Departments, e => e.DepartmentID, d => d.DepartmentID, (e, d) => new
            {
                EmployeeID = e.EmployeeID,
                EmployeeName = e.EmployeeName,
                DepartmentName = d.DepartmentName,
            }).ToList();
            var query2 = (from e in query
                          join d in OT2 on e.EmployeeID equals d.EmployeeID into ae
                          from d in ae.DefaultIfEmpty()
                              //where e.DepartmentID == id
                          select new MonthlyStatisticsViewModel
                          {
                              EmployeeID = e.EmployeeID,
                              EmployeeName = e.EmployeeName,
                              DepartmentName = e.DepartmentName,
                              WorkingDay = monthlyhours(new DateTime(2019, id, 01)),
                              AttendanceDay = AttendanceDays(e.EmployeeID, new DateTime(2019, id, 01)),
                              WorkingDayHours = 0,
                              AttendanceDayHours = 0,
                              AttendanceRate = 0.00,
                              //LeaveDayHours = AllLeaveSun(e.EmployeeID),

                              OverTimeHours = d == null ? null : d.Q,
                          }).ToList();





            foreach (var item in query2)
            {
                item.WorkingDayHours = item.WorkingDay * 8;
                item.AttendanceDayHours = item.AttendanceDay * 8;
                item.AttendanceRate = (item.AttendanceDay * 800) / (item.WorkingDay * 8);
                item.LeaveDayHours = await AllLeaveSun(item.EmployeeID, new DateTime(2019, id, 01)); 
            }

            return PartialView("_MonthlyStatisticsListPartial", query2);
        }


        public int monthlyhours(DateTime month)////統計本月應到天數
        {
            string EmployeeID = Convert.ToString(Session["UserName"]);
            DateTime FirstDay = month.AddDays(-month.Day + 1);//////取每月第一天
            DateTime LastDay = month.AddMonths(1).AddDays(-month.AddMonths(1).Day);//////取每月最後一天
            var monthly = db.WorkingDaySchedule.Where(p => p.Date >= FirstDay && p.Date <= LastDay && p.WorkingDay == "工作日");
            var CountLeaveDays = db.LeaveRequests.Where(p => p.EmployeeID == EmployeeID && p.StartTime >= FirstDay && p.EndTime <= LastDay && p.ReviewStatusID == 2);
            int sum = monthly.Count();
            return sum;
        }

        public int AttendanceDays(string EmployeeID, DateTime month)////統計目前出勤天數
        {

            DateTime FirstDay = month.AddDays(-month.Day + 1);//////取每月第一天
            DateTime LastDay = month.AddMonths(1).AddDays(-month.AddMonths(1).Day);//////取每月最後一天
            var monthly = db.Attendances.Where(p => p.EmployeeID == EmployeeID && p.Date >= FirstDay && p.Date <= LastDay && p.station == null);
            int sum = monthly.Count();
            return sum;
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
            byte[] content = db.Employees.Find(id).Photo;
            return File(content, "image/jpeg");
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
        public async Task<int>  AllLeaveSun(string User, DateTime month)
        {
            
            //LeaveRequests L = new LeaveRequests();
            //var dbLeave = db.LeaveRequests;
            //var dbseday = db.LeaveRequests.AsEnumerable().ToList().Where(n => n.EmployeeID == User && ((n.StartTime.Month == DateTime.Now.Month) || (n.EndTime.Month == DateTime.Now.Month))).Sum(n => AllMonth(n.StartTime, n.EndTime));

            ////var All = dbLeave.Where(n => n.EmployeeID == User&&DateTime.Now.Year==n.StartTime);
            int sum = 0;
            var dbseday = db.LeaveRequests.AsEnumerable().Where(n => ((n.StartTime.Month == month.Month) || (n.EndTime.Month == month.Month)) && n.EmployeeID == User ).ToList();
            foreach (var n in dbseday)
            {
                sum+= await AllMonth(n.StartTime, n.EndTime, month);
            }

            return sum;
        }
        #endregion

        #region 算當月請假時數總和
        public async Task<int> AllMonth(DateTime start, DateTime end, DateTime month)
        {

            if ((start.Month == month.Month) && (end.Month == month.Month))  //開始時間和結束時間的月份都等於當月份
            {
                var sum = await GetLeaveDay(start, end);
                return sum;

            }
            else if ((start.Month <month.Month) && (end.Month ==month.Month))//開始時間小於當月份，結束時間等於當月
            {
                var FirstDay =month.AddDays(-month.Day + 1);
                var yesday = DateTime.Parse($"{FirstDay.Year}-{FirstDay.Month}-{FirstDay.Day} 08:30:00");
                var sum = await GetLeaveDay(yesday, end);
                return sum;

            }
            else if ((start.Month ==month.Month) && (end.Month >month.Month))//開始時間等於當月，結束時間大於當月份
            {
                var LastDay =month.AddMonths(1).AddDays(-month.AddMonths(1).Day);
                var yesday = DateTime.Parse($"{LastDay.Year}-{LastDay.Month}-{LastDay.Day} 17:30:00");
                var sum = await GetLeaveDay(end, yesday);

                return sum;
            }
            else//開始時間和結束時間都不等於當月
            {
                return 0;
            }




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
        public async Task<int> SumLeaveAsync()
        {
            string User = Convert.ToString(Session["UserName"]);

            int sum = 0;
            var aa = db.LeaveRequests.AsEnumerable().Where(n => n.EmployeeID == User && n.LeaveType == "補休假").ToList();
  
            foreach (var n in aa)
            {
                sum += await GetLeaveDay(n.StartTime, n.EndTime);
            }
            //foreach(var item in aa)
            //{
            //    sumleave+=GetLeaveDay(item.StartTime, item.EndTime);

            //}
            return sum;
        }
        #endregion


        #region 判斷是否為工作天  套用在GetLeaveDay中
        /// 判断是否是工作日| true：工作 | flase：休息
        /// </summary>
        /// <param name="date">時間</param>
        /// <returns>true：工作 | flase:休息</returns>
        public async Task< bool> IsWorkDay(DateTime date)
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

        public async Task<int>GetLeaveDay(DateTime dtStart, DateTime dtEnd)
        {

            DateTime dtFirstDayGoToWork = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 08, 00, 0);//請假第一天的上班時間
            DateTime dtFirstDayGoOffWork = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 17, 00, 0);//請假第一天的下班時間

            DateTime dtLastDayGoToWork = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 08, 00, 0);//請假最後一天的上班時間
            DateTime dtLastDayGoOffWork = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 17, 00, 0);//請假最後一天的下班時間

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
                    if (await IsWorkDay(dtStart))
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
                    if (await IsWorkDay(dtEnd))
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
                bool a = await IsWorkDay(tempdt);
                if (await IsWorkDay(tempdt))
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









    }


}
