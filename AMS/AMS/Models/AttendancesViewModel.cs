using System;
using System.ComponentModel.DataAnnotations;

namespace AMS.Models
{
    public class AttendancesViewModel
    {
        [Key]
        public string EmployeeID { get; set; }
        [Display(Name = "員工姓名")]
        public string EmployeeName { get; set; }
        [DataType(DataType.DateTime), Display(Name = "上班日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [DataType(DataType.DateTime), Display(Name = "上班時間")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? OnDuty { get; set; }
        [DataType(DataType.DateTime), Display(Name = "下班時間")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? OffDuty { get; set; }
        [Display(Name = "異常")]
        public string station { get; set; }
    }
}