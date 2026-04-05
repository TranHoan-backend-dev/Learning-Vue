using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Model;

[Index(nameof(Name), IsUnique = true)]
[Index(nameof(PhoneNumber), IsUnique = true)]
public class Department
{
    [Key] public Guid DepartmentId { get; set; }

    [Required] [MaxLength(200)] public required string Name { get; set; }

    [MaxLength(10)] public string? PhoneNumber { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public void AddEmployee(Employee employee)
    {
        if (!Employees.Any(e => e.EmployeeId == employee.EmployeeId))
        {
            Employees.Add(employee);
        }
    }

    public void RemoveEmployee(string id)
    {
        var employee = Employees.FirstOrDefault(e => e.EmployeeId == Guid.Parse(id));
        if (employee != null)
        {
            Employees.Remove(employee);
        }
    }

    public bool EmployeeExists(string id)
    {
        return Employees.Any(e => e.EmployeeId == Guid.Parse(id));
    }
}