using Microsoft.AspNetCore.Mvc;
using MISA.BL.Base;
using MISA.BL.DTO.Request;
using MISA.Common.Model;
using MISA.Common.Model.Pageable;

namespace MISA.API.Controller;

[ApiController]
[Route("api/v1/[controller]")]
public class OrganizationsController(IBaseBl<Organization> baseBl) : ControllerBase
{
    /// <summary>
    /// Lấy ra danh sách các bản ghi Đơn vị áp dụng kèm phân trang
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await baseBl.GetAllAsync(
            new Pageable { PageSize = 1000 },
            new FilterRequest()
        );
        return Ok(result.Data);
    }
}