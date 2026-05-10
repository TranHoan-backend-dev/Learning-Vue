using MISA.BL.DTO.Request;
using MISA.Common.Base;
using MISA.Common.Model.Pageable;

namespace MISA.BL.Base;

public interface IBaseBl<T> where T : BaseModel
{
    Task<PagingData<T>> GetAllAsync(Pageable pageable, FilterRequest request);
    Task<T?> GetByIdAsync(Guid id);
    Task AddAsync(T model);
    Task<int> UpdateAsync(T model, Guid id);
    Task DeleteAsync(List<string> ids);
}