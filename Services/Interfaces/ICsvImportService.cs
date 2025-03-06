using EmployeeImport.Models;

namespace EmployeeImport.Services.Interfaces;

public interface ICsvImportService
{
    Task<CsvImportResult> ImportCsvAsync(IFormFile file);
}