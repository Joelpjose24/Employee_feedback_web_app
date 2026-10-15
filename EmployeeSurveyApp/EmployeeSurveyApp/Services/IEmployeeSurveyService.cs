using EmployeeSurveyApp.Models;

namespace EmployeeSurveyApp.Services
{
    public interface IEmployeeSurveyService
    {
        Task<List<EmployeeSurvey>> GetAllAsync();

        Task<List<EmployeeSurvey>> SearchAsync(
            string? search,
            string? department,
            string? employmentStatus);

        Task<EmployeeSurvey?> GetByIdAsync(int id);

        Task<EmployeeSurvey> CreateAsync(
            EmployeeSurvey survey);

        Task<bool> UpdateAsync(
            EmployeeSurvey survey);

        Task<bool> DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}