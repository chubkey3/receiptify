using webapi.Services;
using webapi.Models;
using Microsoft.AspNetCore.Mvc;
using webapi.DTO;

namespace webapi.Controllers;



[ApiController]
[Route("[controller]")]
public class ExpenseController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    ExpenseService _service;

    public ExpenseController(IWebHostEnvironment env, ExpenseService service)
    {
        _service = service;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!_env.IsDevelopment())
        {
            return Unauthorized("This endpoint is only available in development.");
        }
        return Ok(await _service.GetAll());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Expense>> GetById(int id)
    {
        var userId = HttpContext.Items["userId"]?.ToString();

        if (userId is null)
        {
            return Unauthorized("Cookie not valid.");
        }

        var expense = await _service.GetById(id);

        if (expense is not null)
        {
            if (expense.Uid != userId)
            {
                return Unauthorized("You do not have permission to view this expense.");
            }

            return expense;
        }
        else
        {
            return NotFound();
        }
    }


    [HttpPost]
    public async Task<IActionResult> Create(CreateExpenseDto dto)
    {
        // TODO: validate uid, receiptid, and supplierid
        var userId = HttpContext.Items["userId"]?.ToString();

        if (userId is null)
        {
            return Unauthorized("Cookie not valid.");
        }

        var expense = new Expense
        {
            TotalAmount = dto.TotalAmount,
            // load foreign keys
            Uid = userId,
            ReceiptId = dto.ReceiptId,
            SupplierId = dto.SupplierId
        };

        await _service.Create(expense);

        return CreatedAtAction(nameof(GetById), new { id = expense!.Uid }, expense);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = HttpContext.Items["userId"]?.ToString();

        if (userId is null)
        {
            return Unauthorized("Cookie not valid.");
        }

        var expense = await _service.GetById(id);

        if (expense is not null)
        {
            if (expense.Uid != userId)
            {
                return Unauthorized("You do not have permission to delete this expense.");
            }

            await _service.DeleteById(id);
            return Ok();
        }
        else
        {
            return NotFound();
        }
    }
}
