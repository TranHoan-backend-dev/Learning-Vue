using Microsoft.AspNetCore.Mvc;
using MISA.BL.Base;
using MISA.Common.Enum;
using MISA.Common.Model;
// using MISA.Common.Resources;

namespace MISA.API.Controllers;

[ApiController]
[Route("/v1/api/[controller]")]
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
        // try
        // {
            candidate.State = AppEnum.ModelState.Update;
            var res = await baseBl.SaveDataAsync([candidate]);
            return Ok(res);
        // }
        // catch (System.Exception e)
        // {
        //     log.LogError(e, $"{LogPrefix} Error updating entity with id {id}");
        //     return StatusCode(
        //         (int)AppEnum.StatusCode.InternalServerError,
        //         new ErrorResult()
        //         {
        //             DevMsg = e.Message,
        //             UserMsg = ResourcesVN.Exception,
        //             MoreInfo = e.ToString(),
        //         });
        // }
    }

    [HttpPost]
    public async Task<IActionResult> AddNewCandidate([FromBody] Candidate candidate)
    {
        log.LogInformation($"{LogPrefix} Create new candidate");
        // try
        // {
            candidate.State = AppEnum.ModelState.Insert;
            var res = await baseBl.SaveDataAsync([candidate]);
            return Ok(res);
        // }
        // catch (System.Exception e)
        // {
            // log.LogError(e.Message);
            // return StatusCode(
                // (int)AppEnum.StatusCode.InternalServerError,
                // new ErrorResult()
                // {
                    // DevMsg = e.Message,
                    // UserMsg = ResourcesVN.Exception,
                    // MoreInfo = e.ToString(),
                // }
            // );
        // }
    }
}