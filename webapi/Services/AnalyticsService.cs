using webapi.Models;
using webapi.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Services;

public class AnalyticsService
{
    private readonly IDatabase _redis;
    private readonly ReceiptifyContext _context;
    public AnalyticsService(IConnectionMultiplexer redis, ReceiptifyContext context)
    {
        _redis = redis.GetDatabase();
        _context = context;
    }

    public async Task SetUserCacheAsync(string userId, double amountSpent, double amountProjected, string topMerchant, double topMerchantAmount)
    {

        var hashEntries = new HashEntry[]
        {
            new HashEntry("amount_spent", amountSpent),
            new HashEntry("amount_projected", amountProjected),
            new HashEntry("top_merchant", topMerchant),
            new HashEntry("top_merchant_amount", topMerchantAmount)
        };

        await _redis.HashSetAsync($"user:{userId}", hashEntries);

        // FUTURE: set TTL to 24 hours
        //await _db.KeyExpireAsync(key, TimeSpan.FromDays(7));
    }

    public async Task<IActionResult?> GetSummary(string userId)
    {

        var results = await _redis.HashGetAsync($"user:{userId}", new RedisValue[]
        {
            "amount_spent", "amount_projected", "top_merchant", "top_merchant_amount"
        });

        // If all fields are null, cache miss
        if (results.All(r => r.IsNull))
        {
            Console.WriteLine("Cache Miss!");            
            var summary = await Update(userId);

            return new JsonResult(summary);
        }
        Console.WriteLine("Cache Hit!");

        double amountSpent = results[0].TryParse(out double spentVal) ? spentVal : 0.00;
        double amountProjected = results[1].TryParse(out double projectedVal) ? projectedVal : 0.00;

        string topMerchant = results[2].IsNull ? "" : results[2].ToString();
        double topMerchantAmount = results[3].TryParse(out double topMerchantVal) ? topMerchantVal : 0.00;

        return new JsonResult(new { amountSpent, amountProjected, topMerchant, topMerchantAmount });
    }

    public async Task<object> Update(string userId)
    {
        // run aggregations on userId and update Redis

        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        // amount spent this month
        var total_month = await _context.Expenses
            .Where(e => e.Uid == userId && e.ExpenseDate.Month == currentMonth && e.ExpenseDate.Year == currentYear)
            .SumAsync(e => (double?)e.TotalAmount);


        // projected spenting this month
        var today = DateTime.Today;
        var currentDay = today.Day;
        var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);

        var sumThisMonth = await _context.Expenses
            .Where(e => e.Uid == userId && e.ExpenseDate.Month == currentMonth && e.ExpenseDate.Year == currentYear)
            .SumAsync(e => (decimal?)e.TotalAmount) ?? 0;

        var projectedSpending = (double?)Math.Round(daysInMonth * (sumThisMonth / currentDay), 2);

        // most spent supplier and amount
        var topSupplier = await _context.Expenses
            .Where(e => e.Uid == userId && e.ExpenseDate.Month == currentMonth && e.ExpenseDate.Year == currentYear)
            .GroupBy(e => new { e.SupplierId, e.Supplier.SupplierName })
            .Select(g => new
            {
                supplier_name = g.Key.SupplierName,
                total_spent = (double?)Math.Round(g.Sum(e => e.TotalAmount), 2)
            })
            .OrderByDescending(x => x.total_spent)
            .FirstOrDefaultAsync();

        // update redis

        await SetUserCacheAsync(userId, total_month ?? 0.00, projectedSpending ?? 0.00, topSupplier?.supplier_name ?? "", topSupplier?.total_spent ?? 0.00);

        return new
        {
            amountSpent = total_month ?? 0.00,
            amountProjected = projectedSpending ?? 0.00,
            topMerchant = topSupplier?.supplier_name ?? "",
            topMerchantAmount = topSupplier?.total_spent ?? 0.00
        };

    }
        
}