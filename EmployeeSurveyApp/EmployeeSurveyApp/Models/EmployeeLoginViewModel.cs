using System.ComponentModel.DataAnnotations;

namespace EmployeeSurveyApp.Models
{
    public class EmployeeLoginViewModel
    {
        [Required(ErrorMessage = "Personnel Number (PERNR) is required")]
        [Display(Name = "Personnel Number (PERNR) / Employee ID")]
        public string EmployeeCode { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Work Email (Optional)")]
        public string? WorkEmail { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
