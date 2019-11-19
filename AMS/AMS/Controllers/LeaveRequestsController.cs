using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using AMS.Metadata;
using AMS.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using SelectPdf;

namespace AMS.Controllers
{
    public class LeaveRequestsController : Controller
    {

        private Entities db = new Entities();

        // GET: LeaveRequests
        public ActionResult Index()
        {
            string User = Convert.ToString(Session["UserName"]);
            return View(db.LeaveRequests);
        }

        // GET: LeaveRequests/Details/5

        public ActionResult LeaveIndexView()
        {
            string User = Convert.ToString(Session["UserName"]);  //從Session抓UserID
            var LeaveTimeRequest = (from lt in db.LeaveRequests.AsEnumerable()
                                    join emp in db.Employees.AsEnumerable() on lt.EmployeeID equals emp.EmployeeID
                                    join rev in db.ReviewStatus.AsEnumerable() on lt.ReviewStatusID equals rev.ReviewStatusID
                                    where lt.EmployeeID == User
                                    select new LeaveIndexViewModel
                                    {
                                        LeaveRequestID = lt.LeaveRequestID,
                                        EmployeeName = emp.EmployeeName,
                                        LeaveType = lt.LeaveType,
                                        RequestTime = lt.RequestTime,
                                        StartTime = lt.StartTime,
                                        EndTime = lt.EndTime,
                                        LeaveReason = lt.LeaveReason,
                                        Review = rev.ReviewStatus1,
                                        ReviewTime = lt.ReviewTime,
                                        Attachment = lt.Attachment
                                    });
            
            return PartialView("LeaveIndexView", LeaveTimeRequest);

        }


        [HttpPost]
        public ActionResult Leave11(string id,string id2, string[] value) //傳入開始時間,結束時間,假別
        {
            string User = Convert.ToString(Session["UserName"]);  //從Session抓UserID
            DateTime startime = Convert.ToDateTime(id);
            DateTime endtime = Convert.ToDateTime(id2);
            string[] LeaveValue = value;

           
            var LeaveTimeRequest = (from lt in db.LeaveRequests.AsEnumerable()
                                    join emp in db.Employees.AsEnumerable() on lt.EmployeeID equals emp.EmployeeID
                                    join rev in db.ReviewStatus.AsEnumerable() on lt.ReviewStatusID equals rev.ReviewStatusID
                                    where lt.StartTime >= startime && lt.EndTime <= endtime && lt.EmployeeID == User&&LeaveValue.Any(x=>x==lt.LeaveType)
                                    select new LeaveIndexViewModel
                                    {
                                        LeaveRequestID = lt.LeaveRequestID,
                                        EmployeeName = emp.EmployeeName,
                                        LeaveType = lt.LeaveType,
                                        RequestTime = lt.RequestTime,
                                        StartTime = lt.StartTime,
                                        EndTime = lt.EndTime,
                                        LeaveReason = lt.LeaveReason,
                                        Review = rev.ReviewStatus1,
                                        ReviewTime = lt.ReviewTime,
                                        Attachment = lt.Attachment
                                    });
           
            if (LeaveTimeRequest != null)
            {
                return PartialView("_LeaveIndexPartialView", LeaveTimeRequest);

            }
            else
            {
                return HttpNotFound();
            }

         
        }

        //轉PDF
        public ActionResult ToPdf()
        {
            // instantiate a html to pdf converter object
            string pdfname = "差假報表.pdf";
            HtmlToPdf converter = new HtmlToPdf();
            //var fullUrl = this.Url.Action("Posts", "Edit", new { id = 5 }, this.Request.Url.Scheme);
            //Request.RequestUri.PathAndQuery
            var fullurl = this.Url.Action("LeaveIndexView", "LeaveRequests", null, this.Request.Url.Scheme);
            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertUrl(fullurl);
            MemoryStream stream = new MemoryStream();
            // save pdf document   
            doc.Save(stream);
            //doc.Save(System.Web.HttpContext.Current.Response, false, pdfname);
            //close pdf document
            doc.Close();
            stream.Position = 0;
            return File(stream, "application/pdf", pdfname);  //pdfname 儲存PDF的名稱
        }

