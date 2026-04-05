namespace BL.Dto.Request;

public class EmployeeDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public int Age { get; set; }
    public string? Address { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
}