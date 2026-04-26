using BookVault.API.Data;
using BookVault.API.Middleware;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BookVaultDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.

builder.Services.AddControllers();

//VALIDATION    

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseDefaultFiles();   // index.html jako strona domyœlna
app.UseStaticFiles();    // serwuje pliki z wwwroot

app.UseCors("AllowAll");

app.UseMiddleware<ExceptionMiddleware>();  // pierwszy!
app.UseCors("AllowAll");
//app.UseHttpsRedirection(); 
app.UseAuthorization();
app.MapControllers();
app.Run();



// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
