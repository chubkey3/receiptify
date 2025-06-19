using webapi.Models;
using webapi.Data;
using Microsoft.EntityFrameworkCore;

namespace webapi.Services;

public class UserService
{
    private readonly ReceiptifyContext _context;
    public UserService(ReceiptifyContext context)
    {
        _context = context;
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users
       .AsNoTracking()
       .ToList();
    }

    public User? GetById(string id)
    {
        return _context.Users
        .AsNoTracking()
        .SingleOrDefault(p => p.Uid == id);
    }

    public User? Create(User newUser)
    {
        _context.Users.Add(newUser);
        _context.SaveChanges();

        return newUser;
    }

    public void DeleteById(string id)
    {
        var userToDelete = _context.Users.Find(id);
        if (userToDelete is not null)
        {
            _context.Users.Remove(userToDelete);
            _context.SaveChanges();
        }
    }

    public IEnumerable<Expense> GetExpenses(string userId)
    {
        return _context.Expenses
       .Where(e => e.Uid == userId)
       .Include(e => e.Supplier) // Include related Supplier        
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
           }
       })
      .AsNoTracking()
      .ToList();

    }
}