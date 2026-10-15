using ClosedXML.Excel;
using EmployeeSurveyApp.Data;
using EmployeeSurveyApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;

namespace EmployeeSurveyApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // =====================================================
        // EMPLOYEE LOGIN
        // =====================================================

        [HttpGet]
        public IActionResult EmployeeLogin()
        {
            return View(new EmployeeLoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmployeeLogin(EmployeeLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var pernr = model.EmployeeCode.Trim();

            // 1. Verify that employee exists in master_data or employee_surveys
            var employeeRecord = await _context.MasterData
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Pernr == pernr);

            if (employeeRecord == null)
            {
                var fallbackEmployee = await _context.EmployeeSurveys
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.EmployeeCode == pernr);

                if (fallbackEmployee == null)
                {
                    ModelState.AddModelError("EmployeeCode", $"Personnel Number (PERNR: {pernr}) was not found in employee records.");
                    return View(model);
                }
            }

            // 2. Default password check: pernr@123 or {pernr}@123 (e.g., 20001@123 or pernr@123)
            bool isPasswordValid = model.Password.Equals("pernr@123", StringComparison.OrdinalIgnoreCase) ||
                                  model.Password.Equals($"{pernr}@123", StringComparison.OrdinalIgnoreCase);

            if (!isPasswordValid)
            {
                ModelState.AddModelError("Password", $"Incorrect password. The default password is {pernr}@123 or pernr@123");
                return View(model);
            }

            // 3. Successful login -> Redirect to Employee Dashboard
            TempData["SuccessMessage"] = $"Welcome, {employeeRecord?.Name ?? "Employee"}!";
            return RedirectToAction(nameof(EmployeeDashboard), new { pernr = pernr });
        }

        // =====================================================
        // EMPLOYEE DASHBOARD (AFTER EMPLOYEE LOGIN)
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> EmployeeDashboard(string? pernr)
        {
            if (string.IsNullOrWhiteSpace(pernr))
            {
                return RedirectToAction(nameof(EmployeeLogin));
            }

            pernr = pernr.Trim();

            // Load employee information from master_data
            var employee = await _context.MasterData
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Pernr == pernr);

            // Fallback to employee_surveys if needed
            string empName = employee?.Name ?? "";
            string empDept = employee?.Department ?? "";
            string empDesgn = employee?.Designation ?? "";

            if (employee == null)
            {
                var fallback = await _context.EmployeeSurveys
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.EmployeeCode == pernr);
                if (fallback != null)
                {
                    empName = $"{fallback.FirstName} {fallback.LastName}";
                    empDept = fallback.Department;
                    empDesgn = fallback.JobTitle;
                }
            }

            // Fetch active surveys where this PERNR is authorized
            var allSurveys = await _context.SurveyForms
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            var authorizedSurveys = allSurveys
                .Where(s => IsPernrAllowed(s.AllowedPernrs, pernr))
                .ToList();

            // Check which surveys the employee has already submitted
            var submittedSurveyIds = await _context.SurveyResponses
                .AsNoTracking()
                .Where(r => r.Pernr == pernr)
                .Select(r => r.SurveyId)
                .Distinct()
                .ToListAsync();

            ViewBag.Pernr = pernr;
            ViewBag.EmployeeName = !string.IsNullOrWhiteSpace(empName) ? empName : $"Employee {pernr}";
            ViewBag.Department = empDept;
            ViewBag.Designation = empDesgn;
            ViewBag.SubmittedSurveyIds = submittedSurveyIds;

            return View(authorizedSurveys);
        }

        // =====================================================
        // ADMIN LOGIN
        // =====================================================

        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View(new AdminLoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdminLogin(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isValidAdmin = (model.UsernameOrEmail.Equals("admin", StringComparison.OrdinalIgnoreCase) ||
                                 model.UsernameOrEmail.Equals("admin@company.com", StringComparison.OrdinalIgnoreCase)) &&
                                (model.Password == "admin" || model.Password == "Admin@123" || model.Password == "Nanotech$4");

            if (!isValidAdmin)
            {
                ModelState.AddModelError(string.Empty, "Invalid Admin username or password. (Hint: Try username: 'admin' and password: 'admin')");
                return View(model);
            }

            TempData["SuccessMessage"] = "Administrator login successful. Welcome to the Admin Dashboard!";
            return RedirectToAction(nameof(AdminDashboard));
        }

        // =====================================================
        // ADMIN DASHBOARD
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> AdminDashboard()
        {
            var surveys = await _context.SurveyForms
                .AsNoTracking()
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            var responseCounts = await _context.SurveyResponses
                .GroupBy(r => r.SurveyId)
                .Select(g => new { SurveyId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.SurveyId, x => x.Count);

            ViewBag.ResponseCounts = responseCounts;
            ViewBag.TotalSurveys = surveys.Count;

            return View(surveys);
        }

        // =====================================================
        // GOOGLE FORMS SURVEY CREATOR (ADMIN ONLY)
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> CreateSurvey()
        {
            var employees = await _context.MasterData
                .AsNoTracking()
                .OrderBy(m => m.Pernr)
                .ToListAsync();

            ViewBag.Employees = employees;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSurvey(
            string title,
            string? description,
            string allowedPernrs,
            string questionsJson)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                ModelState.AddModelError("title", "Survey title is required.");
            }

            if (string.IsNullOrWhiteSpace(allowedPernrs))
            {
                ModelState.AddModelError("allowedPernrs", "Please specify at least one authorized PERNR.");
            }

            if (string.IsNullOrWhiteSpace(questionsJson) || questionsJson == "[]")
            {
                ModelState.AddModelError("questionsJson", "Please add at least one question to the survey.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Employees = await _context.MasterData.AsNoTracking().OrderBy(m => m.Pernr).ToListAsync();
                ViewBag.TitleValue = title;
                ViewBag.DescriptionValue = description;
                ViewBag.AllowedPernrsValue = allowedPernrs;
                ViewBag.QuestionsJsonValue = questionsJson;
                return View();
            }

            var pernrTokens = allowedPernrs
                .Split(new[] { ',', ';', ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Distinct()
                .ToList();

            var normalizedPernrs = string.Join(", ", pernrTokens);

            var survey = new SurveyForm
            {
                Title = title.Trim(),
                Description = description?.Trim(),
                AllowedPernrs = normalizedPernrs,
                QuestionsJson = questionsJson,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.SurveyForms.Add(survey);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Google Form survey '{survey.Title}' created successfully with {pernrTokens.Count} authorized employee(s)!";
            return RedirectToAction(nameof(AdminDashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSurvey(int id)
        {
            var survey = await _context.SurveyForms.FindAsync(id);
            if (survey != null)
            {
                var responses = await _context.SurveyResponses.Where(r => r.SurveyId == id).ToListAsync();
                _context.SurveyResponses.RemoveRange(responses);
                _context.SurveyForms.Remove(survey);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Survey deleted successfully.";
            }

            return RedirectToAction(nameof(AdminDashboard));
        }

        // =====================================================
        // ATTEND SURVEY (EMPLOYEE - PERNR PROTECTED)
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> AttendSurvey(int id, string? pernr)
        {
            var survey = await _context.SurveyForms
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(pernr))
            {
                ViewBag.IsAuthorized = false;
                ViewBag.Message = "A valid employee PERNR is required to access this survey.";
                return View("SurveyAccessDenied", survey);
            }

            pernr = pernr.Trim();

            // Strict authorization check: Only allowed PERNR can attend
            if (!IsPernrAllowed(survey.AllowedPernrs, pernr))
            {
                ViewBag.IsAuthorized = false;
                ViewBag.EmployeePernr = pernr;
                ViewBag.Message = $"Access Denied: Your employee PERNR ({pernr}) is NOT authorized to attend this survey.";
                return View("SurveyAccessDenied", survey);
            }

            // Check if already submitted
            var existingResponse = await _context.SurveyResponses
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.SurveyId == id && r.Pernr == pernr);

            if (existingResponse != null)
            {
                ViewBag.AlreadySubmitted = true;
                ViewBag.SubmittedAt = existingResponse.SubmittedAt;
                ViewBag.PreviousAnswersJson = existingResponse.AnswersJson;
            }

            var emp = await _context.MasterData.AsNoTracking().FirstOrDefaultAsync(m => m.Pernr == pernr);
            ViewBag.EmployeeName = emp?.Name ?? $"Employee {pernr}";
            ViewBag.EmployeePernr = pernr;

            return View(survey);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitSurveyResponse(int surveyId, string pernr, string answersJson)
        {
            var survey = await _context.SurveyForms.FindAsync(surveyId);
            if (survey == null)
            {
                return NotFound();
            }

            pernr = pernr.Trim();

            // Verify permission
            if (!IsPernrAllowed(survey.AllowedPernrs, pernr))
            {
                return Forbid();
            }

            var emp = await _context.MasterData.AsNoTracking().FirstOrDefaultAsync(m => m.Pernr == pernr);

            // Check if existing response exists for this employee and survey
            var existingResponse = await _context.SurveyResponses
                .FirstOrDefaultAsync(r => r.SurveyId == surveyId && r.Pernr == pernr);

            bool isUpdate = false;
            if (existingResponse != null)
            {
                // UPDATE existing values (do not create a duplicate row)
                existingResponse.AnswersJson = answersJson;
                existingResponse.SubmittedAt = DateTime.UtcNow;
                if (!string.IsNullOrWhiteSpace(emp?.Name))
                {
                    existingResponse.EmployeeName = emp.Name;
                }
                isUpdate = true;
            }
            else
            {
                // INSERT new response
                var response = new SurveyResponse
                {
                    SurveyId = surveyId,
                    Pernr = pernr,
                    EmployeeName = emp?.Name ?? $"Employee {pernr}",
                    AnswersJson = answersJson,
                    SubmittedAt = DateTime.UtcNow
                };
                _context.SurveyResponses.Add(response);
            }

            await _context.SaveChangesAsync();

            ViewBag.SurveyTitle = survey.Title;
            ViewBag.EmployeeName = emp?.Name ?? $"Employee {pernr}";
            ViewBag.EmployeePernr = pernr;
            ViewBag.IsUpdate = isUpdate;

            return View("SurveySubmitted");
        }

        // =====================================================
        // VIEW RESPONSES (ADMIN ONLY)
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> ViewResponses(int? id)
        {
            var allSurveys = await _context.SurveyForms
                .AsNoTracking()
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            if (!allSurveys.Any())
            {
                TempData["ErrorMessage"] = "No surveys found in the system. Create a survey first.";
                return RedirectToAction(nameof(AdminDashboard));
            }

            var survey = id.HasValue && id.Value > 0
                ? allSurveys.FirstOrDefault(s => s.Id == id.Value)
                : allSurveys.First();

            if (survey == null)
            {
                survey = allSurveys.First();
            }

            var responses = await _context.SurveyResponses
                .AsNoTracking()
                .Where(r => r.SurveyId == survey.Id)
                .OrderByDescending(r => r.SubmittedAt)
                .ToListAsync();

            var questions = new List<SurveyQuestionModel>();
            if (!string.IsNullOrWhiteSpace(survey.QuestionsJson))
            {
                try
                {
                    questions = JsonSerializer.Deserialize<List<SurveyQuestionModel>>(survey.QuestionsJson) ?? new();
                }
                catch { }
            }

            var pernrs = responses.Select(r => r.Pernr).Distinct().ToList();
            var employees = await _context.MasterData
                .AsNoTracking()
                .Where(m => pernrs.Contains(m.Pernr))
                .ToDictionaryAsync(m => m.Pernr, m => m);

            ViewBag.Survey = survey;
            ViewBag.AllSurveys = allSurveys;
            ViewBag.Questions = questions;
            ViewBag.Employees = employees;
            return View(responses);
        }

        // =====================================================
        // EXPORT RESPONSES TO EXCEL (.XLSX)
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> DownloadResponsesExcel(int id)
        {
            var survey = await _context.SurveyForms
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
            {
                return NotFound();
            }

            var responses = await _context.SurveyResponses
                .AsNoTracking()
                .Where(r => r.SurveyId == id)
                .OrderBy(r => r.SubmittedAt)
                .ToListAsync();

            // Load questions list
            var questions = new List<SurveyQuestionModel>();
            if (!string.IsNullOrWhiteSpace(survey.QuestionsJson))
            {
                try
                {
                    questions = JsonSerializer.Deserialize<List<SurveyQuestionModel>>(survey.QuestionsJson) ?? new();
                }
                catch
                {
                    // Fallback
                }
            }

            // Load master data for PERNRs to enrich department and designation
            var pernrs = responses.Select(r => r.Pernr).Distinct().ToList();
            var employees = await _context.MasterData
                .AsNoTracking()
                .Where(m => pernrs.Contains(m.Pernr))
                .ToDictionaryAsync(m => m.Pernr, m => m);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Responses");

            // Title Header Row
            worksheet.Cell(1, 1).Value = $"{survey.Title} - Survey Responses";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 14;
            worksheet.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml("#673ab7");

            worksheet.Cell(2, 1).Value = $"Exported on: {DateTime.Now:dd-MMM-yyyy HH:mm:ss} | Total Responses: {responses.Count}";
            worksheet.Cell(2, 1).Style.Font.Italic = true;
            worksheet.Cell(2, 1).Style.Font.FontSize = 10;
            worksheet.Cell(2, 1).Style.Font.FontColor = XLColor.Gray;

            // Table Headers (Row 4)
            int headerRow = 4;
            worksheet.Cell(headerRow, 1).Value = "S.No";
            worksheet.Cell(headerRow, 2).Value = "PERNR";
            worksheet.Cell(headerRow, 3).Value = "Employee Name";
            worksheet.Cell(headerRow, 4).Value = "Department";
            worksheet.Cell(headerRow, 5).Value = "Designation";
            worksheet.Cell(headerRow, 6).Value = "Submitted At";

            int col = 7;
            for (int i = 0; i < questions.Count; i++)
            {
                worksheet.Cell(headerRow, col).Value = $"Q{i + 1}: {questions[i].Title}";
                col++;
            }

            // Header Style
            var headerRange = worksheet.Range(headerRow, 1, headerRow, col - 1);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#673ab7");
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Data Rows
            int currentRow = headerRow + 1;
            for (int i = 0; i < responses.Count; i++)
            {
                var r = responses[i];
                employees.TryGetValue(r.Pernr, out var emp);

                worksheet.Cell(currentRow, 1).Value = i + 1;
                worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(currentRow, 2).Value = r.Pernr;
                worksheet.Cell(currentRow, 3).Value = r.EmployeeName;
                worksheet.Cell(currentRow, 4).Value = emp?.Department ?? "N/A";
                worksheet.Cell(currentRow, 5).Value = emp?.Designation ?? "N/A";
                worksheet.Cell(currentRow, 6).Value = r.SubmittedAt.ToString("dd-MMM-yyyy HH:mm");

                // Parse Answers
                JsonDocument? answersDoc = null;
                try
                {
                    if (!string.IsNullOrWhiteSpace(r.AnswersJson))
                    {
                        answersDoc = JsonDocument.Parse(r.AnswersJson);
                    }
                }
                catch { }

                int answerCol = 7;
                foreach (var q in questions)
                {
                    string answerText = "";
                    if (answersDoc != null && answersDoc.RootElement.TryGetProperty(q.Id, out var qProp))
                    {
                        if (qProp.TryGetProperty("response", out var respProp))
                        {
                            if (respProp.ValueKind == JsonValueKind.Array)
                            {
                                var arrVals = respProp.EnumerateArray()
                                    .Select(x => x.GetString())
                                    .Where(x => !string.IsNullOrEmpty(x));
                                answerText = string.Join(", ", arrVals);
                            }
                            else
                            {
                                answerText = respProp.GetString() ?? respProp.ToString();
                            }
                        }
                    }

                    worksheet.Cell(currentRow, answerCol).Value = answerText;
                    answerCol++;
                }

                // Zebra striping
                if (i % 2 == 1)
                {
                    var dataRange = worksheet.Range(currentRow, 1, currentRow, col - 1);
                    dataRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#f8f9fa");
                }

                currentRow++;
            }

            // Borders for data rows
            if (responses.Count > 0)
            {
                var allDataRange = worksheet.Range(headerRow + 1, 1, currentRow - 1, col - 1);
                allDataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                allDataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            }

            worksheet.Columns().AdjustToContents(1, 50);

            using var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            memoryStream.Position = 0;

            string safeTitle = string.Concat(survey.Title.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");
            string fileName = $"{safeTitle}_Responses_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

            return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        // Helper to check if a PERNR is in allowed list
        private static bool IsPernrAllowed(string allowedPernrs, string pernr)
        {
            if (string.IsNullOrWhiteSpace(allowedPernrs) || string.IsNullOrWhiteSpace(pernr))
                return false;

            var tokens = allowedPernrs
                .Split(new[] { ',', ';', ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim());

            return tokens.Any(t => t.Equals(pernr, StringComparison.OrdinalIgnoreCase));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
