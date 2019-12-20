using System;

namespace WebApplication5.Controllers
{
    public class Serch_UncHeck
    {
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string Date { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string LineID { get; set; }
        public string station { get; set; }
    }
}