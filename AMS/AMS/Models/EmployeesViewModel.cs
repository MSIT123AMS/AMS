using System;
using System.ComponentModel.DataAnnotations;

namespace AMS.Controllers
{
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
        public Nullable<bool> gender { get; set; }
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
    }




}