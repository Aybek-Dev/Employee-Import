/*using System.Text;
using Microsoft.EntityFrameworkCore;
using EmployeeImport.DataBase;
using EmployeeImport.Services;

namespace EmployeeImport.Tests;

public class CsvImportServiceTests
{
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;
        private readonly Mock<ILogger<CsvImportService>> _loggerMock;

        public CsvImportServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "ImportTestDb_" + Guid.NewGuid().ToString())
                .Options;

            _loggerMock = new Mock<ILogger<CsvImportService>>();
        }

        [Fact]
        public async Task ImportCsvAsync_WithValidFile_ShouldImportData()
        {
            // Arrange
            var csvContent = @"Personnel_Records.Payroll_NumberPersonnel_Records.ForenamesPersonnel_Records.SurnamePersonnel_Records.Date_of_BirthPersonnel_Records.TelephonePersonnel_Records.MobilePersonnel_Records.AddressPersonnel_Records.Address_2Personnel_Records.PostcodePersonnel_Records.EMail_HomePersonnel_Records.Start_Date
COOP08,John,William,26/01/1955,12345678,98765423,12 Foreman road,London,GU12 6JW,nomadic20@hotmail.co.uk,18/04/2013
JACK13,Jerry,Jackson,11/5/1974,2050508,6987457,15 Spinney Road,Luton,LU33DF,gerry.jackson@bt.com,18/04/2013";

            var formFile = CreateMockFormFile(csvContent, "employees.csv");

            // Act
            using (var context = new AppDbContext(_dbContextOptions))
            {
                var service = new CsvImportService(context, _loggerMock.Object);
                var result = await service.ImportCsvAsync(formFile);

                // Assert
                Assert.Equal(2, result.ProcessedRows);
                Assert.Equal(2, result.SuccessfulRows);
                Assert.Equal(0, result.FailedRows);
                Assert.Null(result.ErrorMessage);
            }

            // Verify data was saved
            using (var context = new AppDbContext(_dbContextOptions))
            {
                var employees = await context.Employees.ToListAsync();
                Assert.Equal(2, employees.Count);
                
                var employee1 = employees[0];
                Assert.Equal("COOP08", employee1.PayrollNumber);
                Assert.Equal("John", employee1.Forenames);
                Assert.Equal("William", employee1.Surname);
                
                var employee2 = employees[1];
                Assert.Equal("JACK13", employee2.PayrollNumber);
                Assert.Equal("Jerry", employee2.Forenames);
                Assert.Equal("Jackson", employee2.Surname);
            }
        }

        [Fact]
        public async Task ImportCsvAsync_WithNullFile_ShouldReturnError()
        {
            // Arrange
            using var context = new AppDbContext(_dbContextOptions);
            var service = new CsvImportService(context, _loggerMock.Object);

            // Act
            var result = await service.ImportCsvAsync(null);

            // Assert
            Assert.Equal(0, result.ProcessedRows);
            Assert.Equal(0, result.SuccessfulRows);
            Assert.Equal(0, result.FailedRows);
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("Файл не выбран", result.ErrorMessage);
        }

        private IFormFile CreateMockFormFile(string content, string fileName)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var formFile = new Mock<IFormFile>();
            
            formFile.Setup(f => f.Length).Returns(stream.Length);
            formFile.Setup(f => f.FileName).Returns(fileName);
            formFile.Setup(f => f.OpenReadStream()).Returns(stream);
            formFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Callback<Stream, CancellationToken>((s, c) => {
                    stream.Position = 0;
                    stream.CopyTo(s);
                })
                .Returns(Task.CompletedTask);
            
            return formFile.Object;
        }
}*/