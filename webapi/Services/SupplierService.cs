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

    public async Task<IEnumerable<Supplier>> GetAll()
    {
        return await _context.Suppliers
       .AsNoTracking()
       .ToListAsync();
    }

    public async Task<Supplier?> GetById(int id)
    {
        return await _context.Suppliers
        .AsNoTracking()
        .SingleOrDefaultAsync(p => p.SupplierId == id);
    }

    public async Task<Supplier?> GetByName(string supplierName)
    {
        return await _context.Suppliers.FirstOrDefaultAsync(u => u.SupplierName == supplierName);
    }

    public async Task<Supplier?> Create(Supplier newSupplier)
    {
        await _context.Suppliers.AddAsync(newSupplier);
        await _context.SaveChangesAsync();

        return newSupplier;
    }

    public async Task DeleteById(int id)
    {
        var supplierToDelete = await _context.Suppliers.FindAsync(id);
        if (supplierToDelete is not null)
        {
            _context.Suppliers.Remove(supplierToDelete);
            await _context.SaveChangesAsync();
        }
    }
}