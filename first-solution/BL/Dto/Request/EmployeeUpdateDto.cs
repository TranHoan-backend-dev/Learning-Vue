using System.ComponentModel.DataAnnotations;

namespace BL.Dto.Request;

public class EmployeeUpdateDto
{
    [Required]
    public Guid EmployeeId { get; set; }

    [Required]
    [MaxLength(20)]
    public required string EmployeeCode { get; set; }
    
    [Required]
    [MaxLength(50)]
    public required string Username { get; set; }
    
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public required string Email { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Fullname { get; set; }
    
    [Required]
    public int Age { get; set; }
    
    [MaxLength(200)]
    public string? Address { get; set; }
    
    public Guid? DepartmentId { get; set; }
}
