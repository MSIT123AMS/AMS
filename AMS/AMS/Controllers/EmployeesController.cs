﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AMS.Models;

using System.IO;

namespace AMS.Controllers
{
    public class EmployeesController : Controller
    {
        private Entities db = new Entities();
        private OverTimeClassLibrary.OverTime OvertimeObj = new OverTimeClassLibrary.OverTime();



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

 

           
            return View(query);

            //return View();
        }

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




            return View(query);

            //return View();
        }

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
            return View(query);

            //return View();
        }
        [HttpPost]
        public ActionResult SerchLeave(DateTime time1, DateTime time2)
        {
            var query = (from ot in db.LeaveRequests.AsEnumerable()
                         join emp in db.Employees.AsEnumerable() on ot.EmployeeID equals emp.EmployeeID
                         join rev in db.ReviewStatus.AsEnumerable() on ot.ReviewStatusID equals rev.ReviewStatusID
                         select new SerchLeaveViewModel
                         {
                             LeaveRequestID = ot.LeaveRequestID,
                             EmployeeName = emp.EmployeeName,
                             RequestTime = ot.RequestTime,
                             StartTime = ot.StartTime,
                             EndTime = ot.EndTime,
                             Review = rev.ReviewStatus1

                         }).Where(q=>q.RequestTime >= time1 && q.RequestTime<= time2);

            Entities dc = new Entities();
            ViewBag.Employees = new SelectList(dc.Employees, "EmployeeID", "EmployeeName");
            ViewBag.Department = new SelectList(dc.Departments, "DepartmentID", "DepartmentName");
            return View(query);

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
            return View(query);
            
            //return View();
        }

        public ActionResult backIndex()
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
            return PartialView("_emplistPartial", query);

            //return View();
        }



        public ActionResult GetDdlandListemp(int? id)
        {
            Entities dc = new Entities();

            var query = dc.Employees.Where(emp => emp.DepartmentID == id);       
            ViewBag.Employees = new SelectList(query, "EmployeeID", "EmployeeName");
            if (query != null)
            {
                return PartialView("_GetDdlandListempPartial", new SelectList(query, "EmployeeID", "EmployeeName"));
            }
            else
            {
                return HttpNotFound();
            }


        }


        public ActionResult Listemp(string id,int? id2)
        {
            Entities dc = new Entities();

            IEnumerable<EmployeesViewModel> c ;
            if (id == "null" && id2 != null)
            {
                c = dc.Employees.Where(emp => emp.DepartmentID == id2).AsEnumerable().Join(dc.Departments, e => e.DepartmentID, d => d.DepartmentID, (e, d) => new EmployeesViewModel
                {
                    EmployeeID = e.EmployeeID,
                    EmployeeName = e.EmployeeName,
                    DepartmentName = d.DepartmentName,
                    JobTitle = e.JobTitle,
                    Manager = d.Manager,
                    Hireday = e.Hireday.ToString("yyyy/MM/dd"),
                    JobStaus = e.JobStaus
                });
                var query = dc.Employees.Where(emp => emp.DepartmentID == id2);
                ViewBag.Employees = new SelectList(query, "EmployeeID", "EmployeeName");
            }
            else if (id2 == null && id != "null")
            {
                c = dc.Employees.Where(emp => emp.EmployeeID == id).AsEnumerable().Join(dc.Departments, e => e.DepartmentID, d => d.DepartmentID, (e, d) => new EmployeesViewModel
                {
                    EmployeeID = e.EmployeeID,
                    EmployeeName = e.EmployeeName,
                    DepartmentName = d.DepartmentName,
                    JobTitle = e.JobTitle,
                    Manager = d.Manager,
                    Hireday = e.Hireday.ToString("yyyy/MM/dd"),
                    JobStaus = e.JobStaus
                });
            }
            else
            {
                c = dc.Employees.Where(emp => emp.EmployeeID == id && emp.DepartmentID == id2).AsEnumerable().Join(dc.Departments, e => e.DepartmentID, d => d.DepartmentID, (e, d) => new EmployeesViewModel
                {
                    EmployeeID = e.EmployeeID,
                    EmployeeName = e.EmployeeName,
                    DepartmentName = d.DepartmentName,
                    JobTitle = e.JobTitle,
                    Manager = d.Manager,
                    Hireday = e.Hireday.ToString("yyyy/MM/dd"),
                    JobStaus = e.JobStaus
                });
            }


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
   

            return View(employees);

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
        //[ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,EmployeeName,IDNumber,DeputyPhone,Deputy,Marital,Email,Birthday,Leaveday,Hireday,Address,DepartmentID,PositionID,Phone,Photo,JobStaus,JobTitle,EnglishName,gender,Notes,LineID,Education,DepartmentName,empFile")] EmployeesCreateViewModel emp)
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
                    gender = emp.gender,
                    Notes = emp.Notes,
                    Education = emp.Education
                    //, Photo = null
                    //, LineID = ""
                    //, Leaveday = null
                    , Phone=emp.Phone
            
                };
                if (Request.Files["Photo"].ContentLength != 0)
                {
                    byte[] data = null;
                    using (BinaryReader br = new BinaryReader(
                        Request.Files["Photo"].InputStream))
                    {
                        data = br.ReadBytes(Request.Files["Photo"].ContentLength);
                    }
                    employees.Photo= data;
                }




                employees.DepartmentID = db.Departments.Where(e=>e.DepartmentName==emp.DepartmentName).First().DepartmentID;
                employees.JobStaus = "在職";

                db.Employees.Add(employees);
                db.SaveChanges();
                var dropdownlist = new List<Gender>
            {
                new Gender{ text="男生",value=true},
                new Gender{ text="女生",value=false}

            };
                ViewBag.gender = new SelectList(dropdownlist, "value", "text");

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

                return PartialView("_emplistPartial", query);
                //return Content("<script>alert('測試文字');</script>");
            }



            return View(emp);
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


            return View(employees);

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
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartmentName,EmployeeID,EmployeeName,IDNumber,DeputyPhone,Deputy,Marital,Email,Birthday,Leaveday,Hireday,Address,DepartmentID,PositionID,Phone,Photo,JobStaus,JobTitle,EnglishName,gender,Notes,LineID,Education")] EmployeesDetailsViewModel emp)
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
                if (Request.Files["empFile"].ContentLength != 0)
                {
                    byte[] data = null;
                    using (BinaryReader br = new BinaryReader(
                        Request.Files["empFile"].InputStream))
                    {
                        data = br.ReadBytes(Request.Files["empFile"].ContentLength);
                    }
                    employees.Photo = data;
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
