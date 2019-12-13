using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using AMS.Models;
using WebApplication5.Controllers;

namespace AMS.Controllers
{
    public class ClockInApplyController : Controller
    {
        private Entities db = new Entities();          
        // GET: ClockInApply       
        public ActionResult ClockInApplyView()
        {
            string EmployeeID = Convert.ToString(Session["UserName"]);////////要再更改id參數////////////////
            var query = db.ClockInApply.Join(
                db.ReviewStatus, ClockIn => ClockIn.ReviewStatusID,
                Review => Review.ReviewStatusID,
                (ClockIn, Review) => new
                {
                    ClockIn.EmployeeID,
                    ClockIn.OnDuty,
                    ClockIn.OffDuty,
                    ClockIn.RequestDate,
                    Review.ReviewStatus1
                }).Join(db.Employees, p => p.EmployeeID, z => z.EmployeeID, (p, z) => new ClockInApplyViewModel
                {
                    EmployeeID = p.EmployeeID,
                    EmployeeName=z.EmployeeName,
                    OnDuty=p.OnDuty,
                    OffDuty=p.OffDuty,
                    RequestDate=p.RequestDate,
                    ReviewStatus1=p.ReviewStatus1
                }).Where(x=>x.EmployeeID== EmployeeID);
            
            return PartialView("_ClockInApplyView", query);
        }
      
        // GET: ClockInApply/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClockInApply clockInApply = db.ClockInApply.Find(id);
            if (clockInApply == null)
            {
                return HttpNotFound();
            }
            return View(clockInApply);
        }

        // GET: ClockInApply/Create
       
            public ActionResult LineClockinApply()
        {
            return View();           
        }
        public ActionResult ClockInApply()
        {


            return PartialView("_ClockInApply");
        }

        // POST: ClockInApply/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ClockInApply([Bind(Include = "EmployeeID,OnDuty,OffDuty,ReviewStatusID,RequestDate,ReviewTime")] ClockInApply clockInApply)
        {     
        //string EmployeeID;
          string EmployeeID = Convert.ToString(Session["UserName"]);          
           //AdminUserId= lineBotWebHook.ReceivedMessage.events.FirstOrDefault().source.userId;        
            if (string.IsNullOrEmpty(EmployeeID))
            {
                EmployeeID = clockInApply.EmployeeID;
                var GetLineId = db.Employees.Where(p => p.LineID == EmployeeID).FirstOrDefault();
                EmployeeID = GetLineId.EmployeeID;
                clockInApply.EmployeeID = EmployeeID;
                clockInApply.ReviewStatusID = 1;
                db.ClockInApply.Add(clockInApply);
                try
                {
                    db.SaveChanges();
                    return Json(new { msg = "申請成功" }, JsonRequestBehavior.AllowGet);
                    //return PartialView("_ClockInApplyView", clockInApply);
                }
                catch
                {

                    //new AjaxOptions
                    //{
                    //    OnSuccess = "onSuccess"
                    //};
                    //TempData["message"] = $"已經有{clockInApply.RequestDate.Value.ToString("yyyy年MM月dd日")}的申請紀錄!";
                    //return PartialView("_ClockInApplyView", clockInApply);
                    return Json(new { msg = $"申請失敗:已經有{clockInApply.RequestDate.Value.ToString("yyyy年MM月dd日")}的申請紀錄!" }, JsonRequestBehavior.AllowGet);
                }

            }
            if (ModelState.IsValid)
            {
                
               clockInApply.EmployeeID= EmployeeID;
               clockInApply.ReviewStatusID = 1;
                db.ClockInApply.Add(clockInApply);
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("ClockInApplyView", "ClockInApply");
                    //return PartialView("_ClockInApplyView", clockInApply);
                }
                catch {

                    //new AjaxOptions
                    //{
                    //    OnSuccess = "onSuccess"
                    //};
                    TempData["message"] = $"已經有{clockInApply.RequestDate.Value.ToString("yyyy年MM月dd日")}的申請紀錄!";
                    //return PartialView("_ClockInApplyView", clockInApply);
                    return RedirectToAction("ClockInApplyView", "ClockInApply");
                }
            }
            return RedirectToAction("ClockInApplyView", "ClockInApply");
            //return PartialView("_ClockInApplyView", clockInApply);
        }

        // GET: ClockInApply/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClockInApply clockInApply = db.ClockInApply.Find(id);
            if (clockInApply == null)
            {
                return HttpNotFound();
            }
            return View(clockInApply);
        }

        // POST: ClockInApply/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,OnDuty,OffDuty,ReviewStatusID,RequestDate,ReviewTime")] ClockInApply clockInApply)
        {
            if (ModelState.IsValid)
            {
               
                db.Entry(clockInApply).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clockInApply);
        }

        // GET: ClockInApply/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClockInApply clockInApply = db.ClockInApply.Find(id);
            if (clockInApply == null)
            {
                return HttpNotFound();
            }
            return View(clockInApply);
        }

        // POST: ClockInApply/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ClockInApply clockInApply = db.ClockInApply.Find(id);
            db.ClockInApply.Remove(clockInApply);
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
