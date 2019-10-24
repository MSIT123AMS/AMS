using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AMS.Models;

namespace AMS.Controllers
{
    public class EmployeesController : Controller
    {
        private Entities db = new Entities();

        // GET: Employees
        public ActionResult Index()
        {
            //var query = db.Employees.AsEnumerable().Join(db.Departments,e=>e.DepartmentID,d=>d.DepartmentID, (e, d) =>new EmployeesViewModel
            //{
            //    EmployeeID=e.EmployeeID,
            //    EmployeeName = e.EmployeeName,
            //    DepartmentName= d.DepartmentName,
            //    JobTitle = e.JobTitle,
            //    Manager = d.Manager,
            //    Hireday = e.Hireday.ToString("yyyy/MM/dd"),
            //    JobStaus = e.JobStaus
            //});


            //return View(query);
            Entities dc = new Entities();
            ViewBag.Employees = new SelectList(dc.Employees, "EmployeeID", "EmployeeName");
            ViewBag.Department = new SelectList(dc.Departments, "DepartmentID", "DepartmentName");
            return View();
        }

        public ActionResult Listemp(string id,int id2)
        {
            Entities dc = new Entities();

            IEnumerable<EmployeesViewModel> c = null;
            if (id != "")
            {
                c = dc.Employees.Where(emp => emp.EmployeeID == id && emp.DepartmentID == id2).AsEnumerable().Join(db.Departments, e => e.DepartmentID, d => d.DepartmentID, (e, d) => new EmployeesViewModel
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
                 c = dc.Employees.Where(emp => emp.DepartmentID == id2).AsEnumerable().Join(db.Departments,e => e.DepartmentID,d => d.DepartmentID, (e, d) => new EmployeesViewModel
                 {
                     EmployeeID = e.EmployeeID,
                     EmployeeName = e.EmployeeName,
                     DepartmentName = d.DepartmentName,
                     JobTitle = e.JobTitle,
                     Manager = d.Manager,
                     Hireday = e.Hireday.ToString("yyyy/MM/dd"),
                     JobStaus = e.JobStaus
                 }); ;
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
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return HttpNotFound();
            }
            return View(employees);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EmployeeID,EmployeeName,IDNumber,DeputyPhone,Deputy,Marital,Email,Birthday,Leaveday,Hireday,Address,DepartmentID,PositionID,Phone,Photo,JobStaus,JobTitle,EnglishName,gender,Notes,LineID,Education")] Employees employees)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employees);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(employees);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(string id)
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

        // POST: Employees/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,EmployeeName,IDNumber,DeputyPhone,Deputy,Marital,Email,Birthday,Leaveday,Hireday,Address,DepartmentID,PositionID,Phone,Photo,JobStaus,JobTitle,EnglishName,gender,Notes,LineID,Education")] Employees employees)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employees).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employees);
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
    }
}
