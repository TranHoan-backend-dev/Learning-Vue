using Microsoft.AspNetCore.Mvc;
using MISA.BL.Base;
using MISA.BL.DTO.Request;
using MISA.Common.Model;
using MISA.Common.Model.Pageable;

namespace MISA.API.Controller;

[ApiController]
[Route("api/v1/[controller]")]
public class SalaryCompositionSystemsController(IBaseBl<SalaryCompositionSystem> baseBl) : ControllerBase
{
    /// <summary>
    /// Lấy ra danh sách các bản ghi Danh mục hệ thống
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] Pageable pageable, [FromQuery] FilterRequest request)
    {
        pageable ??= new Pageable();
        request ??= new FilterRequest();
        var result = await baseBl.GetAllAsync(pageable, request);
        return Ok(result);
    }
}
