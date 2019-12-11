using System;
using System.ComponentModel.DataAnnotations;

namespace AMS.Models
{
    public class calendar
    {

        public string title { get; set; }
        
        public string start { get; set; }
        
        public string end { get; set; }

        public string backgroundColor { get; set; }
    }

    public class CalendarAttendance
    {
        public string AbsenceType { get; set; }

        public string Name { get; set; }

    }

}