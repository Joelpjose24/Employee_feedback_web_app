using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeSurveyApp.Models
{
    [Table("survey_responses")]
    public class SurveyResponse
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("survey_id")]
        public int SurveyId { get; set; }

        [Required]
        [Column("pernr")]
        public string Pernr { get; set; } = string.Empty;

        [Column("employee_name")]
        public string? EmployeeName { get; set; }

        [Required]
        [Column("answers_json")]
        public string AnswersJson { get; set; } = "{}";

        [Column("submitted_at")]
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
