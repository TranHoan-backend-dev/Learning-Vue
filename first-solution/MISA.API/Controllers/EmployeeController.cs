using BL.Dto.Request;
using BL.Dto.Response;
using BL.Service;
using Microsoft.AspNetCore.Mvc;

namespace first_solution.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
public class EmployeeController(
    IEmployeeService service,
    ILogger<EmployeeController> log
) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateNewEmployee([FromBody] EmployeeCreateDto dto)
    {
        log.Log(LogLevel.Information, "Create a new employee with code {EmployeeCode}", dto.EmployeeCode);
        await service.CreateNewEmployee(dto);

        return Ok(new ApiResponse
        {
            Status = 200,
            Message = "OK"
        });
    }
}