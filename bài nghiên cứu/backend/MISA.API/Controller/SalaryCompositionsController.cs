using Microsoft.AspNetCore.Mvc;
using MISA.BL.DTO.Request;
using MISA.BL.Service;
using MISA.Common.Enum;
using MISA.Common.Model;
using MISA.Common.Model.Pageable;

namespace MISA.API.Controller;

[ApiController]
[Route("api/v1/[controller]")]
public class SalaryCompositionsController(ISalaryCompositionBl baseBl) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách bản ghi phân trang và lọc
    /// </summary>
    /// <param name="pageable">Thông tin phân trang</param>
    /// <param name="request">Điều kiện lọc</param>
    /// <returns>Danh sách bản ghi</returns>
    [HttpPost("filter")]
    public async Task<IActionResult> GetAllAsync([FromQuery] Pageable pageable, [FromBody] FilterRequest request)
    {
        var result = await baseBl.GetAllAsync(pageable, request);
        return Ok(result);
    }

    /// <summary>
    /// Lấy bản ghi theo ID
    /// </summary>
    /// <param name="id">ID bản ghi</param>
    /// <returns>Bản ghi tương ứng</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await baseBl.GetByIdAsync(id);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Thêm mới bản ghi
    /// </summary>
    /// <param name="request">Dữ liệu thêm mới</param>
    /// <returns>Kết quả thêm mới</returns>
    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] SalaryComposition request)
    {
        await baseBl.AddAsync(request);
        return StatusCode((int)ApiStatusCode.Created, request.SalaryComponentId);
    }

    /// <summary>
    /// Cập nhật bản ghi
    /// </summary>
    /// <param name="id">ID bản ghi cần cập nhật</param>
    /// <param name="request">Dữ liệu cập nhật</param>
    /// <returns>Kết quả cập nhật</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] SalaryComposition request)
    {
        var result = await baseBl.UpdateAsync(request, id);
        return Ok(result);
    }

    /// <summary>
    /// Xóa nhiều bản ghi
    /// </summary>
    /// <param name="ids">Danh sách ID các bản ghi cần xóa</param>
    /// <returns>Kết quả xóa</returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteAsync([FromBody] List<string> ids)
    {
        await baseBl.DeleteAsync(ids);
        return Ok();
    }
}