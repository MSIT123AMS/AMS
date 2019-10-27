using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Metadata
{
    public class LeaveRequestesMatadata
    {
        public string LeaveRequestID { get; set; }
        [Display(Name = "員工編號")]
        public string EmployeeID { get; set; }
        [DataType(DataType.DateTime),Display(Name = "申請時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> RequestTime { get; set; }
        [DataType(DataType.DateTime), Display(Name = "請假開始時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> StartTime { get; set; }
        [DataType(DataType.DateTime), Display(Name = "請假結束時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> EndTime { get; set; }
        [Display(Name = "假別")]
        public string LeaveType { get; set; }
        [Display(Name = "事由")]
        public string LeaveReason { get; set; }
        [Display(Name = "審核狀態")]
        public int ReviewStatusID { get; set; }
        [Display(Name = "審核時間")]
        public Nullable<System.DateTime> ReviewTime { get; set; }
        [Display(Name = "附件")]
        public byte[] Attachment { get; set; }
    }
}