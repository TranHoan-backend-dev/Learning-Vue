using MISA.Common.Model;
using MISA.DL.Base;

namespace MISA.DL.Repository;

public interface ISalaryCompositionDl : IBaseDl<SalaryComposition>
{
    Task<List<Guid>> GetAppliedUnitIdsAsync(Guid id);
    Task InsertAppliedUnitsAsync(Guid componentId, List<Guid> unitIds);
    Task DeleteAppliedUnitsAsync(Guid componentId);
}
