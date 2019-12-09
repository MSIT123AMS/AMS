using System;
using System.ComponentModel.DataAnnotations;

namespace AMS.Models
{
    public class AnnouncementViewModel
    {         
        public string Title { get; set; }

        public string Detail { get; set; }

        public string Importance { get; set; }
        
    }
}