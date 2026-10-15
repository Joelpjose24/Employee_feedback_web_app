using EmployeeSurveyApp.Data;
using EmployeeSurveyApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeSurveyApp.Services
{
    public class EmployeeSurveyService : IEmployeeSurveyService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeSurveyService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET ALL
        // =====================================================

        public async Task<List<EmployeeSurvey>> GetAllAsync()
        {
            return await _context.EmployeeSurveys
                .AsNoTracking()
                .OrderByDescending(x => x.SurveyDate)
                .ThenBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();
        }

        // =====================================================
        // SEARCH / FILTER
        // =====================================================

        public async Task<List<EmployeeSurvey>> SearchAsync(
            string? search,
            string? department,
            string? employmentStatus)
        {
            IQueryable<EmployeeSurvey> query =
                _context.EmployeeSurveys
                    .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(x =>
                    x.EmployeeCode.Contains(search) ||
                    x.FirstName.Contains(search) ||
                    x.LastName.Contains(search) ||
                    x.WorkEmail.Contains(search) ||
                    x.JobTitle.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(department))
            {
                query = query.Where(x =>
                    x.Department == department);
            }

            if (!string.IsNullOrWhiteSpace(employmentStatus))
            {
                query = query.Where(x =>
                    x.EmploymentStatus ==
                    employmentStatus);
            }

            return await query
                .OrderByDescending(x => x.SurveyDate)
                .ThenBy(x => x.LastName)
                .ToListAsync();
        }

        // =====================================================
        // GET BY ID
        // =====================================================

        public async Task<EmployeeSurvey?> GetByIdAsync(int id)
        {
            return await _context.EmployeeSurveys
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // =====================================================
        // CREATE
        // =====================================================

        public async Task<EmployeeSurvey> CreateAsync(
            EmployeeSurvey survey)
        {
            survey.CreatedDate = DateTime.UtcNow;

            survey.UpdatedDate = null;

            _context.EmployeeSurveys.Add(survey);

            await _context.SaveChangesAsync();

            return survey;
        }

        // =====================================================
        // UPDATE
        // =====================================================

        public async Task<bool> UpdateAsync(
            EmployeeSurvey survey)
        {
            var existing =
                await _context.EmployeeSurveys
                    .FirstOrDefaultAsync(
                        x => x.Id == survey.Id);

            if (existing == null)
            {
                return false;
            }

            existing.EmployeeCode = survey.EmployeeCode;

            existing.FirstName = survey.FirstName;
            existing.MiddleName = survey.MiddleName;
            existing.LastName = survey.LastName;
            existing.PreferredName = survey.PreferredName;

            existing.WorkEmail = survey.WorkEmail;
            existing.PersonalEmail = survey.PersonalEmail;
            existing.Phone = survey.Phone;

            existing.DateOfBirth = survey.DateOfBirth;
            existing.Gender = survey.Gender;

            existing.Department = survey.Department;
            existing.JobTitle = survey.JobTitle;
            existing.EmploymentType = survey.EmploymentType;
            existing.EmploymentStatus = survey.EmploymentStatus;

            existing.DateJoined = survey.DateJoined;
            existing.WorkLocation = survey.WorkLocation;
            existing.WorkMode = survey.WorkMode;
            existing.ManagerName = survey.ManagerName;

            existing.YearsOfExperience =
                survey.YearsOfExperience;

            existing.EducationLevel =
                survey.EducationLevel;

            existing.AnnualSalary =
                survey.AnnualSalary;

            existing.SalaryCurrency =
                survey.SalaryCurrency;

            existing.BenefitsSatisfaction =
                survey.BenefitsSatisfaction;

            existing.OverallSatisfaction =
                survey.OverallSatisfaction;

            existing.JobSatisfaction =
                survey.JobSatisfaction;

            existing.ManagementSatisfaction =
                survey.ManagementSatisfaction;

            existing.WorkLifeBalance =
                survey.WorkLifeBalance;

            existing.CareerGrowth =
                survey.CareerGrowth;

            existing.LearningDevelopment =
                survey.LearningDevelopment;

            existing.Recognition =
                survey.Recognition;

            existing.CompensationSatisfaction =
                survey.CompensationSatisfaction;

            existing.WorkplaceCulture =
                survey.WorkplaceCulture;

            existing.RemoteWorkSatisfaction =
                survey.RemoteWorkSatisfaction;

            existing.TeamCollaboration =
                survey.TeamCollaboration;

            existing.Communication =
                survey.Communication;

            existing.Workload =
                survey.Workload;

            existing.TechnologyToolsSatisfaction =
                survey.TechnologyToolsSatisfaction;

            existing.Strengths =
                survey.Strengths;

            existing.Challenges =
                survey.Challenges;

            existing.ImprovementSuggestions =
                survey.ImprovementSuggestions;

            existing.CareerGoals =
                survey.CareerGoals;

            existing.AdditionalComments =
                survey.AdditionalComments;

            existing.SurveyDate =
                survey.SurveyDate;

            existing.SurveyPeriod =
                survey.SurveyPeriod;

            existing.SurveyStatus =
                survey.SurveyStatus;

            existing.IsAnonymous =
                survey.IsAnonymous;

            existing.UpdatedDate =
                DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        // =====================================================
        // DELETE
        // =====================================================

        public async Task<bool> DeleteAsync(int id)
        {
            var survey =
                await _context.EmployeeSurveys
                    .FirstOrDefaultAsync(
                        x => x.Id == id);

            if (survey == null)
            {
                return false;
            }

            _context.EmployeeSurveys.Remove(survey);

            await _context.SaveChangesAsync();

            return true;
        }

        // =====================================================
        // EXISTS
        // =====================================================

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.EmployeeSurveys
                .AnyAsync(x => x.Id == id);
        }
    }
}