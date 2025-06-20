using webapi.Services;
using webapi.Models;
using Microsoft.AspNetCore.Mvc;
using webapi.DTO;

namespace webapi.Controllers;



[ApiController]
[Route("[controller]")]
public class ReceiptController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    ReceiptService _service;

    public ReceiptController(IWebHostEnvironment env, ReceiptService service)
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
    public async Task<ActionResult<Receipt>> GetById(int id)
    {
        var userId = HttpContext.Items["userId"]?.ToString();

        if (userId is null)
        {
            return Unauthorized("Cookie not valid.");
        }

        var receipt = await _service.GetById(id);

        if (receipt is not null)
        {
            if (receipt.UploadedBy != null && receipt.UploadedBy != userId)
            {
                return Unauthorized("You do not have permission to view this receipt.");
            }

            return receipt;
        }
        else
        {
            return NotFound();
        }
    }


    [HttpPost]
    public async Task<IActionResult> Create(CreateReceiptDto dto)
    {
        // TODO: validate uid, receiptid, and supplierid

        var receipt = new Receipt
        {
            ReceiptUrl = dto.ReceiptUrl,

            // load foreign key (user.Uid) (optional)
            UploadedBy = HttpContext.Items["userId"]?.ToString()
        };

        await _service.Create(receipt);

        return CreatedAtAction(nameof(GetById), new { id = receipt!.ReceiptId }, receipt);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = HttpContext.Items["userId"]?.ToString();

        if (userId is null)
        {
            return Unauthorized("Cookie not valid.");
        }

        var receipt = await _service.GetById(id);

        if (receipt is not null)
        {
            if (receipt != null && receipt.UploadedBy != userId)
            {
                return Unauthorized("You do not have permission to delete this receipt.");
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
