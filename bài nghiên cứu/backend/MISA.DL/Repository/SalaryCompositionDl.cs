using Dapper;
using Microsoft.Extensions.Logging;
using MISA.Common.Model;
using MISA.DL.Context;

namespace MISA.DL.Repository;

public class SalaryCompositionDl(
    DbContext context,
    ILogger<SalaryCompositionDl> log
) : BaseDl<SalaryComposition>(context, log), ISalaryCompositionDl
{
    private readonly DbContext _context = context;

    /// <summary>
    /// Lấy ra danh sách các id của đơn vị áp dụng sở hữu thành phần lương này
    /// <param name="id">Id của Salary composition</param>
    /// </summary>
    public async Task<List<Guid>> GetAppliedUnitIdsAsync(Guid id)
    {
        await using var conn = _context.GetConnection();
        var sql = "SELECT organization_id FROM pa_salary_composition_organization WHERE salary_component_id = @Id";
        var unitIds = await conn.QueryAsync<Guid>(sql, new { Id = id });
        return unitIds.ToList();
    }

    /// <summary>
    /// Thêm các Đơn vị áp dụng vào 1 thành phần lương
    /// <param name="componentId">Id của Salary composition</param>
    /// <param name="unitIds">Danh sách các id của Đơn vị áp dụng</param>
    /// </summary>
    public async Task InsertAppliedUnitsAsync(Guid componentId, List<Guid> unitIds)
    {
        if (unitIds == null || !unitIds.Any()) return;
        
        await using var conn = _context.GetConnection();
        await conn.OpenAsync();
        await using var transaction = await conn.BeginTransactionAsync();
        try
        {
            var sql = "INSERT INTO pa_salary_composition_organization (salary_component_id, organization_id) VALUES (@ComponentId, @UnitId)";
            var parameters = unitIds.Select(unitId => new { ComponentId = componentId, UnitId = unitId });
            await conn.ExecuteAsync(sql, parameters, transaction);
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            log.LogError(ex, "Failed to insert applied organization units for component {ComponentId}", componentId);
            throw;
        }
    }

    /// <summary>
    /// Xóa các Đơn vị áp dụng bằng id của salary composition
    /// <param name="componentId">Id của Salary composition</param>
    /// </summary
    public async Task DeleteAppliedUnitsAsync(Guid componentId)
    {
        await using var conn = _context.GetConnection();
        await conn.OpenAsync();
        await using var transaction = await conn.BeginTransactionAsync();
        try
        {
            var sql = "DELETE FROM pa_salary_composition_organization WHERE salary_component_id = @Id";
            await conn.ExecuteAsync(sql, new { Id = componentId }, transaction);
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            log.LogError(ex, "Failed to delete applied organization units for component {ComponentId}", componentId);
            throw;
        }
    }
}
