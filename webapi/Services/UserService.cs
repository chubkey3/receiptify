using webapi.Models;
using webapi.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;

namespace webapi.Services;

public class PaginatedResponse<T>
{
    public int CurrentPage { get; set; }
    public int PageSize {get; set;}
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public List<T> Items { get; set; }
}
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

    public async Task<PaginatedResponse<Expense>> GetExpenses(string userId, int pageNumber, int pageSize)
    {

        var count = await _context.Expenses
        .Where(e => e.Uid == userId)
        .CountAsync();

        var items = await _context.Expenses
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
      .OrderByDescending(e => e.ExpenseDate)
      .Skip((pageNumber - 1) * pageSize)
      .Take(pageSize)
      .AsNoTracking()
      .ToListAsync();

        return new PaginatedResponse<Expense>
        {
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalCount = count,
            TotalPages = (int)Math.Ceiling((double)count / pageSize),
            Items = items
        };

    }
}