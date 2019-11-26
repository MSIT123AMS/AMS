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

        public JsonResult LoadCalender()
        {
            string User = Convert.ToString(Session["UserName"]);  //從Session抓UserID
            //var query = db.OverTimeRequest.AsEnumerable().Where(ot => ot.EmployeeID == User).Select(ot => new calendar
            //{
            //    title = "加班",
            //    start = CalendarDate(ot.StartTime),
            //    backgroundColor = "lightBlue"
            //});
           
            var query= db.Attendances.AsEnumerable().Where(att => att.EmployeeID == User).Select(att => new calendar
            {
                title = $"上班: {att.OnDuty.Value.ToShortTimeString()}",
                start = CalendarDate(att.OnDuty.Value),
                backgroundColor = "lightBlue"

            });
            var query1 = db.Attendances.AsEnumerable().Where(att => att.EmployeeID == User&&att.OffDuty.HasValue).Select(att => new calendar
            {
                title = $"下班: {att.OffDuty.Value.ToShortTimeString()}",
                start = CalendarDate(att.OffDuty.Value),
                backgroundColor = "LightPink"

            });
            query= query.Concat(query1);



            return Json(query, JsonRequestBehavior.AllowGet);

        }

        public string CalendarDate(DateTime StartTime)
        {
            string getDate = StartTime.Date.ToShortDateString().Replace('/','-');

            return getDate;

        }


    }
}