using webapi.Services;
using webapi.Models;
using Microsoft.AspNetCore.Mvc;
using webapi.DTO;

namespace webapi.Controllers;



[ApiController]
[Route("[controller]")]
public class SupplierController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    SupplierService _service;

    public SupplierController(IWebHostEnvironment env, SupplierService service)
    {
        _service = service;
        _env = env;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        if (!_env.IsDevelopment())
        {
            return Unauthorized("This endpoint is only available in development.");
        }
        return Ok(_service.GetAll());
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

        var exists = _service.GetByName(dto.SupplierName);

        if (exists == null)
        {
            var Supplier = new Supplier
            {
                SupplierName = dto.SupplierName
            };

            _service.Create(Supplier);

            return CreatedAtAction(nameof(GetById), new { id = Supplier!.SupplierId }, Supplier);
        }

        return Ok(exists);

    }


    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (!_env.IsDevelopment())
        {
            return Unauthorized("This endpoint is only available in development.");
        }

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
