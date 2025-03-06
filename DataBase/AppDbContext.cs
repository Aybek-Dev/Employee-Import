using EmployeeImport.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeImport.DataBase;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
        
    public DbSet<Employee> Employees { get; set; }
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Индекс для ускорения сортировки по фамилии
        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Surname);
                
        base.OnModelCreating(modelBuilder);
    }
}