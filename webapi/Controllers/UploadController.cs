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

        string uid;
        string filename;

        // load credentials
        if (!Request.Cookies.TryGetValue("token", out string? token))
        {            
            return Unauthorized("No auth token found in cookies");
        }

        try
        {
            var user = await _authService.VerifyTokenAsync(token);
            uid = user.Uid;
        }
        catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.ExpiredIdToken)
        {
            return Unauthorized("Token expired");
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
            var result = await _uploadService.TriggerWorkflow(uid, filename);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading image: {ex.Message}");
        }
    }

    
}
