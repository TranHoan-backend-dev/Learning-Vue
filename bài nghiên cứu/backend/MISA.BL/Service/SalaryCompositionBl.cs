using Microsoft.Extensions.Logging;
using MISA.BL.DTO.Request;
using MISA.Common.Model;
using MISA.Common.Model.Pageable;
using MISA.DL.Base;

namespace MISA.BL.Service;

public class SalaryCompositionBl(IBaseDl<SalaryComposition> baseDl, ILogger<SalaryComposition> log) 
    : BaseBl<SalaryComposition>(baseDl, log), ISalaryCompositionBl
{
    protected override string GetJoinColumns() => "t2.organization_name AS AppliedUnitName, t3.salary_component_system_name AS SalaryComponentSystemName";

    protected override string GetJoinSql() => 
        "LEFT JOIN pa_organization t2 ON t1.applied_unit_id = t2.organization_id " +
        "LEFT JOIN pa_salary_composition_system t3 ON t1.salary_component_system_id = t3.salary_component_system_id";
}
