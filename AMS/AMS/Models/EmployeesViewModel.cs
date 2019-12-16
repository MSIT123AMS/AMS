using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace AMS.Controllers
{
    public class MonthlyStatisticsViewModel
    {
        [Display(Name = "員工編號")]
        public string EmployeeID { get; set; }
        [Display(Name = "姓名")]
        public string EmployeeName { get; set; }
        [Display(Name = "工作日")]
        public string DepartmentName { get; set; }
        public int WorkingDay { get; set; }
        [Display(Name = "出勤日")]
        public int AttendanceDay { get; set; }
        [Display(Name = "工作時數")]
        public int WorkingDayHours { get; set; }
        [Display(Name = "出勤時數")]
        public int AttendanceDayHours { get; set; }
        [Display(Name = "出勤率")]
        public double AttendanceRate { get; set; }
        [Display(Name = "請假時數")]
        public int LeaveDayHours { get; set; }
        [Display(Name = "加班時數")]
        public double? OverTimeHours { get; set; }
    }

    public class OverTimeHoursSumModel
    {
        public string EmployeeID { get; set; }
        public double? Q { get; set; }
    }
    public class Daily
    {
        public int 人數 { get; set; }
        public int 請假 { get; set; }
        public int 未到 { get; set; }
        public int? 部門 { get; internal set; }
    }
    public class DailyStatisticsViewModel1
    {
        [Display(Name = "姓名")]
        public string EmployeeName { get; set; }
        [Display(Name = "請假類別")]
        public string LeaveType { get; set; }
        [DataType(DataType.DateTime), Display(Name = "上班時間")]
        [DisplayFormat(DataFormatString = "{0: HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? StartTime { get; set; }

        [DataType(DataType.DateTime), Display(Name = "下班時間")]
        [DisplayFormat(DataFormatString = "{0: HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? EndTime { get; set; }
    }




    public class LineIdBindViewModel
    {
        [Key]
        [Display(Name = "員工編號")]
        public string EmployeeID { get; set; }
        [Display(Name = "密碼")]
        public string Password { get; set; }
        public string LineID { get; set; }
    }



        public class EmployeesViewModel
    {
        [Display(Name = "員工姓名")]
        public string EmployeeName { get; set; }
        [Display(Name = "部門")]
        public string DepartmentName { get; set; }
        [Display(Name = "職位名稱")]
        public string JobTitle { get; set; }
        [Display(Name = "主管")]
        public string Manager { get; set; }
        [Display(Name = "到職日")]
        public string Hireday { get; set; }
        [Display(Name = "狀態")]
        public string JobStaus { get; set; }
        [Key]
        public string EmployeeID { get; set; }
    }




    public class EmployeesCreateViewModel
    {
        [Key]
        [Display(Name = "員工編號")]
        public string EmployeeID { get; set; }
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [Display(Name = "員工姓名")]
        [Required(ErrorMessage ="XXXX")]
        public string EmployeeName { get; set; }
        [Display(Name = "英文名字")]
        [Required(ErrorMessage = "XXXX")]
        public string EnglishName { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        [Display(Name = "生日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]        
        public DateTime Birthday { get; set; }
        [Display(Name = "身分證字號")]
        public string IDNumber { get; set; }
        [Display(Name = "性別")]
        public Nullable<bool> gender { get; set; }
        [Display(Name = "婚姻")]
        [Required]
        public string Marital { get; set; }
        [Display(Name = "電子信箱")]
        public string Email { get; set; }
        [Display(Name = "聯絡地址")]
        public string Address { get; set; }
        [Display(Name = "連絡電話")]
        public string Phone { get; set; }
        [Display(Name = "部門")]
        public string DepartmentName { get; set; }       
        [Display(Name = "主管")]
        public string Manager { get; set; }
        [Display(Name = "職位名稱")]
        public string JobTitle { get; set; }
        [Display(Name = "學歷")]
        public string Education { get; set; }
        [Display(Name = "備註")]
        public string Notes { get; set; }
        [Display(Name = "代理人")]
        public string Deputy { get; set; }
        [Display(Name = "代理人電話")]
        public string DeputyPhone { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "到職日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]        
        public DateTime Hireday { get; set; }
        [Display(Name = "照片")]
        public HttpPostedFileBase Photo { get; set; }  
    }
    public class LoginFaceModel
    {
        [Display(Name = "照片")]
        public HttpPostedFileBase Photo { get; set; }
    }
    public class EmployeesDetailsViewModel
    {
        [Key]
        [Display(Name = "員工編號")]
        public string EmployeeID { get; set; }
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [Display(Name = "員工姓名")]
        [Required]
        public string EmployeeName { get; set; }
        [Display(Name = "英文名字")]
        public string EnglishName { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "生日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; set; }
        [Display(Name = "身分證字號")]
        public string IDNumber { get; set; }
        [Display(Name = "性別")]
        public string gender { get; set; }
        [Display(Name = "婚姻")]
        public string Marital { get; set; }
        [Display(Name = "電子信箱")]
        public string Email { get; set; }
        [Display(Name = "聯絡地址")]
        public string Address { get; set; }
        [Display(Name = "連絡電話")]
        public string Phone { get; set; }
        [Display(Name = "部門")]
        public string DepartmentName { get; set; }
        [Display(Name = "主管")]
        public string Manager { get; set; }
        [Display(Name = "職位名稱")]
        public string JobTitle { get; set; }
        [Display(Name = "學歷")]
        public string Education { get; set; }
        [Display(Name = "備註")]
        public string Notes { get; set; }
        [Display(Name = "代理人")]
        public string Deputy { get; set; }
        [Display(Name = "代理人電話")]
        public string DeputyPhone { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "到職日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime Hireday { get; set; }
        [Display(Name = "照片")]
        public byte[] Photo { get; set; }

        [Display(Name = "離職日")]
        public System.DateTime Leaveday { get; set; }
        [Display(Name = "狀況")]
        public string JobStaus { get; set; }
    }

    public class EmployeesEditViewModel
    {
        [Key]
        [Display(Name = "員工編號")]
        public string EmployeeID { get; set; }
        [Display(Name = "密碼")]
        public string Password { get; set; }
        [Display(Name = "員工姓名")]
        public string EmployeeName { get; set; }
        [Display(Name = "英文名字")]
        public string EnglishName { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "生日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime Birthday { get; set; }
        [Display(Name = "身分證字號")]
        public string IDNumber { get; set; }
        [Display(Name = "性別")]
        public string gender { get; set; }
        [Display(Name = "婚姻")]
        public string Marital { get; set; }
        [Display(Name = "電子信箱")]
        public string Email { get; set; }
        [Display(Name = "聯絡地址")]
        public string Address { get; set; }
        [Display(Name = "連絡電話")]
        public string Phone { get; set; }
        [Display(Name = "部門")]
        public string DepartmentName { get; set; }
        [Display(Name = "主管")]
        public string Manager { get; set; }
        [Display(Name = "職位名稱")]
        public string JobTitle { get; set; }
        [Display(Name = "學歷")]
        public string Education { get; set; }
        [Display(Name = "備註")]
        public string Notes { get; set; }
        [Display(Name = "代理人")]
        public string Deputy { get; set; }
        [Display(Name = "代理人電話")]
        public string DeputyPhone { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "到職日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime Hireday { get; set; }
        [Display(Name = "照片")]
        public HttpPostedFileBase Photo { get; set; }

        [Display(Name = "離職日")]
        public System.DateTime Leaveday { get; set; }
        [Display(Name = "狀況")]
        public string JobStaus { get; set; }
    }





}