using webapi.Services;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class UploadController : ControllerBase
{
    UploadService _uploadService;

    public UploadController(UploadService service)
    {
        _uploadService = service;
    }



    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        // 1. load credentials
        // 2. upload image to GCS
        // 3. use uid and filename to trigger workflow

        string filename;

        // load credentials
        var userId = HttpContext.Items["userId"]?.ToString();

        if (userId is null)
        {
            return Unauthorized("Cookie not valid.");
        }

        // upload image to GCS
        if (file == null || file.Length == 0)
        {
            return BadRequest("No image provided.");
        }

        try
        {
            var uploadedFilename = await _uploadService.UploadImage(file);

            filename = uploadedFilename.Split('/')[1];
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading image: {ex.Message}");
        }

        // use firebase token and filename to trigger workflow
        try
        {
            Request.Cookies.TryGetValue("token", out var token);

            if (token is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error extracting cookie");
            }

            var result = await _uploadService.TriggerWorkflow(token, filename);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading image: {ex.Message}");
        }
    }


}
