using System.ComponentModel.DataAnnotations;

namespace EmployeeImport.Models;

public class Employee
{
    [Key]
    public int Id { get; set; }
        
    [Display(Name = "Табельный номер")]
    [MaxLength(20)]
    public string PayrollNumber { get; set; }
        
    [Display(Name = "Имя")]
    [Required, MaxLength(100)]
    public string Forenames { get; set; }
        
    [Display(Name = "Фамилия")]
    [Required, MaxLength(100)]
    public string Surname { get; set; }
        
    [Display(Name = "Дата рождения")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }
        
    [Display(Name = "Телефон")]
    [MaxLength(20)]
    public string Telephone { get; set; }
        
    [Display(Name = "Мобильный")]
    [MaxLength(20)]
    public string Mobile { get; set; }
        
    [Display(Name = "Адрес")]
    [MaxLength(200)]
    public string Address { get; set; }
        
    [Display(Name = "Адрес 2")]
    [MaxLength(200)]
    public string Address2 { get; set; }
        
    [Display(Name = "Почтовый индекс")]
    [MaxLength(20)]
    public string Postcode { get; set; }
        
    [Display(Name = "Email")]
    [MaxLength(100)]
    public string EmailHome { get; set; }
        
    [Display(Name = "Дата начала работы")]
    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }
}