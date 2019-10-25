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
}