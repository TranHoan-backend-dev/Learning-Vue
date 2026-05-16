using Dapper;
using Microsoft.Extensions.Logging;
using MISA.BL.DTO.Request;
using MISA.Common.Enum;
using MISA.Common.Model;
using MISA.Common.Model.Pageable;
using MISA.DL.Base;

namespace MISA.BL.Service;

public class SalaryCompositionBl(
    IBaseDl<SalaryComposition> baseDl,
    ILogger<SalaryComposition> log
)
    : BaseBl<SalaryComposition>(baseDl, log), ISalaryCompositionBl
{
    protected override string GetJoinColumns() =>
        "t2.organization_name AS AppliedUnitName, t3.salary_component_system_name AS SalaryComponentSystemName";

    protected override string GetJoinSql() =>
        "LEFT JOIN pa_organization t2 ON t1.applied_unit_id = t2.organization_id " +
        "LEFT JOIN pa_salary_composition_system t3 ON t1.salary_component_system_id = t3.salary_component_system_id";

    public async Task<PagingData<SalaryComposition>> GetAllUsedCompositions(
        FilterRequest request, Pageable pageable,
        bool flag
    )
    {
        log.LogInformation("Get all used salary compositions");
        var filter = request.ColumnFilters?.ToList() ?? new List<FilterRequest.FilterColumn>();

        filter.Add(new FilterRequest.FilterColumn()
        {
            Column = "is_used",
            DataType = DataType.String,
            FilterType = FilterType.Equals,
            Value = flag ? "1" : "0"
        });

        request.ColumnFilters = filter;

        var parameters = new DynamicParameters();
        var (query, pagination) = BuildQueryStringWithCondition(
            request,
            pageable.PageIndex,
            (long)pageable.PageSize,
            ref parameters
        );

        var data = await baseDl.GetPagedDataListAsync<SalaryComposition>(
            parameters, query,
            parameters, pagination
        );

        return new PagingData<SalaryComposition>
        {
            Data = data.Data,
            Pageable = new Pageable
            {
                PageIndex = pageable.PageIndex,
                PageSize = pageable.PageSize,
                TotalElements = data.Pageable
            }
        };
    }
}