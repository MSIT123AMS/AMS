using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AMS.Metadata
{
    public class ClockInApplyMatadata
    {
        [Display(Name = "員工編號")]
        public string EmployeeID { get; set; }
        [Display(Name = "上班時間:")]
        public System.DateTime OnDuty { get; set; }
        [Display(Name = "下班時間:")]
        public Nullable<System.DateTime> OffDuty { get; set; }
        [Display(Name = "審核ID")]
        public Nullable<int> ReviewStatusID { get; set; }
        [DataType(DataType.DateTime), Display(Name = "申請日期:")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> RequestDate { get; set; }
        [Display(Name = "審核時間")]
        public Nullable<System.DateTime> ReviewTime { get; set; }
    }
}