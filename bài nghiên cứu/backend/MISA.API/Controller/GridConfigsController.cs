using Microsoft.AspNetCore.Mvc;
using MISA.BL.Base;
using MISA.BL.DTO.Request;
using MISA.Common.Model;
using MISA.Common.Model.Pageable;

namespace MISA.API.Controller;

[ApiController]
[Route("api/v1/[controller]")]
public class GridConfigsController(IBaseBl<GridConfig> baseBl) : ControllerBase
{
    /// <summary>
    /// Lấy ra thông tin chi tiết của 1 grid
    /// </summary>
    /// <param name="gridId">Id của grid</param>
    /// <returns></returns>
    [HttpGet("{gridId}")]
    public async Task<IActionResult> GetByGridIdAsync(string gridId)
    {
        var filter = new FilterRequest 
        { 
            ColumnFilters = new List<FilterRequest.FilterColumn> 
            { 
                new() { Column = "grid_id", Value = gridId } 
            } 
        };
        var result = await baseBl.GetAllAsync(new Pageable { PageSize = 1000 }, filter);
        return Ok(result.Data);
    }

    /// <summary>
    /// Cập nhật kích cỡ cột
    /// </summary>
    /// <param name="configs">Danh sách các cột đã được cập nhật</param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> UpdateRangeAsync([FromBody] List<GridConfig> configs)
    {
        foreach (var config in configs)
        {
            await baseBl.UpdateAsync(config, config.GridConfigId);
        }
        return Ok();
    }
}
