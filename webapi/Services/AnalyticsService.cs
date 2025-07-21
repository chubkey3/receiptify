using webapi.Models;
using webapi.Data;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace webapi.Services;

public class GetTopSupplierType
{
    public string supplier_name { get; set; }
    public double? total_spent { get; set; }
}

public class PercentChangeType {
    public double? percentChangeTotal { get; set; }
    public double? percentChangeProjected { get; set; }
}
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

    public async Task SetUserCacheAsync(string userId, double amountSpent, double amountProjected, string topMerchant, double topMerchantAmount, string lineGraphExpenses, string circleGraphExpenses, double? percentChangeTotal, double? percentChangeProjected)
    {

        var hashEntries = new HashEntry[]
        {
            new HashEntry("amount_spent", amountSpent),
            new HashEntry("amount_projected", amountProjected),
            new HashEntry("top_merchant", topMerchant),
            new HashEntry("top_merchant_amount", topMerchantAmount),
            new HashEntry("line_graph_expenses", lineGraphExpenses),
            new HashEntry("circle_graph_expenses", circleGraphExpenses),
            new HashEntry("percent_change_total", percentChangeTotal),
            new HashEntry("percent_change_projected", percentChangeProjected)
        };

        await _redis.HashSetAsync($"user:{userId}", hashEntries);

        // FUTURE: set TTL to 24 hours
        //await _db.KeyExpireAsync(key, TimeSpan.FromDays(7));
    }

    public async Task<IActionResult?> GetSummary(string userId)
    {

        var results = await _redis.HashGetAsync($"user:{userId}", new RedisValue[]
        {
            "amount_spent", "amount_projected", "top_merchant", "top_merchant_amount", "line_graph_expenses", "circle_graph_expenses", "percent_change_total", "percent_change_projected"
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

        double? percentChangeTotal = results[6].TryParse(out double percentChangeTotalVal) ? percentChangeTotalVal : null;
        double? percentChangeProjected = results[7].TryParse(out double percentChangeProjectedVal) ? percentChangeProjectedVal : null;

        return new JsonResult(new { amountSpent, amountProjected, topMerchant, topMerchantAmount, line_graph_expenses, circle_graph_expenses, percentChangeTotal, percentChangeProjected });
    }

    public async Task<double?> GetAmountSpent(string userId, int currentMonth, int currentYear)
    {
        return await _context.Expenses
            .Where(e => e.Uid == userId && e.ExpenseDate.Month == currentMonth && e.ExpenseDate.Year == currentYear)
            .SumAsync(e => (double?)e.TotalAmount);
    }

    public async Task<double?> GetProjectedAmountSpent(string userId, int currentMonth, int currentYear) {
        var today = DateTime.Today;
        var currentDay = today.Day;
        var daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);

        var sumThisMonth = await _context.Expenses
            .Where(e => e.Uid == userId && e.ExpenseDate.Month == currentMonth && e.ExpenseDate.Year == currentYear)
            .SumAsync(e => (decimal?)e.TotalAmount) ?? 0;

        return (double?)Math.Round(daysInMonth * (sumThisMonth / currentDay), 2);
    }

    public async Task<GetTopSupplierType?> GetTopSupplier(string userId, int currentMonth, int currentYear) {
        return await _context.Expenses
            .Where(e => e.Uid == userId && e.ExpenseDate.Month == currentMonth && e.ExpenseDate.Year == currentYear)
            .GroupBy(e => new { e.SupplierId, e.Supplier.SupplierName })
            .Select(g => new GetTopSupplierType
            {
                supplier_name = g.Key.SupplierName,
                total_spent = (double?)Math.Round(g.Sum(e => e.TotalAmount), 2)
            })
            .OrderByDescending(x => x.total_spent)
            .FirstOrDefaultAsync();
    }

    public async Task<JsonArray?> GetLineGraphExpenses(string userId, int currentMonth, int currentYear)
    {
        var daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);

        var lineGraphsExpenses = await _context.Expenses
            .Where(e => e.ExpenseDate.Month == currentMonth &&
                        e.ExpenseDate.Year == currentYear &&
                        e.Uid == userId)
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

        return lineGraphsExpensesFormatted;

    }
    public async Task<List<CircleGraphExpensesType>?> GetPieGraphExpenses(string userId, int currentMonth, int currentYear)
    {
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
            .Select(g => new CircleGraphExpensesType
            {
                SupplierName = g.Key.SupplierName,
                TotalAmount = g.Sum(e => e.TotalAmount),
                GrandTotal = g.Sum(e => e.TotalAmount) / totalAmountThisMonth
            })
            .OrderByDescending(x => x.GrandTotal)
            .ToListAsync();

        return circleGraphExpenses;

    }

    public async Task<PercentChangeType?> GetPercentChange(string userId, int currentMonth, int currentYear, double total_month, double projectedSpending)
    {
        // 1 to 12
        var lastMonth = currentMonth - 1;
        var lastYear = currentYear;
        if (lastMonth == 0)
        {
            lastMonth = 12;
            lastYear = lastYear - 1;
        }
        
        var lastMonthExpenses = await _context.Expenses
            .Where(e => e.Uid == userId &&
                        e.ExpenseDate.Month == lastMonth &&
                        e.ExpenseDate.Year == lastYear)
            .CountAsync();
        
        //  check if user has at least some expenses a m
        if (lastMonthExpenses > 0)
        {
            // amount spent this month
            var totalLastMonth = await GetAmountSpent(userId, lastMonth, lastYear);

            // projected spenting this month
            var lastMonthProjectedSpending = await GetProjectedAmountSpent(userId, lastMonth, lastYear);
            
            return (PercentChangeType?)new JsonResult(new PercentChangeType{ percentChangeTotal = (total_month - totalLastMonth) / totalLastMonth * 100, percentChangeProjected = (projectedSpending - lastMonthProjectedSpending) / lastMonthProjectedSpending * 100 }).Value;
        }
        else
        {
            return null;
        }

        
        
    }

    public async Task<object> Update(string userId)
    {
        // run aggregations on userId and update Redis

        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        // amount spent this month
        var total_month = await GetAmountSpent(userId, currentMonth, currentYear);

        // projected spenting this month
        var projectedSpending = await GetProjectedAmountSpent(userId, currentMonth, currentYear);

        // most spent supplier and amount
        var topSupplier = await GetTopSupplier(userId, currentMonth, currentYear);

        // line graph
        var lineGraphsExpensesFormatted = await GetLineGraphExpenses(userId, currentMonth, currentYear);

        // pie chart        
        var circleGraphExpenses = await GetPieGraphExpenses(userId, currentMonth, currentYear);

        // percent change
        var percentChange = await GetPercentChange(userId, currentMonth, currentYear, total_month ?? 0.00, projectedSpending ?? 0.00);              

        // update redis
        await SetUserCacheAsync(userId, total_month ?? 0.00, projectedSpending ?? 0.00, topSupplier?.supplier_name ?? "", topSupplier?.total_spent ?? 0.00, System.Text.Json.JsonSerializer.Serialize(lineGraphsExpensesFormatted) ?? "", System.Text.Json.JsonSerializer.Serialize(circleGraphExpenses) ?? "", percentChange?.percentChangeTotal ?? null, percentChange?.percentChangeProjected ?? null);

        return new
        {
            amountSpent = total_month ?? 0.00,
            amountProjected = projectedSpending ?? 0.00,
            topMerchant = topSupplier?.supplier_name ?? "",
            topMerchantAmount = topSupplier?.total_spent ?? 0.00,
            line_graph_expenses = lineGraphsExpensesFormatted ?? [],
            circle_graph_expenses = circleGraphExpenses ?? [],
            percent_change_total = percentChange?.percentChangeTotal ?? null,
            percent_change_projected = percentChange?.percentChangeProjected ?? null
        };

    }

}