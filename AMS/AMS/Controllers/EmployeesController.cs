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

            //if (!await class1.MakeRequest())
            //{
            //    Debug.WriteLine("XXXX");

            //}


            return Json("",JsonRequestBehavior.AllowGet);
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




            return PartialView("_SerchOverTime", query);

            //return View();
        }
        [HttpPost]
        public ActionResult SerchOverTime(DateTime time1, DateTime time2)
        {
            var query = (from ot in db.OverTimeRequest.AsEnumerable().Where(q => q.RequestTime >= time1 && q.RequestTime <= time2)
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

            return PartialView("_SerchAttendances", query);
    

            //return View();
        }
        [HttpPost]
        public ActionResult SerchAttendances(DateTime time1, DateTime time2)
        {
            var query = (from ot in db.Attendances.AsEnumerable().Where(q => q.Date >= time1 && q.Date <= time2)
                         join emp in db.Employees.AsEnumerable() on ot.EmployeeID equals emp.EmployeeID
                         select new SerchAttendancesViewModel
                         {
                             EmployeeName = emp.EmployeeName,
                             Date = ot.Date.ToString("yyyy/MM/dd"),
                             StartTime = ot.OnDuty,
                             EndTime = ot.OffDuty,
                         });

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
           
            return PartialView("_SerchLeave", query);
            //return View();
        }
        [HttpPost]
        public ActionResult SerchLeave(DateTime time1, DateTime time2)
        {
            var query = (from ot in db.LeaveRequests.AsEnumerable().Where(q => q.RequestTime >= time1 && q.RequestTime <= time2)
                         join emp in db.Employees.AsEnumerable() on ot.EmployeeID equals emp.EmployeeID
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
                return RedirectToAction("Index");

            }
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
        public ActionResult MonthlyStatistics()
        {
            var OT = (from ot in db.OverTimeRequest.AsEnumerable().Where(ot => ot.ReviewStatusID == 2)
                      join date in db.WorkingDaySchedule.AsEnumerable() on ot.StartTime.Date equals date.Date
                      select new
                      {
                          EmployeeID = ot.EmployeeID,
                          RequestTime = ot.RequestTime,
                          SummaryTime = OvertimeObj.Summary(ot.StartTime, ot.EndTime, date.WorkingDay, ot.OverTimePay),
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
                              WorkingDay = monthlyhours(),
                              AttendanceDay = AttendanceDays(e.EmployeeID),
                              WorkingDayHours = 0,
                              AttendanceDayHours = 0,
                              AttendanceRate = 0.00,
                              LeaveDayHours = 0,
                              //AllLeaveSun(e.EmployeeID)
                              OverTimeHours = d == null ? null : d.Q,
                          }).ToList();


            //var query2 = query.Join(OT2, e => e.EmployeeID, d => d.EmployeeID, (e, d) => new MonthlyStatisticsViewModel
            //{
            //    EmployeeID = e.EmployeeID,
            //    EmployeeName = e.EmployeeName,
            //    DepartmentName = e.DepartmentName,
            //    WorkingDay = monthlyhours(),
            //    AttendanceDay = AttendanceDays(e.EmployeeID),
            //    WorkingDayHours = 0,
            //    AttendanceDayHours = 0,
            //    AttendanceRate = 0.00,
            //    LeaveDayHours = 0,
            //    //AllLeaveSun(e.EmployeeID)
            //    OverTimeHours = d.Q,
            //}).ToList();



            foreach (var item in query2)
            {
                item.WorkingDayHours = item.WorkingDay * 8;
                item.AttendanceDayHours = item.AttendanceDay * 8;
                item.AttendanceRate = (item.AttendanceDay * 800) / (item.WorkingDay * 8);
            }

            return PartialView("_MonthlyStatistics", query2);
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

        public int AttendanceDays(string EmployeeID)////統計目前出勤天數
        {

            DateTime FirstDay = DateTime.Now.AddDays(-DateTime.Now.Day + 1);//////取每月第一天
            DateTime LastDay = DateTime.Now.AddMonths(1).AddDays(-DateTime.Now.AddMonths(1).Day);//////取每月最後一天
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
    }


}
