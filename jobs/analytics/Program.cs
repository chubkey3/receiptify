using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

class Program
{    
    static async Task Main(string[] args)
    {
        // Setup DI
        var services = new ServiceCollection();

        var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ?? "";

        services.AddDbContext<ReceiptifyContext>(options =>
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        // connect Redis to app
        var redisConnectionHost = Environment.GetEnvironmentVariable("REDIS_CONNECTION_HOST") ?? "";
        var redisConnectionPassword = Environment.GetEnvironmentVariable("REDIS_CONNECTION_PASSWORD") ?? "";

        var redisConfig = ConfigurationOptions.Parse(redisConnectionHost);
        redisConfig.Password = redisConnectionPassword;

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfig));

        // Add AnalyticsService
        services.AddScoped<AnalyticsService>();

        // Add logging (optional)
        services.AddLogging(config =>
        {            
            config.AddConsole();
            config.SetMinimumLevel(LogLevel.Information);
        });

        var serviceProvider = services.BuildServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        var context = serviceProvider.GetRequiredService<ReceiptifyContext>();
        var analyticsService = serviceProvider.GetRequiredService<AnalyticsService>();

        try
        {
            // Get all user IDs (or filter recent active users)
            var userIds = await context.Users
                //.Where(u => u.LastLogin >= DateTime.UtcNow.AddDays(-30)) // optional filter
                .Select(u => u.Uid)
                .ToListAsync();

            foreach (var userId in userIds)
            {
                logger.LogInformation($"Refreshing cache for user {userId}");
                await analyticsService.Update(userId);
            }

            logger.LogInformation("All user caches refreshed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error refreshing user caches.");
            Environment.ExitCode = 1;
        }
    }
}
