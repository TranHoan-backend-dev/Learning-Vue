using Microsoft.AspNetCore.Mvc;
using MISA.BL.Base;
using MISA.BL.DTO.Request;
using MISA.BL.DTO.Response;
using MISA.BL.Service;
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
        if (result?.Data != null)
        {
            var responses = result.Data.Select(s => new SalaryCompositionResponse
            {
                SalaryComponentId = s.SalaryComponentId,
                SalaryComponentCode = s.SalaryComponentCode,
                SalaryComponentName = s.SalaryComponentName,
                AppliedUnitId = s.AppliedUnitId,
                SalaryComponentSystemId = s.SalaryComponentSystemId,
                Attribute = s.Attribute,
                ValueType = s.ValueType,
                Value = s.Value,
                Status = s.Status,
                Source = s.Source,
                AppliedUnitName = s.AppliedUnitName,
                SalaryComponentSystemName = s.SalaryComponentSystemName,
                CreatedAt = s.CreatedAt,
                CreatedBy = s.CreatedBy,
                ModifiedAt = s.ModifiedAt,
                ModifiedBy = s.ModifiedBy
            }).ToList();

            var pagingResponse = new PagingData<SalaryCompositionResponse>
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

        var response = new SalaryCompositionResponse
        {
            SalaryComponentId = result.SalaryComponentId,
            SalaryComponentCode = result.SalaryComponentCode,
            SalaryComponentName = result.SalaryComponentName,
            AppliedUnitId = result.AppliedUnitId,
            SalaryComponentSystemId = result.SalaryComponentSystemId,
            Attribute = result.Attribute,
            ValueType = result.ValueType,
            Value = result.Value,
            Status = result.Status,
            Source = result.Source,
            AppliedUnitName = result.AppliedUnitName,
            SalaryComponentSystemName = result.SalaryComponentSystemName,
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
    public async Task<IActionResult> AddAsync([FromBody] SalaryCompositionCreateRequest request)
    {
        var entity = new SalaryComposition
        {
            SalaryComponentId = Guid.NewGuid(),
            SalaryComponentCode = request.SalaryComponentCode,
            SalaryComponentName = request.SalaryComponentName,
            AppliedUnitId = request.AppliedUnitId,
            SalaryComponentSystemId = request.SalaryComponentSystemId,
            Attribute = request.Attribute,
            ValueType = request.ValueType,
            Value = request.Value,
            Status = request.Status,
            Source = request.Source ?? "Tự thêm"
        };

        await baseBl.AddAsync(entity);
        return StatusCode(201, entity.SalaryComponentId);
    }

    /// <summary>
    /// Cập nhật bản ghi
    /// </summary>
    /// <param name="id">ID bản ghi cần cập nhật</param>
    /// <param name="request">Dữ liệu cập nhật</param>
    /// <returns>Kết quả cập nhật</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] SalaryCompositionUpdateRequest request)
    {
        var entity = new SalaryComposition
        {
            SalaryComponentId = id,
            SalaryComponentCode = request.SalaryComponentCode,
            SalaryComponentName = request.SalaryComponentName,
            AppliedUnitId = request.AppliedUnitId,
            SalaryComponentSystemId = request.SalaryComponentSystemId,
            Attribute = request.Attribute,
            ValueType = request.ValueType,
            Value = request.Value,
            Status = request.Status,
            Source = request.Source ?? "Tự thêm"
        };

        var result = await baseBl.UpdateAsync(entity, id);
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
