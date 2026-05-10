using Dapper;
using MISA.Common.Base;

namespace MISA.DL.Base;

public interface IBaseDl<T> where T : BaseModel
{
    Task<IEnumerable<T>> GetPagedDataListAsync(DynamicParameters parameters, string command);
    Task<T?> GetByIdAsync(Guid id);
    Task CreateAsync(IEnumerable<T> entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(List<string> id);
    // Task<object> ExecuteCommandText(string commandText, DynamicParameters parameters);
}