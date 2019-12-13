using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AMS.Metadata
{
    public class AnnouncementMetadata
    {
        [Key]
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

        [DataType(DataType.DateTime),Display(Name = "公告時間")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> AnnounceTime { get; set; }
    }
}