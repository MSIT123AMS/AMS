using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Controllers
{
    public class AttendancesSearchController : Controller
    {
        // GET: AttendancesSearch
        public ActionResult Index()
        {
            return View();
        }

        // GET: AttendancesSearch/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AttendancesSearch/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AttendancesSearch/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: AttendancesSearch/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AttendancesSearch/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: AttendancesSearch/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AttendancesSearch/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
