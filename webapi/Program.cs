using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using StackExchange.Redis;
using System.Management;

var builder = WebApplication.CreateBuilder(args);

// connect DB to app
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";

builder.Services.AddDbContext<ReceiptifyContext>(options =>
options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// connect Redis to app
var config = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("RedisConnectionHost") ?? "");
config.Password = builder.Configuration.GetConnectionString("RedisConnectionPassword") ?? "";

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(config));


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// add services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ReceiptService>();
builder.Services.AddScoped<ExpenseService>();
builder.Services.AddScoped<SupplierService>();
builder.Services.AddScoped<UploadService>();
builder.Services.AddScoped<AnalyticsService>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(p => p.AddPolicy("CORSPolicy", builder =>
    {
        builder.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    }));
}
else
{
    builder.Services.AddCors(p => p.AddPolicy("CORSPolicy", builder =>
    {
        // please remove url soon
        builder.WithOrigins("https://receiptify-frontend-573080908500.us-central1.run.app").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    }));
}




var app = builder.Build();

// connect to firebase admin
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("firebase-service-account.json")
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.UseMiddleware<ParseCookieMiddleware>();

app.MapControllers();

app.UseCors("CORSPolicy");

app.CreateDbIfNotExists();

app.Run();
