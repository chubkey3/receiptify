using webapi.Models;
using webapi.Data;
using Microsoft.EntityFrameworkCore;

namespace webapi.Services;

public class ReceiptService
{
    private readonly ReceiptifyContext _context;
    public ReceiptService(ReceiptifyContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Receipt>> GetAll()
    {
        return await _context.Receipts
       .AsNoTracking()
       .ToListAsync();
    }

    public async Task<Receipt?> GetById(int id)
    {
        return await _context.Receipts
        .AsNoTracking()
        .SingleOrDefaultAsync(p => p.ReceiptId == id);
    }

    public async Task<Receipt?> Create(Receipt newReceipt)
    {
        await _context.Receipts.AddAsync(newReceipt);
        await _context.SaveChangesAsync();

        return newReceipt;
    }

    public async Task DeleteById(int id)
    {
        var receiptToDelete = await _context.Receipts.FindAsync(id);
        if (receiptToDelete is not null)
        {
            _context.Receipts.Remove(receiptToDelete);
            await _context.SaveChangesAsync();
        }
    }
}