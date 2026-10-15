using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeSurveyApp.Models
{
    [Table("master_data")]
    public class MasterData
    {
        [Key]
        [Column("pernr")]
        public string Pernr { get; set; } = string.Empty;

        [Column("name")]
        public string? Name { get; set; }

        [Column("desgn")]
        public string? Designation { get; set; }

        [Column("comp")]
        public string? Company { get; set; }

        [Column("dept")]
        public string? Department { get; set; }

        [Column("pers_area")]
        public string? PersonnelArea { get; set; }

        [Column("gender")]
        public string? Gender { get; set; }
    }
}
