using System;
using System.ComponentModel.DataAnnotations;

namespace AMS.Models
{
    public class OverTimeViewModel
    {
        [Key]
        [Display(Name = "單號")]
        public string RequestID { get; set; }

        [Display(Name = "員工姓名")]
        public string EmployeeName { get; set; }

        [ DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy/MM/dd}",ApplyFormatInEditMode =true)]
        [Display(Name ="申請日期")]
        public DateTime? RequestTime { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "開始時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? StartTime { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "結束時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? EndTime { get; set; }

        [Display(Name = "請假補休")]
        public string PayorOFF { get; set; }

        [Display(Name = "加班日類別")]
        public string OTDateType { get; set; }

        [Display(Name = "加權時數")]
        public string SummaryTime { get; set; }

        [Display(Name = "申請理由")]
        public string Reason { get; set; }

        [Display(Name = "審核狀態")]
        public string Review { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "審核時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd} hh:mm", ApplyFormatInEditMode = true)]
        public DateTime? ReviewTime { get; set; }
    }

    public class OverTimeCreateViewModel
    {
        [Key]
        [Display(Name = "單號")]
        public string RequestID { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "開始時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "結束時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime EndTime { get; set; }

        [Display(Name = "請假補休")]
        public string PayorOFF { get; set; }

        [Display(Name = "申請理由")]
        public string Reason { get; set; }


    }



}