        //public ActionResult Leavetable(string id, string id2 ,string value)
        //{
        //    string User = Convert.ToString(Session["UserName"]);  //從Session抓UserID
        //    DateTime startime = Convert.ToDateTime(id.Substring(0, 4) + "/" + id.Substring(4, 2) + "/" + id.Substring(6, 2) + " 00:00:00");
        //    DateTime endtime = Convert.ToDateTime(id2.Substring(0, 4) + "/" + id2.Substring(4, 2) + "/" + id2.Substring(6, 2) + " 23:59:59");

        //    var LeaveTimeRequest = (from lt in db.LeaveRequests.AsEnumerable()
        //                            join emp in db.Employees.AsEnumerable() on lt.EmployeeID equals emp.EmployeeID
        //                            join rev in db.ReviewStatus.AsEnumerable() on lt.ReviewStatusID equals rev.ReviewStatusID
        //                            where lt.StartTime >= startime && lt.EndTime <= endtime && lt.EmployeeID == User
        //                            select new LeaveIndexViewModel
        //                            {
        //                                LeaveRequestID = lt.LeaveRequestID,
        //                                EmployeeName = emp.EmployeeName,
        //                                LeaveType = lt.LeaveType,
        //                                RequestTime = lt.RequestTime,
        //                                StartTime = lt.StartTime,
        //                                EndTime = lt.EndTime,
        //                                LeaveReason = lt.LeaveReason,
        //                                Review = rev.ReviewStatus1,
        //                                ReviewTime = lt.ReviewTime,
        //                                Attachment = lt.Attachment
        //                            });
        //    if (LeaveTimeRequest != null)
        //    {
        //        return PartialView("_LeaveIndexPartialView", LeaveTimeRequest);

        //    }
        //    else
        //    {
        //        return HttpNotFound();
        //    }
        //}
        public ActionResult Details(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            LeaveRequests leaveRequests = db.LeaveRequests.Find(id);
            if (leaveRequests == null)
            {
                return HttpNotFound();
            }
            return View(leaveRequests);
        }

        // GET: LeaveRequests/Create

        public class LeaveCombobox
        {
            public string comtext { get; set; }
            public string text { get; set; }
        }

        public ActionResult Create()
        {
            ViewBag.LeaveType = new SelectList(new List<LeaveCombobox> {
                new LeaveCombobox {comtext="事假",text="事假" },
                new LeaveCombobox {comtext="病假",text="病假"},
                new LeaveCombobox {comtext="公假",text="公假"},
                new LeaveCombobox {comtext="喪假",text="喪假"},
                new LeaveCombobox {comtext="特休假",text="特休假"},
                new LeaveCombobox {comtext="產假",text="產假"},
                new LeaveCombobox {comtext="陪產假",text="陪產假"},
                new LeaveCombobox {comtext="生理假",text="生理假"},
                new LeaveCombobox {comtext="補休假",text="補休假"},
                new LeaveCombobox {comtext="家庭照顧假",text="家庭照顧假"},
            }, "text", "comtext");

           ViewBag.on = remleave();  //剩餘特休天數
           ViewBag.Off= Days();      //特休天數共幾天
            return PartialView("_Create");
        }

        // POST: LeaveRequests/Create
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。


