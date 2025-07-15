using webapi.Models;
using webapi.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace webapi.Services;

public class LineGraphExpensesType
{
    public int Day { get; set; }
    public decimal TotalAmount { get; set; }
}

public class CircleGraphExpensesType {
    public string SupplierName { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public decimal GrandTotal { get; set; }
}


public class AnalyticsService
{
    private readonly IDatabase _redis;
    private readonly ReceiptifyContext _context;
    public AnalyticsService(IConnectionMultiplexer redis, ReceiptifyContext context)
    {
        _redis = redis.GetDatabase();
        _context = context;
    }

    public async Task SetUserCacheAsync(string userId, double amountSpent, double amountProjected, string topMerchant, double topMerchantAmount, string lineGraphExpenses, string circleGraphExpenses)
    {

        var hashEntries = new HashEntry[]
        {
            new HashEntry("amount_spent", amountSpent),
            new HashEntry("amount_projected", amountProjected),
            new HashEntry("top_merchant", topMerchant),
            new HashEntry("top_merchant_amount", topMerchantAmount),
            new HashEntry("line_graph_expenses", lineGraphExpenses),
            new HashEntry("circle_graph_expenses", circleGraphExpenses)
        };

        await _redis.HashSetAsync($"user:{userId}", hashEntries);

        // FUTURE: set TTL to 24 hours
        //await _db.KeyExpireAsync(key, TimeSpan.FromDays(7));
    }

    public async Task<IActionResult?> GetSummary(string userId)
    {

        var results = await _redis.HashGetAsync($"user:{userId}", new RedisValue[]
        {
            "amount_spent", "amount_projected", "top_merchant", "top_merchant_amount", "line_graph_expenses", "circle_graph_expenses"
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

        List<LineGraphExpensesType>? line_graph_expenses = results[4].IsNull ? [] : JsonSerializer.Deserialize<List<LineGraphExpensesType>>(results[4].ToString());
        List<CircleGraphExpensesType>? circle_graph_expenses = results[5].IsNull ? [] : JsonSerializer.Deserialize<List<CircleGraphExpensesType>>(results[5].ToString());

        return new JsonResult(new { amountSpent, amountProjected, topMerchant, topMerchantAmount, line_graph_expenses, circle_graph_expenses });
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

        // line graph
        var lineGraphsExpenses = await _context.Expenses
            .Where(e => e.ExpenseDate.Month == currentMonth &&
                        e.ExpenseDate.Year == currentYear)
            .Select(e => new
            {
                e.ExpenseDate.Day,
                e.TotalAmount
            })
            .OrderBy(x => x.Day)
            .ToListAsync();

        var lineGraphsExpensesFormatted = new JsonArray();

        decimal sum = 0;
        var pos = 0;

        for (int i = 1; i <= daysInMonth; i++)
        {
            while (pos < lineGraphsExpenses.Count && lineGraphsExpenses[pos].Day <= i)
            {
                // update aggregated sum up to day i
                sum = sum + lineGraphsExpenses[pos].TotalAmount;
                pos++;
            }

            var dayObj = new JsonObject
            {
                ["Day"] = i,
                ["TotalAmount"] = sum
            };

            lineGraphsExpensesFormatted.Add(dayObj);
        }
        
        // pie chart        
        var totalAmountThisMonth = await _context.Expenses
            .Where(e => e.Uid == userId &&
                        e.ExpenseDate.Month == currentMonth &&
                        e.ExpenseDate.Year == currentYear)
            .SumAsync(e => e.TotalAmount);

        var circleGraphExpenses = await _context.Expenses
            .Where(e => e.Uid == userId &&
                        e.ExpenseDate.Month == currentMonth &&
                        e.ExpenseDate.Year == currentYear)
            .GroupBy(e => new { e.SupplierId, e.Supplier.SupplierName })
            .Select(g => new
            {
                SupplierName = g.Key.SupplierName,
                TotalAmount = g.Sum(e => e.TotalAmount),
                GrandTotal = g.Sum(e => e.TotalAmount) / totalAmountThisMonth
            })
            .OrderByDescending(x => x.GrandTotal)
            .ToListAsync();

        // update redis
        await SetUserCacheAsync(userId, total_month ?? 0.00, projectedSpending ?? 0.00, topSupplier?.supplier_name ?? "", topSupplier?.total_spent ?? 0.00, System.Text.Json.JsonSerializer.Serialize(lineGraphsExpensesFormatted) ?? "", System.Text.Json.JsonSerializer.Serialize(circleGraphExpenses) ?? "");

        return new
        {
            amountSpent = total_month ?? 0.00,
            amountProjected = projectedSpending ?? 0.00,
            topMerchant = topSupplier?.supplier_name ?? "",
            topMerchantAmount = topSupplier?.total_spent ?? 0.00,
            line_graph_expenses = lineGraphsExpensesFormatted ?? [],
            circle_graph_expenses = circleGraphExpenses ?? []
        };

    }

}