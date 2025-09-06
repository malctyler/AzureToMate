using Microsoft.EntityFrameworkCore;
using test_webapi;
using test_webapi.Data;
using test_webapi.Repositories;
using test_webapi.Services;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// ...existing code...
var defaultConnection = Environment.GetEnvironmentVariable("DefaultConnection") 
                        ?? Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DefaultConnection");

if (string.IsNullOrEmpty(defaultConnection))
{
    throw new InvalidOperationException("DefaultConnection environment variable is not set.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        defaultConnection,
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));
// ...existing code...
// // Add services to the container.
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseSqlServer(
//         builder.Configuration.GetConnectionString("DefaultConnection"),
//         sqlOptions => sqlOptions.EnableRetryOnFailure()
//     ));
builder.Services.AddScoped<ISummaryRepository, SummaryRepository>();
builder.Services.AddScoped<ISummaryService, SummaryService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "azuretomate v1");
        c.RoutePrefix = string.Empty; // Set Swagger as the default start page
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
