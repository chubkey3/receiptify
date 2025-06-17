using webapi.Models;
using webapi.Data;
using Microsoft.EntityFrameworkCore;

namespace webapi.Services;

public class SupplierService
{
    private readonly ReceiptifyContext _context;
    public SupplierService(ReceiptifyContext context)
    {
        _context = context;
    }

    public IEnumerable<Supplier> GetAll()
    {
         return _context.Suppliers
        .AsNoTracking()
        .ToList();
    }

    public Supplier? GetById(int id)
    {
        return _context.Suppliers     
        .AsNoTracking()
        .SingleOrDefault(p => p.SupplierId == id);
    }

    public Supplier? GetByName(string supplierName)
    {
        return _context.Suppliers.FirstOrDefault(u => u.SupplierName == supplierName);
    }

    public Supplier? Create(Supplier newSupplier)
    {
        _context.Suppliers.Add(newSupplier);
        _context.SaveChanges();

        return newSupplier;
    }    

    public void DeleteById(int id)
    {
        var supplierToDelete = _context.Suppliers.Find(id);
        if (supplierToDelete is not null)
        {
            _context.Suppliers.Remove(supplierToDelete);
            _context.SaveChanges();
        }        
    }
}