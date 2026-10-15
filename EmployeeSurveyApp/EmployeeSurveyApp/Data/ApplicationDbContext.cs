using EmployeeSurveyApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSurveyApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<EmployeeSurvey> EmployeeSurveys { get; set; }
        public DbSet<MasterData> MasterData { get; set; }
        public DbSet<SurveyForm> SurveyForms { get; set; }
        public DbSet<SurveyResponse> SurveyResponses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmployeeSurvey>(entity =>
            {
                entity.ToTable("employee_surveys");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.EmployeeCode)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.FirstName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.WorkEmail)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.Department)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(e => e.JobTitle)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(e => e.AnnualSalary)
                    .HasPrecision(18, 2);

                entity.Property(e => e.YearsOfExperience)
                    .HasPrecision(5, 2);

                entity.HasIndex(e => e.EmployeeCode)
                    .IsUnique();

                entity.HasIndex(e => e.WorkEmail);

                entity.HasIndex(e => e.Department);

                entity.HasIndex(e => e.EmploymentStatus);

                entity.HasIndex(e => e.SurveyDate);
            });
        }
    }
}