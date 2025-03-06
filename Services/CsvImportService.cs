using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using EmployeeImport.DataBase;
using EmployeeImport.Models;
using EmployeeImport.Services.Interfaces;

namespace EmployeeImport.Services;

public class CsvImportService: ICsvImportService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<CsvImportService> _logger;
        
        public CsvImportService(AppDbContext dbContext, ILogger<CsvImportService> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<CsvImportResult> ImportCsvAsync(IFormFile file)
        {
            var result = new CsvImportResult();
            
            if (file == null || file.Length == 0)
            {
                result.ErrorMessage = "Файл не выбран или пуст";
                return result;
            }
            
            try
            {
                // Используем семафор для контроля конкурентного доступа
                using var semaphore = new System.Threading.SemaphoreSlim(1, 1);
                await semaphore.WaitAsync();
                
                try
                {
                    // Чтение содержимого файла
                    using var stream = new MemoryStream();
                    await file.CopyToAsync(stream);
                    stream.Position = 0;
                    
                    using var reader = new StreamReader(stream, Encoding.UTF8);
                    var fileContent = await reader.ReadToEndAsync();
                    
                    // Разбор данных CSV
                    var employees = ParseCsvContent(fileContent);
                    result.ProcessedRows = employees.Count;
                    result.SuccessfulRows = employees.Count;
                    
                    // Сохранение данных в базу
                    await _dbContext.Employees.AddRangeAsync(employees);
                    await _dbContext.SaveChangesAsync();
                }
                finally
                {
                    semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при импорте CSV файла");
                result.ErrorMessage = $"Ошибка при импорте: {ex.Message}";
                result.FailedRows = result.ProcessedRows - result.SuccessfulRows;
            }
            
            return result;
        }
        
        private List<Employee> ParseCsvContent(string content)
        {
            var employees = new List<Employee>();
            
            // Разделение на строки
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            // Получение заголовков
            var headers = lines[0].Split(new[] { ',' }, StringSplitOptions.None);
            
            // Обработка строк данных
            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    var employee = ParseEmployeeRow(lines[i]);
                    if (employee != null)
                    {
                        employees.Add(employee);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Ошибка при обработке строки {i + 1}");
                }
            }
            
            return employees;
        }
        
        private Employee ParseEmployeeRow(string row)
        {
            // Используем регулярное выражение для корректной обработки CSV
            // Это позволяет обрабатывать поля, содержащие запятые внутри кавычек
            var pattern = @"(?:^|,)(?=[^""]|("")?)""?((?(1)[^""]*|[^,""]*))""?(?=,|$)";
            var matches = Regex.Matches(row, pattern);
            
            if (matches.Count == 0)
            {
                // Если регулярное выражение не сработало, используем простое разделение
                var values = row.Split(',');
                return CreateEmployeeFromValues(values);
            }
            
            var fieldValues = matches.Cast<Match>()
                .Select(m => m.Groups[2].Value.Trim())
                .ToArray();
                
            return CreateEmployeeFromValues(fieldValues);
        }
        
        private Employee CreateEmployeeFromValues(string[] values)
        {
            if (values.Length < 11) // Минимальное количество полей
            {
                return null;
            }
            
            var employee = new Employee
            {
                PayrollNumber = values[0],
                Forenames = values[1],
                Surname = values[2],
                Telephone = values[4],
                Mobile = values[5],
                Address = values[6],
                Address2 = values[7],
                Postcode = values[8],
                EmailHome = values[9]
            };
            
            // Обработка даты рождения
            if (DateTime.TryParseExact(values[3], new[] { "dd/MM/yyyy", "d/M/yyyy", "dd/M/yyyy", "d/MM/yyyy" }, 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateOfBirth))
            {
                employee.DateOfBirth = dateOfBirth;
            }
            
            // Обработка даты начала работы
            if (DateTime.TryParseExact(values[10], new[] { "dd/MM/yyyy", "d/M/yyyy", "dd/M/yyyy", "d/MM/yyyy" }, 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var startDate))
            {
                employee.StartDate = startDate;
            }
            
            return employee;
        }
    }