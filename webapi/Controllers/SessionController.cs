using webapi.Services;
using webapi.Models;
using Microsoft.AspNetCore.Mvc;
using webapi.DTO;

namespace webapi.Controllers;

public class TokenRequest
{
    public string IdToken { get; set; } = null!;
}


[ApiController]
[Route("[controller]")]
public class SessionController : ControllerBase
{


    public SessionController()
    {

    }



    [HttpPost("login")]
    public IActionResult SessionLogin([FromBody] TokenRequest request)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddHours(1)
        };

        Response.Cookies.Append("token", request.IdToken, cookieOptions);

        return Ok();
    }


    [HttpPost("refresh")]

    public IActionResult SessionRefresh([FromBody] TokenRequest request)
    {


        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddHours(1)
        };

        Response.Cookies.Append("token", request.IdToken, options);
        
        return Ok();
    }
}
