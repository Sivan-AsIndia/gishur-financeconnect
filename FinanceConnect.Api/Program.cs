using Microsoft.EntityFrameworkCore;
using FinanceConnect.Api.Data;
using FinanceConnect.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("AuthDb") 
    ?? "Data Source=FinanceConnectDB.db";
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseInMemoryDatabase("FinanceDb"));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<ClientSettings>(builder.Configuration.GetSection("ClientSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7001", "http://localhost:5001", "https://localhost:5002", "http://localhost:5002", "https://localhost:14826", "http://localhost:14825", "https://localhost:14829", "http://localhost:14830")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var authContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    authContext.Database.EnsureCreated();

    // Flush WAL to main DB file before repairing rows
    authContext.Database.ExecuteSqlRaw("PRAGMA wal_checkpoint(TRUNCATE)");

    // Fix rows where CreatedAt is NULL or empty string (SQLite dynamic typing quirk)
    authContext.Database.ExecuteSqlRaw(
        "UPDATE Users SET CreatedAt = datetime('now') WHERE CreatedAt IS NULL OR TRIM(CAST(CreatedAt AS TEXT)) = ''");

    var financeContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
    financeContext.Database.EnsureCreated();
}

app.Run();
