using System;
using System.ComponentModel.DataAnnotations;
namespace AMS.Controllers
{
    public class SerchAttendancesViewModel
    {
        [Key]
        [Display(Name = "員工姓名")]
        public string EmployeeName { get; set; }
        [Display(Name = "打卡日")]
        public string Date { get; set; }

        [DataType(DataType.DateTime), Display(Name = "上班時間")]
        [DisplayFormat(DataFormatString = "{0: HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? StartTime { get; set; }

        [DataType(DataType.DateTime), Display(Name = "下班時間")]
        [DisplayFormat(DataFormatString = "{0: HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? EndTime { get; set; }
    }
}