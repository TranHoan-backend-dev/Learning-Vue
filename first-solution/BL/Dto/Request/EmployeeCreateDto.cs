using System.ComponentModel.DataAnnotations;

namespace BL.Dto.Request;

public record EmployeeCreateDto(
    [Required, MaxLength(20)] string EmployeeCode,
    [Required, MaxLength(50)] string Username,
    [Required, EmailAddress, MaxLength(100)]
    string Email,
    [Required, MaxLength(100)] string Fullname,
    [Required] int Age,
    [MaxLength(200)] string? Address,
    Guid DepartmentId
);