using System;
using System.ComponentModel.DataAnnotations;

namespace AMS.Controllers
{
    public class SerchAttendancesViewModel
    {
        [Display(Name = "員工姓名")]
        public string EmployeeName { get; set; }
        [Display(Name = "日期")]
        public string Date { get; set; }
        [Display(Name = "上班時間")]
        public DateTime? StartTime { get; set; }
        [Display(Name = "下班時間")]
        public DateTime? EndTime { get; set; }
    }
}