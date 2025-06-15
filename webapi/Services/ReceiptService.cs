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

    public IEnumerable<Receipt> GetAll()
    {
         return _context.Receipts
        .AsNoTracking()
        .ToList();
    }

    public Receipt? GetById(int id)
    {
        return _context.Receipts     
        .AsNoTracking()
        .SingleOrDefault(p => p.ReceiptId == id);
    }

    public Receipt? Create(Receipt newReceipt)
    {
          _context.Receipts.Add(newReceipt);
         _context.SaveChanges();

        return newReceipt;
    }    

    public void DeleteById(int id)
    {
        var receiptToDelete = _context.Receipts.Find(id);
        if (receiptToDelete is not null)
        {
            _context.Receipts.Remove(receiptToDelete);
            _context.SaveChanges();
        }        
    }
}