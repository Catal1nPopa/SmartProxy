using Microsoft.Extensions.Options;
using SyncNode.Services;
using SyncNode.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<EmployeeAPISettings>(builder.Configuration.GetSection("EmployeeAPISettings"));
builder.Services.AddSingleton<IEmployeeAPISettings>(provider =>
    provider.GetRequiredService<IOptions<EmployeeAPISettings>>().Value);

builder.Services.AddSingleton<SyncWorkJobService>();
builder.Services.AddHostedService(provider => provider.GetService<SyncWorkJobService>());

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
