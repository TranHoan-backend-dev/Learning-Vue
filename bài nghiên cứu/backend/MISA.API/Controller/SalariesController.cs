using Microsoft.AspNetCore.Mvc;
using MISA.BL.Base;
using MISA.BL.DTO.Request;
using MISA.BL.DTO.Response;
using MISA.Common.Model;
using MISA.Common.Model.Pageable;

namespace MISA.API.Controller;

[ApiController]
[Route("api/v1/[controller]")]
public class SalariesController(IBaseBl<Salary> baseBl) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách bản ghi phân trang và lọc
    /// </summary>
    /// <param name="pageable">Thông tin phân trang</param>
    /// <param name="request">Điều kiện lọc</param>
    /// <returns>Danh sách bản ghi</returns>
    [HttpPost]
    public async Task<IActionResult> GetAllAsync([FromQuery] Pageable pageable, [FromBody] FilterRequest request)
    {
        var result = await baseBl.GetAllAsync(pageable, request);
        // Map Result to SalaryResponse
        if (result?.Data != null)
        {
            var responses = result.Data.Select(s => new SalaryResponse
            {
                ComponentId = s.ComponentId,
                ComponentCode = s.ComponentCode,
                ComponentName = s.ComponentName,
                AppliedUnit = s.AppliedUnit,
                ComponentType = s.ComponentType,
                Attribute = s.Attribute,
                ValueType = s.ValueType,
                Value = s.Value,
                CreatedAt = s.CreatedAt,
                CreatedBy = s.CreatedBy,
                ModifiedAt = s.ModifiedAt,
                ModifiedBy = s.ModifiedBy
            }).ToList();

            var pagingResponse = new PagingData<SalaryResponse>
            {
                Data = responses,
                Pageable = result.Pageable
            };
            return Ok(pagingResponse);
        }
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

        var response = new SalaryResponse
        {
            ComponentId = result.ComponentId,
            ComponentCode = result.ComponentCode,
            ComponentName = result.ComponentName,
            AppliedUnit = result.AppliedUnit,
            ComponentType = result.ComponentType,
            Attribute = result.Attribute,
            ValueType = result.ValueType,
            Value = result.Value,
            CreatedAt = result.CreatedAt,
            CreatedBy = result.CreatedBy,
            ModifiedAt = result.ModifiedAt,
            ModifiedBy = result.ModifiedBy
        };

        return Ok(response);
    }

    /// <summary>
    /// Thêm mới bản ghi
    /// </summary>
    /// <param name="request">Dữ liệu thêm mới</param>
    /// <returns>Kết quả thêm mới</returns>
    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] CreateRequest request)
    {
        var salary = new Salary
        {
            ComponentId = Guid.NewGuid(),
            ComponentCode = request.ComponentCode,
            ComponentName = request.ComponentName,
            AppliedUnit = request.AppliedUnit,
            ComponentType = request.ComponentType,
            Attribute = request.Attribute,
            ValueType = request.ValueType,
            Value = request.Value
        };

        await baseBl.AddAsync(salary);
        return StatusCode(201, request);
    }

    /// <summary>
    /// Cập nhật bản ghi
    /// </summary>
    /// <param name="id">ID bản ghi cần cập nhật</param>
    /// <param name="request">Dữ liệu cập nhật</param>
    /// <returns>Kết quả cập nhật</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateRequest request)
    {
        var salary = new Salary
        {
            ComponentId = id,
            ComponentCode = request.ComponentCode,
            ComponentName = request.ComponentName,
            AppliedUnit = request.AppliedUnit,
            ComponentType = request.ComponentType,
            Attribute = request.Attribute,
            ValueType = request.ValueType,
            Value = request.Value
        };

        var result = await baseBl.UpdateAsync(salary, id);
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
