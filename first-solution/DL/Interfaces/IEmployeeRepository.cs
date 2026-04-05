using Model;

namespace DL.Interfaces;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(Guid employeeId);
    Task AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task DeleteAsync(Guid employeeId);
    
    Task<Boolean> IsExistAsync(string employeeCode);
}