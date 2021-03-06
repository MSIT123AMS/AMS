﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AMS.Models
{
    public class LeaveReviewViewModels
    {
        [Key]
        public string LeaveRequestID { get; set; }
        public string EmployeeID { get; set; }
        [DisplayName("員工姓名")]
        public string EmployeeName { get; set; }
        [DisplayName("假別")]
        public string LeaveType { get; set; }
        [DisplayName("起始日")]
        public DateTime? StartTime { get; set; }
        [DisplayName("結束日")]
        public DateTime? EndTime { get; set; }
        [DisplayName("申請日期")]
        public DateTime? RequestTime { get; set; }
        [DisplayName("事由")]
        public string LeaveReason { get; set; }
        public int ReviewStatusID { get; set; }
        [DisplayName("審核狀態")]
        public string ReviewStatus { get; set; }
    }

    public class OverTimeReviewViewModels
    {

        public string EmployeeID { get; set; }
        [DisplayName("員工姓名")]
        public string EmployeeName { get; set; }
        public bool OverTimePay { get; set; }
        [DisplayName("起始日")]
        public DateTime StartTime { get; set; }
        [DisplayName("結束日")]
        public DateTime EndTime { get; set; }
        [DisplayName("申請日期")]
        public DateTime RequestTime { get; set; }
        [DisplayName("事由")]
        public string OverTimeReason { get; set; }
        [DisplayName("審核狀態")]
        public string ReviewStatus { get; set; }

        public int ReviewStatusID { get; set; }
        [Key]
        public string OverTimeRequestID { get; set; }
    }
}