using BL.Dto.Request;
using BL.Service;
using DL.Interfaces;
using Microsoft.Extensions.Logging;
using Model;
using DL.Exception;

namespace BL.Impl;

public class EmployeeService(
    IEmployeeRepository repository,
    IDepartmentRepository departmentRepository,
    ILogger<EmployeeService> log
) : IEmployeeService
{
    public async Task CreateNewEmployee(EmployeeCreateDto dto)
    {
        log.Log(LogLevel.Information, "Creating new employee");
        if (await repository.IsExistAsync(dto.EmployeeCode))
        {
            throw new AlreadyExistsException($"Employee with code {dto.EmployeeCode} already exists");
        }

        var department = await departmentRepository.GetByIdAsync(dto.DepartmentId);
        if (department == null)
        {
            throw new NotFoundException("Department not found");
        }
        await repository.AddAsync(new Employee
        {
            EmployeeCode = dto.EmployeeCode,
            Username = dto.Username,
            Email = dto.Email,
            Fullname = dto.Fullname,
            Age = dto.Age,
            Address = dto.Address,
            Department = department
        });
        log.Log(LogLevel.Information, "Created new employee {EmployeeCode}", dto.EmployeeCode);
    }
}