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

    public IEnumerable<Expense> GetAll()
    {
         return _context.Expenses
        .AsNoTracking()
        .ToList();
    }

    public Expense? GetById(int id)
    {
        return _context.Expenses
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
        .SingleOrDefault(p => p.ExpenseId == id);
    }

    public Expense? Create(Expense newExpense)
    {
          _context.Expenses.Add(newExpense);
         _context.SaveChanges();

        return newExpense;
    }

    public void DeleteById(int id)
    {
        var expenseToDelete = _context.Expenses.Find(id);
        if (expenseToDelete is not null)
        {
            _context.Expenses.Remove(expenseToDelete);
            _context.SaveChanges();
        }        
    }
}