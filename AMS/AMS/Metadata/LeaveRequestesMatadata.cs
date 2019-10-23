using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMS.Metadata
{
    public class LeaveRequestesMatadata
    {
        public string LeaveRequestID { get; set; }
        public string EmployeeID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> RequestTime { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> StartTime { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> EndTime { get; set; }
        public string LeaveType { get; set; }
        public string LeaveReason { get; set; }
        public int ReviewStatusID { get; set; }
        public Nullable<System.DateTime> ReviewTime { get; set; }
        public byte[] Attachment { get; set; }
    }
}