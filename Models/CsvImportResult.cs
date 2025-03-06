namespace EmployeeImport.Models;

public class CsvImportResult
{
    public int ProcessedRows { get; set; }
    public int SuccessfulRows { get; set; }
    public int FailedRows { get; set; }
    public string ErrorMessage { get; set; }
}