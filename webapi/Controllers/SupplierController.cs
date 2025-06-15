using webapi.Services;
using webapi.Models;
using Microsoft.AspNetCore.Mvc;
using webapi.DTO;

namespace webapi.Controllers;



[ApiController]
[Route("[controller]")]
public class SupplierController : ControllerBase
{
    SupplierService _service;

    public SupplierController(SupplierService service)
    {
        _service = service;
    }

    [HttpGet]
    public IEnumerable<Supplier> GetAll()
    {
        return _service.GetAll();
    }

    [HttpGet("{id}")]
    public ActionResult<Supplier> GetById(int id)
    {
        var supplier = _service.GetById(id);

        if (supplier is not null)
        {
            return supplier;
        }
        else
        {
            return NotFound();
        }
    }


    [HttpPost]
    public IActionResult Create(CreateSupplierDto dto)
    {
        // TODO: validate uid, Supplierid, and supplierid

        var Supplier = new Supplier
        {            
            SupplierName = dto.SupplierName
        };

        _service.Create(Supplier);
        
        return CreatedAtAction(nameof(GetById), new { id = Supplier!.SupplierId }, Supplier);
    }


    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var Supplier = _service.GetById(id);

        if (Supplier is not null)
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
