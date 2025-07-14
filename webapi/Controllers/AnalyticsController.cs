using webapi.Services;
using webapi.Models;
using Microsoft.AspNetCore.Mvc;
using webapi.DTO;

namespace webapi.Controllers;



[ApiController]
[Route("[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    AnalyticsService _service;

    public AnalyticsController(IWebHostEnvironment env, AnalyticsService service)
    {
        _service = service;
        _env = env;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetAll()
    {

        var userId = HttpContext.Items["userId"]?.ToString();

        if (userId is null)
        {
            return Unauthorized("Cookie not valid.");
        }

        var summary = await _service.GetSummary(userId);
            
        return Ok(summary);        

    }    

    [HttpPost("summary")]
    public async Task<IActionResult> Update()
    {

        var userId = HttpContext.Items["userId"]?.ToString();

        if (userId is null)
        {
            return Unauthorized("Cookie not valid.");
        }

        await _service.Update(userId);

        return Ok();

    }

}
