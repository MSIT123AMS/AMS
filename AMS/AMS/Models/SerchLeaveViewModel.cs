using System;
using System.ComponentModel.DataAnnotations;

namespace AMS.Controllers
{
    public class SerchLeaveViewModel
    {
        [Key]
        [Display(Name = "單號")]
        public string LeaveRequestID { get; set; }
        [Display(Name = "員工姓名")]
        public string EmployeeName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "申請日期")]
        public DateTime? RequestTime { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "開始時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? StartTime { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "結束時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? EndTime { get; set; }
        [Display(Name = "審核狀態")]
        public string Review { get; set; }
        [Display(Name = "假別")]
        public string LeaveType { get; internal set; }
    }
}