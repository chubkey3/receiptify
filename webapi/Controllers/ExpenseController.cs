using webapi.Services;
using webapi.Models;
using Microsoft.AspNetCore.Mvc;
using webapi.DTO;

namespace webapi.Controllers;



[ApiController]
[Route("[controller]")]
public class ExpenseController : ControllerBase
{
    ExpenseService _service;

    public ExpenseController(ExpenseService service)
    {
        _service = service;
    }

    [HttpGet]
    public IEnumerable<Expense> GetAll()
    {
        return _service.GetAll();
    }
    
    [HttpGet("{id}")]
    public ActionResult<Expense> GetById(int id)
    {
        var userId = HttpContext.Items["userId"]?.ToString();
        
        var expense = _service.GetById(id);

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
    public IActionResult Create(CreateExpenseDto dto)
    {
        // TODO: validate uid, receiptid, and supplierid

        var expense = new Expense
        {
            TotalAmount = dto.TotalAmount,
            ExpenseDate = dto.ExpenseDate,
            // load foreign keys
            Uid = dto.Uid,
            ReceiptId = dto.ReceiptId,
            SupplierId = dto.SupplierId
        };

        _service.Create(expense);
        
        return CreatedAtAction(nameof(GetById), new { id = expense!.Uid }, expense);
    }


    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var expense = _service.GetById(id);

        if (expense is not null)
        {
            _service.DeleteById(id);
            return Ok();
        }
        else
        {
            return NotFound();
        }
    }
}
