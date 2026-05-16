using MISA.BL.Base;
using MISA.BL.DTO.Request;
using MISA.Common.Model;
using MISA.Common.Model.Pageable;

namespace MISA.BL.Service;

public interface ISalaryCompositionBl : IBaseBl<SalaryComposition>
{
    Task<PagingData<SalaryComposition>> GetAllUsedCompositions(FilterRequest request, Pageable pageable, bool flag);
}
