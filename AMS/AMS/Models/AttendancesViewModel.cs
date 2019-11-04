using System;
using System.ComponentModel.DataAnnotations;

namespace AMS.Controllers
{
    internal class AttendancesViewModel
    {
        [Key]
        public string EmployeeID { get; set; }
        [Display(Name = "員工姓名")]
        public string EmployeeName { get; set; }
        [DataType(DataType.DateTime), Display(Name = "上班日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [DataType(DataType.DateTime), Display(Name = "上班時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? OnDuty { get; set; }
        public DateTime? OffDuty { get; set; }
        public string station { get; set; }
    }
}