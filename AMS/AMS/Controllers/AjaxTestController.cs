using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Controllers
{
    public class AjaxTestController : Controller
    {
        // GET: AjaxTest
        public ActionResult Index()
        {
           
            return View();
        }
        ///////////定位回傳//////////////////
        //public double x;
        //public ActionResult GetLocation(Position position)
        //{

            
        //    //TempData["jsonData"] = position;
        //    //ViewBag.jsondata = position;
        //    return Json(position, JsonRequestBehavior.AllowGet);
        //    //return RedirectToAction();
        //    // do whatever you want here. I am simply returning the value.
        //    //return Json("You posted: " + position.Lat + " items.", JsonRequestBehavior.AllowGet);
        //}
    }
}