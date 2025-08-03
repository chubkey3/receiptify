using webapi.Services;
using webapi.Models;
using Microsoft.AspNetCore.Mvc;
using webapi.DTO;
using System.Threading.Tasks;

namespace webapi.Controllers;




[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    UserService _service;

    public UserController(IWebHostEnvironment env, UserService service)
    {
        _service = service;
        _env = env;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        if (!_env.IsDevelopment())
        {
            return Unauthorized("This endpoint is only available in development.");
        }
        return Ok(await _service.GetAll());
    }

    [HttpGet]
    public async Task<ActionResult<User>> GetById()
    {
        var userId = HttpContext.Items["userId"]?.ToString();

        if (userId is null)
        {
            return Unauthorized("Cookie not valid.");
        }

        var user = await _service.GetById(userId);

        if (user is not null)
        {
            return user;
        }
        else
        {
            return NotFound();
        }
    }


    [HttpPost]
    public async Task<IActionResult> Create(CreateUserDto newUser)
    {

        var userId = HttpContext.Items["userId"]?.ToString();

        if (userId is null)
        {
            return Unauthorized("Cookie not valid.");
        }

        var user = new User
        {
            Uid = userId,
            Username = newUser.Username,
            Email = newUser.Email
        };

        await _service.Create(user);
        return CreatedAtAction(nameof(GetById), new { id = user!.Uid }, user);
    }


    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        var userId = HttpContext.Items["userId"]?.ToString();

        if (userId is null)
        {
            return Unauthorized("Cookie not valid.");
        }

        var user = await _service.GetById(userId);

        if (user is not null)
        {
            if (user.Uid != userId)
            {
                return Unauthorized("You do not have permission to delete this user.");
            }

            await _service.DeleteById(userId);
            return Ok();
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("expenses")]
    public async Task<IActionResult> GetExpenses(int pageNumber = 1, int pageSize = 10)
    {

        var userId = HttpContext.Items["userId"]?.ToString();

        if (userId is null)
        {
            return Unauthorized("Cookie not valid.");
        }

        var user = await _service.GetById(userId);

        if (user is not null)
        {
            var expenses = await _service.GetExpenses(userId, pageNumber, pageSize);

            return Ok(expenses);
        }
        else
        {
            return NotFound();
        }

    }
}
