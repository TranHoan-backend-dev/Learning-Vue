using Dapper;
using MISA.Common.Base;

namespace MISA.DL.Base;

public interface IBaseDl<T> where T : BaseModel
{
    Task<(IEnumerable<T> Data, int Pageable)> GetPagedDataListAsync<T>(
        DynamicParameters parametersData, string commandData,
        DynamicParameters parametersPageable, string commandPageable
    );

    Task<T?> GetByIdAsync<T>(
        Guid? id, bool includeJoin,
        string? command, DynamicParameters? parameters
    );

    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(List<string> id);
    Task<int> CountTotalElements(DynamicParameters parameters, string command);
}