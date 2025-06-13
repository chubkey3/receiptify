using webapi.Services;
using webapi.Models;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers;

public class CreateExpenseDto
{        
    public decimal TotalAmount { get; set; }
    
    public DateTime ExpenseDate { get; set; }

    public string Uid { get; set; } = null!;

    public int? ReceiptId { get; set; }

    public int SupplierId { get; set; }
}

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
        var expense = _service.GetById(id);

        if (expense is not null)
        {
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
