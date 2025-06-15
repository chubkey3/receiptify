using webapi.Services;
using webapi.Models;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers;

public class CreateReceiptDto


{

    public DateTime? UploadDate { get; set; }

    public string ReceiptUrl { get; set; } = null!;

    public string? UploadedBy { get; set; }    


}

[ApiController]
[Route("[controller]")]
public class ReceiptController : ControllerBase
{
    ReceiptService _service;

    public ReceiptController(ReceiptService service)
    {
        _service = service;
    }

    [HttpGet]
    public IEnumerable<Receipt> GetAll()
    {
        return _service.GetAll();
    }

    [HttpGet("{id}")]
    public ActionResult<Receipt> GetById(int id)
    {
        var receipt = _service.GetById(id);

        if (receipt is not null)
        {
            return receipt;
        }
        else
        {
            return NotFound();
        }
    }


    [HttpPost]
    public IActionResult Create(CreateReceiptDto dto)
    {
        // TODO: validate uid, receiptid, and supplierid

        var receipt = new Receipt
        {
            ReceiptUrl = dto.ReceiptUrl,
            UploadDate = dto.UploadDate,
            
            // load foreign key (user.Uid) (optional)
            UploadedBy = dto.UploadedBy            
        };

        _service.Create(receipt);
        
        return CreatedAtAction(nameof(GetById), new { id = receipt!.ReceiptId }, receipt);
    }


    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var receipt = _service.GetById(id);

        if (receipt is not null)
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
