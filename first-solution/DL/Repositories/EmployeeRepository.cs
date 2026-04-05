using DL.Context;
using DL.Exception;
using DL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model;

namespace DL.Repositories;

public class EmployeeRepository(
    AppDbContext context,
    ILogger<EmployeeRepository> log
) : IEmployeeRepository
{
    public async Task<List<Employee>> GetAllAsync()
    {
        return await context.Employees
            .Include(e => e.Department)
            .ToListAsync();
    }

    public Task<Employee?> GetByIdAsync(Guid employeeId)
    {
        return context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
    }

    /**
     * Thêm 1 bản ghi vào DB. Bản ghi mới không đươợc trùng lặp emp code, username, email
     */
    public async Task AddAsync(Employee employee)
    {
        log.Log(LogLevel.Information, "Adding employee to database: {EmployeeCode}", employee.EmployeeCode);
        var status = await context.Employees
            .AnyAsync(e =>
                e.EmployeeCode == employee.EmployeeCode ||
                e.Username.ToLower() == employee.Username.ToLower() ||
                e.Email == employee.Email
            );
        log.Log(LogLevel.Debug, "Check status for {EmployeeCode}: {Status}", employee.EmployeeCode, status);
        if (status)
        {
            throw new AlreadyExistsException("Employee code or username or email already exists");
        }

        context.Employees.Add(employee);
        await context.SaveChangesAsync();
        log.Log(LogLevel.Information, "Employee {EmployeeCode} added successfully", employee.EmployeeCode);
    }

    public async Task UpdateAsync(Employee employee)
    {
        var emp = context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeCode == employee.EmployeeCode);
        if (emp == null)
        {
            throw new NotFoundException($"Employee not found with code {employee.EmployeeCode}");
        }

        context.Employees.Update(employee);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid employeeId)
    {
        var employee = context.Employees
            .FirstOrDefault(e => e.EmployeeId == employeeId);
        if (employee == null)
        {
            throw new NotFoundException($"Employee not found {employeeId}");
        }

        context.Employees.Remove(employee);
        await context.SaveChangesAsync();
    }

    public async Task<bool> IsExistAsync(string employeeCode)
    {
        log.Log(LogLevel.Information, "Checking if employee exists: {EmployeeCode}", employeeCode);
        var employee = await context.Employees
            .FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode);
        var exists = employee != null;
        log.Log(LogLevel.Information, "Employee {EmployeeCode} exists: {Exists}", employeeCode, exists);
        return exists;
    }
}