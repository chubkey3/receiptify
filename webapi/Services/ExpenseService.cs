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