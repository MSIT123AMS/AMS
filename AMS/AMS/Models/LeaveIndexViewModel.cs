using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace AMS.Models
{
    public class LeaveIndexViewModel
    {
        [Key]
        [Display(Name = "假單編號")]
        public string LeaveRequestID { get; set; }
        [Display(Name = "員工姓名")]
        public string EmployeeName { get; set; }
        [Display(Name = "假別")]
        public string LeaveType { get; set; }
        [DataType(DataType.DateTime), Display(Name = "申請時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime RequestTime { get; set; }
        [DataType(DataType.DateTime), Display(Name = "開始時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }
        [DataType(DataType.DateTime), Display(Name = "結束時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndTime { get; set; }
        [Display(Name = "事由")]
        public string LeaveReason { get; set; }
        [Display(Name = "申請狀態")]
        public string Review { get; set; }
        [DataType(DataType.DateTime), Display(Name = "審核時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ReviewTime { get; set; }
        [Display(Name = "附件")]
        public byte[] Attachment { get; set; }
    }


    public partial class LeaveRequestsViewModel
    {
        public string LeaveRequestID { get; set; }
        public string EmployeeID { get; set; }
        public System.DateTime RequestTime { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime EndTime { get; set; }
        public string LeaveType { get; set; }
        public string LeaveReason { get; set; }
        public int ReviewStatusID { get; set; }
        public Nullable<System.DateTime> ReviewTime { get; set; }
        public HttpPostedFileBase Attachment { get; set; }
    }


}