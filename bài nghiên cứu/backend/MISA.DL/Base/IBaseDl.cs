using Dapper;
using MISA.Common.Base;

namespace MISA.DL.Base;

public interface IBaseDl<T> where T : BaseModel
{
    Task<IEnumerable<TReturn>> GetPagedDataListAsync<TReturn>(DynamicParameters parameters, string command);
    Task<TReturn?> GetByIdAsync<TReturn>(Guid id);
    Task CreateAsync(IEnumerable<T> entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(List<string> id);
    Task<int> CountTotalElements(DynamicParameters parameters, string command);
}