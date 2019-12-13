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

        public string rendering { get; set; }
    }

    public class CalendarAttendance
    {
        public string AbsenceType { get; set; }

        public string Name { get; set; }

    }





    public class HolidayFirst
    {
        public bool success { get; set; }
        public HolidaySecond result { get; set; }
    }

    public class HolidaySecond
    {
        public string resource_id { get; set; }
        public int limit { get; set; }
        public int total { get; set; }
        public Field[] fields { get; set; }
        public Holidays[] records { get; set; }
    }

    public class Field
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class Holidays
    {
        public string date { get; set; }
        public string name { get; set; }
        public string isHoliday { get; set; }
        public string holidayCategory { get; set; }
        public string description { get; set; }
    }


}