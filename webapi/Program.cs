using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

// connect DB to app
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ReceiptifyContext>(options =>
options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// add services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ReceiptService>();
builder.Services.AddScoped<ExpenseService>();
builder.Services.AddScoped<SupplierService>();
builder.Services.AddScoped<FirebaseAuthService>();
builder.Services.AddScoped<UploadService>();

builder.Services.AddCors(p => p.AddPolicy("testing", builder =>
{
    builder.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
}));

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

app.UseAuthorization();

app.MapControllers();

app.UseCors("testing");

app.CreateDbIfNotExists();

app.Run();
