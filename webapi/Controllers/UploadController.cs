using webapi.Services;
using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin.Auth;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class UploadController : ControllerBase
{
    FirebaseAuthService _authService;
    UploadService _uploadService;        

    public UploadController(FirebaseAuthService service, UploadService service2)
    {
        _authService = service;
        _uploadService = service2;
    }



    [HttpGet]
    public IActionResult Test()
    {
        return Ok();
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

        // use uid and filename to trigger workflow
        try
        {
            var result = await _uploadService.TriggerWorkflow(userId, filename);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading image: {ex.Message}");
        }
    }

    
}
