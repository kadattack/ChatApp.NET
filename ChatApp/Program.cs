using System.Text;
using ChatApp.Data;
using ChatApp.Data.Migrations;
using ChatApp.Entities;
using ChatApp.Interfaces;
using ChatApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var config = builder.Configuration;



// builder.Services.AddApplicationServices(config);

// This creates a service that is running only for the time it's being used
builder.Services.AddScoped<ITokenService, TokenService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

builder.Services.AddIdentityCore<AppUsers>(opt =>
    {
        opt.Password.RequireNonAlphanumeric = false;
    }).AddRoles<AppRole>()
    .AddRoleManager<RoleManager<AppRole>>().AddSignInManager<SignInManager<AppUsers>>()
    .AddRoleValidator<RoleValidator<AppRole>>().AddEntityFrameworkStores<DataContext>();

builder.Services.AddDbContext<DataContext>(
    options =>
    {
        options.UseSqlite(config.GetConnectionString("DefaultConnection"));
    }
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
        ValidateIssuer = false,
        ValidateAudience = false,
    });

// builder.Services.AddApplicationServices(_config);

var app = builder.Build();






// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    var userManager = services.GetRequiredService<UserManager<AppUsers>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    // await FullTableData.FillTables(userManager, roleManager, context);
    await FullTableData.FillRoles(userManager, roleManager, context);
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}


app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

// Looks in wwwroot for static files
app.UseDefaultFiles();
app.UseStaticFiles();

// app.UseEndpoints(endpoints => { endpoints.MapControllers(); }

app.MapControllers(); // Maps urls with controllers

// Fallback is the name of the FallbackController.cs
app.MapFallbackToController("Index", "Fallback");

// app.Run();
await app.RunAsync();
