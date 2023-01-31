using System.ComponentModel.DataAnnotations;

namespace Test_Project.Models
{
    public class Employee
    {
        public int id { get; set; }

        [Required(ErrorMessage = "پر کردن این فیلد الزامی می‌باشد")]
        [MinLength(3, ErrorMessage = "حداقل 3 کاراکتر الزامی می‌باشد")]
        [MaxLength(50, ErrorMessage = "حداکثر 50 کاراکتر مجاز می‌باشد")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "پر کردن این فیلد الزامی می‌باشد")]
        [RegularExpression("^[0-9]{11}$", ErrorMessage = "لطفا موبایل را به صورت " + "0912xxxxxxx" + "وارد نمائید")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "پر کردن این فیلد الزامی می‌باشد؛ در صورت مقدار نداشتن صفر قرار دهید")]
        [Range(0 ,120 ,ErrorMessage = "لطفا سن را بطور صحیح وارد نمائید")]
        public int Age { get; set; }

        [MaxLength(100, ErrorMessage = "حداکثر 100 کاراکتر مجاز می‌باشد")]
        public string Address { get; set; }
    }
}
