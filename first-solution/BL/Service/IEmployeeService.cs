using BL.Dto.Request;

namespace BL.Service;

public interface IEmployeeService
{
    Task CreateNewEmployee(EmployeeCreateDto dto);
}