using Microsoft.EntityFrameworkCore;
using Notix.Api.Configuration;
using Notix.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Typed configuration (IOptions<T>) for every appsettings section.
builder.Services.AddNotixOptions(builder.Configuration, builder.Environment);

// SQLite through EF Core.
builder.Services.AddDbContext<NotixDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.LogNotixConfiguration();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
