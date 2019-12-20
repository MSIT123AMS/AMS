using System;

namespace AMS.Controllers
{
    internal class SerchAttendancesViewModel
    {
        public string EmployeeName { get; set; }
        public string Date { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}