        #region 算剩餘的特休天數
        public int remleave()
        {
            string User = Convert.ToString(Session["UserName"]);  //從Session抓UserID

            var t = DateTime.Now.Year;
            var dm = db.Employees.Find(User).Hireday.Month;
            var dd = db.Employees.Find(User).Hireday.Day;
            DateTime t1 = DateTime.Parse($"{t}-{dm}-{dd}");
            DateTime t2 = DateTime.Parse($"{t+1}-{dm}-{dd}");
            int Remain = Days() -( db.LeaveRequests.Where(n => (n.StartTime >= t1 && n.EndTime <= t2 && n.EmployeeID == User && n.LeaveType=="特休假")) .Count());
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LeaveRequestID,EmployeeID,RequestTime,StartTime,EndTime,LeaveType,LeaveReason,ReviewStatusID,ReviewTime,Attachment")] LeaveRequests leaveRequests)
        {
            if (ModelState.IsValid)
            {
                string User = Convert.ToString(Session["UserName"]);  //從Session抓UserID
                leaveRequests.LeaveRequestID = db.LeaveRequests.Count().ToString();
                leaveRequests.EmployeeID = "MSIT1230001";
                leaveRequests.RequestTime = DateTime.Now;
                leaveRequests.ReviewStatusID = 1;

               


                if (Request.Files["LeaveFile"].ContentLength != 0)
                {
                    byte[] data = null;
                    using (BinaryReader br = new BinaryReader(
                        Request.Files["LeaveFile"].InputStream))
                    {
                        data = br.ReadBytes(Request.Files["LeaveFile"].ContentLength);
                    }
                    leaveRequests.Attachment = data;
                }

                //判斷是不是有申請過
                if (db.LeaveRequests.Any(n=>(n.StartTime<=leaveRequests.StartTime&&n.EndTime>=leaveRequests.StartTime) ||(n.StartTime <= leaveRequests.EndTime && n.EndTime>= leaveRequests.EndTime)))
                {
                    return Content("alert('此日期已申請過')");
                }
              
                    db.LeaveRequests.Add(leaveRequests);
                    db.SaveChanges();
                    return RedirectToAction("LeaveIndexView");

                
            }

            return View(leaveRequests);
        }

        // GET: LeaveRequests/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LeaveRequests leaveRequests = db.LeaveRequests.Find(id);
            if (leaveRequests == null)
            {
                return HttpNotFound();
            }
            return View(leaveRequests);
        }

        // POST: LeaveRequests/Edit/5
        // 若要免於過量張貼攻擊，請啟用想要繫結的特定屬性，如需
        // 詳細資訊，請參閱 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LeaveRequestID,EmployeeID,RequestTime,StartTime,EndTime,LeaveType,LeaveReason,ReviewStatusID,ReviewTime,Attachment")] LeaveRequests leaveRequests)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files["LeaveFile"].ContentLength != 0)
                {
                    byte[] data = null;
                    using (BinaryReader br = new BinaryReader(
                        Request.Files["LeaveFile"].InputStream))
                    {
                        data = br.ReadBytes(Request.Files["LeaveFile"].ContentLength);
                    }
                    leaveRequests.Attachment = data;
                }
                else
                {
                    LeaveRequests L = db.LeaveRequests.Find(leaveRequests.LeaveRequestID);
                    L.EmployeeID = leaveRequests.EmployeeID;
                    L.RequestTime = leaveRequests.RequestTime;
                    L.StartTime = leaveRequests.StartTime;
                    L.EndTime = leaveRequests.StartTime;
                    L.LeaveType = leaveRequests.LeaveType;
                    L.LeaveReason = leaveRequests.LeaveReason;
                    L.LeaveRequestID = leaveRequests.LeaveRequestID;
                    L.RequestTime = leaveRequests.RequestTime;
                    leaveRequests = L;
                }


                db.Entry(leaveRequests).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(leaveRequests);
        }

