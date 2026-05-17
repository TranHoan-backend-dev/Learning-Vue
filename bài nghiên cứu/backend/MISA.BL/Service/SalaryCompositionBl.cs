using Dapper;
using Microsoft.Extensions.Logging;
using MISA.BL.DTO.Request;
using MISA.Common.Enum;
using MISA.Common.Model;
using MISA.Common.Model.Pageable;
using MISA.DL.Repository;

namespace MISA.BL.Service;

public class SalaryCompositionBl(
    ISalaryCompositionDl salaryCompositionDl,
    ILogger<SalaryComposition> log
)
    : BaseBl<SalaryComposition>(salaryCompositionDl, log), ISalaryCompositionBl
{
    private readonly ISalaryCompositionDl _salaryCompositionDl = salaryCompositionDl;

    protected override string GetJoinColumns() =>
        "t3.salary_component_system_name AS SalaryComponentSystemName, " +
        "(SELECT GROUP_CONCAT(o.organization_name SEPARATOR ', ') " +
        " FROM pa_salary_composition_organization sco " +
        " JOIN pa_organization o ON sco.organization_id = o.organization_id " +
        " WHERE sco.salary_component_id = t1.salary_component_id) AS AppliedUnitName";

    protected override string GetJoinSql() =>
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

        var data = await _salaryCompositionDl.GetPagedDataListAsync<SalaryComposition>(
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

    public override async Task AddAsync(SalaryComposition model)
    {
        await base.AddAsync(model);
        if (model.AppliedUnitIds != null && model.AppliedUnitIds.Any())
        {
            await _salaryCompositionDl.InsertAppliedUnitsAsync(model.SalaryComponentId, model.AppliedUnitIds);
        }
    }

    public override async Task<int> UpdateAsync(SalaryComposition model, Guid id)
    {
        var result = await base.UpdateAsync(model, id);
        // Delete existing relations
        await _salaryCompositionDl.DeleteAppliedUnitsAsync(id);
        
        // Insert new relations
        if (model.AppliedUnitIds != null && model.AppliedUnitIds.Any())
        {
            await _salaryCompositionDl.InsertAppliedUnitsAsync(id, model.AppliedUnitIds);
        }
        return result;
    }

    /// <summary>
    /// Ghi đè logic của class cha để lấy thông tin chi tiết kèm danh sách đơn vị áp dụng.
    /// </summary>
    public override async Task<SalaryComposition?> GetByIdAsync(Guid id)
    {
        var result = await base.GetByIdAsync(id);
        if (result != null)
        {
            result.AppliedUnitIds = await _salaryCompositionDl.GetAppliedUnitIdsAsync(id);
        }
        return result;
    }
}