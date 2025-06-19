using webapi.Services;
using webapi.Models;
using Microsoft.AspNetCore.Mvc;
using webapi.DTO;

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
    public IActionResult GetAll()
    {
        if (!_env.IsDevelopment())
        {
            return Unauthorized("This endpoint is only available in development.");
        }
        return Ok(_service.GetAll());
    }

    [HttpGet]
    public ActionResult<User> GetById()
    {
        var userId = HttpContext.Items["userId"]?.ToString();

        if (userId is null)
        {
            return Unauthorized("Cookie not valid.");
        }

        var user = _service.GetById(userId);

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
    public IActionResult Create(CreateUserDto newUser)
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

        _service.Create(user);
        return CreatedAtAction(nameof(GetById), new { id = user!.Uid }, user);
    }


    [HttpDelete]
    public IActionResult Delete()
    {
        var userId = HttpContext.Items["userId"]?.ToString();

        if (userId is null)
        {
            return Unauthorized("Cookie not valid.");
        }

        var user = _service.GetById(userId);

        if (user is not null)
        {
            if (user.Uid != userId)
            {
                return Unauthorized("You do not have permission to delete this user.");
            }

            _service.DeleteById(userId);
            return Ok();
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("expenses")]
    public IActionResult GetExpenses()
    {

        var userId = HttpContext.Items["userId"]?.ToString();

        if (userId is null)
        {
            return Unauthorized("Cookie not valid.");
        }

        var user = _service.GetById(userId);

        if (user is not null)
        {

            var expenses = _service.GetExpenses(userId);

            return Ok(expenses);
        }
        else
        {
            return NotFound();
        }






    }
}
