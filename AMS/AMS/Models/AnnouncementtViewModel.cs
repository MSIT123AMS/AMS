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
    public class AnnouncementDisplayViewModel
    {
        [Display(Name = "公告標題")]
        public string Title { get; set; }

        [Display(Name = "公告日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> AnnounceTime { get; set; }

        [Display(Name = "重要性")]
        public string Importance { get; set; }

    }
    public partial class AnnouncementIndexViewModel
    {

        [Display(Name = "公告編號")]
        public int AnnouuncementID { get; set; }

        [Display(Name = "公告人")]
        public string EmployeeID { get; set; }

        [Display(Name = "公告標題")]
        public string Title { get; set; }

        [Display(Name = "公告內容")]
        public string Detail { get; set; }

        [Display(Name = "重要性")]
        public string Importance { get; set; }

        [Display(Name = "公告日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> AnnounceTime { get; set; }
    }

}