using System;
using System.ComponentModel.DataAnnotations;

namespace AMS.Models
{
    public class ClockInApplyViewModel
    {
        [Key]        
        public string EmployeeID { get; set; }
        [Display(Name = "員工姓名")]
        public string EmployeeName { get; set; }
        [Display(Name = "上班時間")]
        public DateTime OnDuty { get; set; }
        [Display(Name = "下班時間")]
        public DateTime? OffDuty { get; set; }
        [DataType(DataType.DateTime), Display(Name = "申請日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? RequestDate { get; set; }
        [Display(Name = "審核狀態")]
        public string ReviewStatus1 { get; set; }
    }
}