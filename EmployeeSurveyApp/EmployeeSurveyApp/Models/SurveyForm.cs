using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeSurveyApp.Models
{
    [Table("survey_forms")]
    public class SurveyForm
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Survey Title is required")]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "At least one PERNR must be allowed to attend this survey")]
        [Column("allowed_pernrs")]
        public string AllowedPernrs { get; set; } = string.Empty;

        [Required]
        [Column("questions_json")]
        public string QuestionsJson { get; set; } = "[]";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;
    }
}
