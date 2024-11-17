using Common.Models;
using EmployeeAPI.Repositories;
using EmployeeAPI.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<PostgresDbSettings>(builder.Configuration.GetSection("PostgresDbSettings"));
builder.Services.AddSingleton<IPostgresDbSettings>(provider =>
    provider.GetRequiredService<IOptions<PostgresDbSettings>>().Value);

//builder.WebHost.UseKestrel(options =>
//{
//    options.AddServerHeader = false; 
//});

builder.Services.AddScoped<IPostgresRepository<Employee>, PostgresRepository<Employee>>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
