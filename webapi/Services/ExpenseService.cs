using webapi.Models;
using webapi.Data;
using Microsoft.EntityFrameworkCore;

namespace webapi.Services;

public class ExpenseService
{
    private readonly ReceiptifyContext _context;
    public ExpenseService(ReceiptifyContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Expense>> GetAll()
    {
        return await _context.Expenses
       .AsNoTracking()
       .ToListAsync();
    }

    public async Task<Expense?> GetById(int id)
    {
        return await _context.Expenses
        .AsNoTracking()
        .Include(e => e.Supplier) // Include related Supplier
        .Include(e => e.Receipt) // Include related Receipt
        .Select(e => new Expense
        {
            ExpenseId = e.ExpenseId,
            TotalAmount = e.TotalAmount,
            ExpenseDate = e.ExpenseDate,
            Uid = e.Uid,
            SupplierId = e.SupplierId,
            ReceiptId = e.ReceiptId,
            Supplier = new Supplier
            {
                SupplierId = e.Supplier.SupplierId,
                SupplierName = e.Supplier.SupplierName
            },
            Receipt = e.Receipt == null ? null : new Receipt
            {
                ReceiptId = e.Receipt.ReceiptId,
                ReceiptUrl = e.Receipt.ReceiptUrl,
                UploadDate = e.Receipt.UploadDate
            }
        })
        .SingleOrDefaultAsync(p => p.ExpenseId == id);
    }

    public async Task<Expense?> Create(Expense newExpense)
    {
        await _context.Expenses.AddAsync(newExpense);
        await _context.SaveChangesAsync();

        return newExpense;
    }

    public async Task DeleteById(int id)
    {
        var expenseToDelete = await _context.Expenses.FindAsync(id);
        if (expenseToDelete is not null)
        {
            _context.Expenses.Remove(expenseToDelete);
            await _context.SaveChangesAsync();
        }
    }
}