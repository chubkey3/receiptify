using webapi.Models;
using webapi.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace webapi.Services;

public class UserService
{
    private readonly ReceiptifyContext _context;
    public UserService(ReceiptifyContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _context.Users
       .AsNoTracking()
       .ToListAsync();
    }

    public async Task<User?> GetById(string id)
    {
        return await _context.Users
        .AsNoTracking()
        .SingleOrDefaultAsync(p => p.Uid == id);
    }

    public async Task<User?> Create(User newUser)
    {
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        return newUser;
    }

    public async Task DeleteById(string id)
    {
        var userToDelete = await _context.Users.FindAsync(id);
        if (userToDelete is not null)
        {
            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Expense>> GetExpenses(string userId)
    {
        return await _context.Expenses
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
      .ToListAsync();

    }
}