using System.ComponentModel.DataAnnotations;

namespace EmployeeSurveyApp.Models
{
    public class AdminLoginViewModel
    {
        [Required(ErrorMessage = "Admin Username or Email is required")]
        [Display(Name = "Admin Username / Email")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
