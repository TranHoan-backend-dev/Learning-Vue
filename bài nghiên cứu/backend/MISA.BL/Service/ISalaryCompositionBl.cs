using MISA.BL.Base;
using MISA.BL.DTO.Request;
using MISA.Common.Model;
using MISA.Common.Model.Pageable;

namespace MISA.BL.Service;

public interface ISalaryCompositionBl : IBaseBl<SalaryComposition>
{
    new Task<PagingData<SalaryCompositionDto>> GetAllAsync(Pageable pageable, FilterRequest request);
    new Task<SalaryCompositionDto?> GetByIdAsync(Guid id);
}
