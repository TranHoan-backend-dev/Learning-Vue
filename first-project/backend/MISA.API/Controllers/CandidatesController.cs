using Microsoft.AspNetCore.Mvc;
using MISA.BL.Base;
using MISA.BL.DTO.Request;
using MISA.Common.Enum;
using MISA.Common.Model;

// using MISA.Common.Resources;

namespace MISA.API.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
public class CandidatesController(
    IBaseBl<Candidate> baseBl,
    ILogger<CandidatesController> log
) : ControllerBase
{
    private const string LogPrefix = "[Candidate Controller]";

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] Candidate candidate, [FromRoute] Guid id)
    {
        log.LogInformation($"{LogPrefix} Updated Candidate with id: {id}");
        var res = await baseBl.UpdateAsync(candidate, id);
        return Ok(res);
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveCandidate([FromBody] Candidate candidate)
    {
        log.LogInformation($"{LogPrefix} Create new candidate");
        candidate.State = AppEnum.ModelState.Insert;
        var res = await baseBl.SaveDataAsync([candidate]);
        return Ok(res);
    }

    [HttpPost()]
    public async Task<IActionResult> AddNewCandidate([FromBody] Candidate candidate)
    {
        log.LogInformation($"{LogPrefix} Create new candidate");
        await baseBl.AddAsync(candidate);
        return Created();
    }

    [HttpGet]
    public async Task<IActionResult> GetCandidates([FromQuery] Pageable pageable, [FromQuery] FilterRequest request)
    {
        log.LogInformation($"{LogPrefix} Get Candidates with pageable: {pageable}, request: {request}");
        var res = await baseBl.GetAllAsync(pageable, request);
        return Ok(new
        {
            Data = res,
            Pageable = new Pageable()
            {
                PageIndex = pageable.PageIndex,
                PageSize = pageable.PageSize,
                TotalElements = baseBl.CountTotalElements().Result
            }
        });
    }
    
    
}