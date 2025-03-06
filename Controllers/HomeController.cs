using System.Diagnostics;
using EmployeeImport.DataBase;
using EmployeeImport.Models;
using EmployeeImport.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeImport.Controllers;

public class HomeController : Controller
{
   private readonly ILogger<HomeController> _logger;
        private readonly ICsvImportService _csvImportService;
        private readonly AppDbContext _dbContext;
        
        public HomeController(
            ILogger<HomeController> logger,
            ICsvImportService csvImportService,
            AppDbContext dbContext)
        {
            _logger = logger;
            _csvImportService = csvImportService;
            _dbContext = dbContext;
        }
        
        public async Task<IActionResult> Index(string sortField = "Surname", string sortDirection = "asc", string searchString = "")
        {
            ViewData["SortField"] = sortField;
            ViewData["SortDirection"] = sortDirection;
            ViewData["SearchString"] = searchString;
            
            // Получение сотрудников из БД с сортировкой и поиском
            var employees = _dbContext.Employees.AsQueryable();
            
            // Применение фильтра поиска
            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e => 
                    e.Forenames.Contains(searchString) || 
                    e.Surname.Contains(searchString) || 
                    e.PayrollNumber.Contains(searchString) || 
                    e.EmailHome.Contains(searchString));
            }
            
            // Применение сортировки
            employees = sortField.ToLower() switch
            {
                "forenames" => sortDirection == "asc" ? employees.OrderBy(e => e.Forenames) : employees.OrderByDescending(e => e.Forenames),
                "payrollnumber" => sortDirection == "asc" ? employees.OrderBy(e => e.PayrollNumber) : employees.OrderByDescending(e => e.PayrollNumber),
                "dateofbirth" => sortDirection == "asc" ? employees.OrderBy(e => e.DateOfBirth) : employees.OrderByDescending(e => e.DateOfBirth),
                "startdate" => sortDirection == "asc" ? employees.OrderBy(e => e.StartDate) : employees.OrderByDescending(e => e.StartDate),
                _ => sortDirection == "asc" ? employees.OrderBy(e => e.Surname) : employees.OrderByDescending(e => e.Surname)
            };
            
            return View(await employees.ToListAsync());
        }
        
        [HttpPost]
        public async Task<IActionResult> Import()
        {
            var file = Request.Form.Files.FirstOrDefault();
            var result = await _csvImportService.ImportCsvAsync(file);
            
            TempData["ImportResult"] = $"Обработано строк: {result.ProcessedRows}, " +
                $"Успешно импортировано: {result.SuccessfulRows}, " +
                $"Ошибок: {result.FailedRows}" +
                (!string.IsNullOrEmpty(result.ErrorMessage) ? $", {result.ErrorMessage}" : "");
            
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _dbContext.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            
            return View(employee);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(employee);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }
        
        private bool EmployeeExists(int id)
        {
            return _dbContext.Employees.Any(e => e.Id == id);
        }
}