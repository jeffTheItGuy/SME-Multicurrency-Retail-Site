using Microsoft.EntityFrameworkCore;
using SmeRetail.Data;
using SmeRetail.Services;

var builder = WebApplication.CreateBuilder(args);

// MySQL
var connString = builder.Configuration.GetConnectionString("Default") 
    ?? "Server=localhost;Database=smeretail;Uid=root;Pwd=;";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connString, ServerVersion.AutoDetect(connString)));

// Services
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<IVatCalculator, VatCalculator>();
builder.Services.AddScoped<IInventoryValuationService, InventoryValuationService>();
builder.Services.AddScoped<IFiscalisationService, FiscalisationService>();
builder.Services.AddSingleton<IMeilisearchService, MeilisearchService>();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Ensure DB + seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var exchange = scope.ServiceProvider.GetRequiredService<IExchangeRateService>();
    var search = scope.ServiceProvider.GetRequiredService<IMeilisearchService>();

    try
    {
        db.Database.Migrate();
        await exchange.SeedRatesAsync();
        // Optionally reindex on startup: await search.IndexAllProductsAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Startup seeding failed");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