        // GET: LeaveRequests/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LeaveRequests leaveRequests = db.LeaveRequests.Find(id);
            if (leaveRequests == null)
            {
                return HttpNotFound();
            }
            return View(leaveRequests);
        }

        // POST: LeaveRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            LeaveRequests leaveRequests = db.LeaveRequests.Find(id);
            db.LeaveRequests.Remove(leaveRequests);
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
            byte[] content = db.LeaveRequests.Find(id).Attachment;
            return File(content, "image/jpeg");
        }

        [HttpPost]
        public ActionResult DownLoadExcel(string id, string id2, string[] value)
        {
            #region 查詢資料
 
            string User = Convert.ToString(Session["UserName"]);  //從Session抓UserID
            DateTime startime = Convert.ToDateTime(id);
            DateTime endtime = Convert.ToDateTime(id2);
            string[] LeaveValue = value;


            var LeaveTimeRequest = from lt in db.LeaveRequests.AsEnumerable()
                                    join emp in db.Employees.AsEnumerable() on lt.EmployeeID equals emp.EmployeeID
                                    join rev in db.ReviewStatus.AsEnumerable() on lt.ReviewStatusID equals rev.ReviewStatusID
                                    where lt.StartTime >= startime && lt.EndTime <= endtime && lt.EmployeeID == User && LeaveValue.Any(x => x == lt.LeaveType)
                                    select new LeaveIndexViewModel
                                    {
                                        LeaveRequestID = lt.LeaveRequestID,
                                        EmployeeName = emp.EmployeeName,
                                        LeaveType = lt.LeaveType,
                                        RequestTime = lt.RequestTime,
                                        StartTime = lt.StartTime,
                                        EndTime = lt.EndTime,
                                        LeaveReason = lt.LeaveReason,
                                        Review = rev.ReviewStatus1,
                                        ReviewTime = lt.ReviewTime,
                                        Attachment = lt.Attachment
                                    };

            string[] titleList = new string[] { "序號", "假單編號", "員工姓名", "假別", "申請時間", "申請開始時間", "申請結束時間", "事由", "申請狀態"};
            List<LeaveIndexViewModel> list = LeaveTimeRequest.ToList();/*OrderInfoBLL.GetOrdersInfoListByTime(OrderStartTime, OrderEndTime);*/
            #endregion

            #region NPOI資料匯出
            //建立Excel檔案的物件
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //新增一個sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //給sheet1新增第一行的頭部標題
            IRow headerrow = sheet1.CreateRow(0);

            HSSFCellStyle headStyle = (HSSFCellStyle)book.CreateCellStyle();
            HSSFFont font = (HSSFFont)book.CreateFont();
            font.FontHeightInPoints = 10;
            font.Boldweight = 700;
            headStyle.SetFont(font);
            for (int i = 0; i < titleList.Length; i++)
            {
                ICell cell = headerrow.CreateCell(i);
                cell.CellStyle = headStyle;
                cell.SetCellValue(titleList[i]);
            }
            //將資料逐步寫入sheet1各個行
            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(i + 1);
                rowtemp.CreateCell(1).SetCellValue(list[i].LeaveRequestID);
                rowtemp.CreateCell(2).SetCellValue(list[i].EmployeeName);
                rowtemp.CreateCell(3).SetCellValue(list[i].LeaveType);
                rowtemp.CreateCell(4).SetCellValue(list[i].RequestTime.ToString("yyyy-MM-dd"));
                rowtemp.CreateCell(5).SetCellValue(list[i].StartTime.ToString("yyyy-MM-dd HH:mm"));
                rowtemp.CreateCell(6).SetCellValue(list[i].EndTime.ToString("yyyy-MM-dd HH:mm"));
                rowtemp.CreateCell(7).SetCellValue(list[i].LeaveReason);
                rowtemp.CreateCell(8).SetCellValue(list[i].Review == "未審核" ? "未審核" : list[i].Review == "已審核" ? "已審核" : list[i].Review == "駁回" ? "駁回" : "取消稽核不通過");
                //rowtemp.CreateCell(9).SetCellValue(list[i].ReviewTime.ToString("yyyy-MM-dd HH:mm"));
            }
            var guid = Guid.NewGuid().ToString();
            DownLoadHelper.SetCache(guid, book);
            return Json(guid, JsonRequestBehavior.AllowGet);
            #endregion
        }


        public void OutExcel(string guid)
        {
            object obj = DownLoadHelper.GetCache(guid);
            NPOI.HSSF.UserModel.HSSFWorkbook book = obj as NPOI.HSSF.UserModel.HSSFWorkbook;
            if (book != null)
            {
                //寫入到客戶端
                MemoryStream ms = new MemoryStream();
                book.Write(ms);
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}.xls", DateTime.Now.ToString("yyyyMMddHHmmssfff")));
                Response.BinaryWrite(ms.ToArray());
                book = null;
                ms.Close();
                ms.Dispose();
            }
            DownLoadHelper.RemoveAllCache(guid);
        }
   

    }
}
