using System.ComponentModel.DataAnnotations;

namespace EmployeeSurveyApp.Models
{
    public class EmployeeSurvey
    {
        public int Id { get; set; }

        // =========================================================
        // EMPLOYEE IDENTITY
        // =========================================================

        [Required]
        [Display(Name = "Employee Code")]
        [StringLength(50)]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "First Name")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Middle Name")]
        public string? MiddleName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Preferred Name")]
        public string? PreferredName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200)]
        [Display(Name = "Work Email")]
        public string WorkEmail { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(200)]
        [Display(Name = "Personal Email")]
        public string? PersonalEmail { get; set; }

        [Phone]
        [StringLength(30)]
        public string? Phone { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(50)]
        public string? Gender { get; set; }

        // =========================================================
        // EMPLOYMENT INFORMATION
        // =========================================================

        [Required]
        [StringLength(150)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Employment Type")]
        public string EmploymentType { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Employment Status")]
        public string EmploymentStatus { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Date Joined")]
        public DateTime? DateJoined { get; set; }

        [StringLength(150)]
        [Display(Name = "Work Location")]
        public string? WorkLocation { get; set; }

        [StringLength(50)]
        [Display(Name = "Work Mode")]
        public string? WorkMode { get; set; }

        [StringLength(150)]
        [Display(Name = "Manager Name")]
        public string? ManagerName { get; set; }

        [Range(0, 60)]
        [Display(Name = "Years of Experience")]
        public decimal? YearsOfExperience { get; set; }

        [StringLength(100)]
        [Display(Name = "Education Level")]
        public string? EducationLevel { get; set; }

        // =========================================================
        // COMPENSATION
        // =========================================================

        [Range(0, 100000000)]
        [Display(Name = "Annual Salary")]
        public decimal? AnnualSalary { get; set; }

        [StringLength(10)]
        [Display(Name = "Salary Currency")]
        public string? SalaryCurrency { get; set; }

        [Range(1, 5)]
        [Display(Name = "Benefits Satisfaction")]
        public int? BenefitsSatisfaction { get; set; }

        // =========================================================
        // EMPLOYEE ENGAGEMENT
        // =========================================================

        [Range(1, 5)]
        [Display(Name = "Overall Satisfaction")]
        public int OverallSatisfaction { get; set; }

        [Range(1, 5)]
        [Display(Name = "Job Satisfaction")]
        public int JobSatisfaction { get; set; }

        [Range(1, 5)]
        [Display(Name = "Management Satisfaction")]
        public int ManagementSatisfaction { get; set; }

        [Range(1, 5)]
        [Display(Name = "Work-Life Balance")]
        public int WorkLifeBalance { get; set; }

        [Range(1, 5)]
        [Display(Name = "Career Growth")]
        public int CareerGrowth { get; set; }

        [Range(1, 5)]
        [Display(Name = "Learning & Development")]
        public int LearningDevelopment { get; set; }

        [Range(1, 5)]
        [Display(Name = "Recognition")]
        public int Recognition { get; set; }

        [Range(1, 5)]
        [Display(Name = "Compensation Satisfaction")]
        public int CompensationSatisfaction { get; set; }

        [Range(1, 5)]
        [Display(Name = "Workplace Culture")]
        public int WorkplaceCulture { get; set; }

        // =========================================================
        // WORK ENVIRONMENT
        // =========================================================

        [Range(1, 5)]
        [Display(Name = "Remote Work Satisfaction")]
        public int? RemoteWorkSatisfaction { get; set; }

        [Range(1, 5)]
        [Display(Name = "Team Collaboration")]
        public int TeamCollaboration { get; set; }

        [Range(1, 5)]
        [Display(Name = "Communication")]
        public int Communication { get; set; }

        [Range(1, 5)]
        [Display(Name = "Workload")]
        public int Workload { get; set; }

        [Range(1, 5)]
        [Display(Name = "Technology & Tools Satisfaction")]
        public int TechnologyToolsSatisfaction { get; set; }

        // =========================================================
        // QUALITATIVE FEEDBACK
        // =========================================================

        [StringLength(2000)]
        [Display(Name = "Strengths")]
        public string? Strengths { get; set; }

        [StringLength(2000)]
        [Display(Name = "Challenges")]
        public string? Challenges { get; set; }

        [StringLength(2000)]
        [Display(Name = "Improvement Suggestions")]
        public string? ImprovementSuggestions { get; set; }

        [StringLength(2000)]
        [Display(Name = "Career Goals")]
        public string? CareerGoals { get; set; }

        [StringLength(4000)]
        [Display(Name = "Additional Comments")]
        public string? AdditionalComments { get; set; }

        // =========================================================
        // SURVEY METADATA
        // =========================================================

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Survey Date")]
        public DateTime SurveyDate { get; set; }

        [StringLength(50)]
        [Display(Name = "Survey Period")]
        public string? SurveyPeriod { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Survey Status")]
        public string SurveyStatus { get; set; } = "Submitted";

        [Display(Name = "Anonymous Survey")]
        public bool IsAnonymous { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Updated Date")]
        public DateTime? UpdatedDate { get; set; }
    }
}