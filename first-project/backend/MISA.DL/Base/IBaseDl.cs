using Dapper;
using MISA.Common.Base;

namespace MISA.DL.Base;

public interface IBaseDl<T> where T : BaseModel
{
    Task<IEnumerable<T>?> GetAllAsync(BaseModel model);
    Task<IEnumerable<T>?> GetPagedAsync(DynamicParameters parameters, string command);
    Task<T?> GetByIdAsync(Guid id);

    Task CreateAsync(T entity);
    Task UpdateAsync(T entity, Guid id);
    Task DeleteAsync(T entity, Guid id);
    Task<bool> CheckExisting(T entity);
    Task<object> ExecuteCommandText(string commandText, DynamicParameters parameters);

    Task<int> CountTotalElements();
}