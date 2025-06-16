using webapi.Services;
using webapi.Models;
using Microsoft.AspNetCore.Mvc;
using webapi.DTO;

namespace webapi.Controllers;




[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    UserService _service;

    public UserController(UserService service)
    {
        _service = service;
    }

    [HttpGet]
    public IEnumerable<User> GetAll()
    {
        return _service.GetAll();
    }

    [HttpGet("{id}")]
    public ActionResult<User> GetById(string id)
    {
        var user = _service.GetById(id);

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
            return Unauthorized("User not found.");            
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


    [HttpDelete("{id}")]
    public IActionResult Delete(string id)
    {
        var user = _service.GetById(id);

        if (user is not null)
        {
            _service.DeleteById(id);
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
            return Unauthorized("User not found.");            
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
