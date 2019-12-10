using System;
using System.ComponentModel.DataAnnotations;

namespace AMS.Models
{
    public class AnnouncementViewModel
    {
        [Display(Name = "公告標題")]
        public string Title { get; set; }

        [Display(Name = "公告內容")]
        public string Detail { get; set; }

        [Display(Name = "重要性")]
        public string Importance { get; set; }
        
    }
}