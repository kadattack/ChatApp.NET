using System.Text;
using ChatApp.Controllers;
using ChatApp.Data;
using ChatApp.Data.Migrations;
using ChatApp.Entities;
using ChatApp.Interfaces;
using ChatApp.Services;
using ChatApp.SignalR;
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

builder.Services.AddSignalR();

builder.Services.AddDbContext<DataContext>(
    options =>
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        string connStr;

        // Depending on if in development or production, use either Heroku-provided
        // connection string, or development connection string from env var.
        if (env == "Development")
        {
            // Use connection string from file.
            connStr = config.GetConnectionString("DefaultConnection");
        }
        else
        {
            // Use connection string provided at runtime by Heroku.
            var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            // Parse connection URL to connection string for Npgsql
            connUrl = connUrl.Replace("postgres://", string.Empty);
            var pgUserPass = connUrl.Split("@")[0];
            var pgHostPortDb = connUrl.Split("@")[1];
            var pgHostPort = pgHostPortDb.Split("/")[0];
            var pgDb = pgHostPortDb.Split("/")[1];
            var pgUser = pgUserPass.Split(":")[0];
            var pgPass = pgUserPass.Split(":")[1];
            var pgHost = pgHostPort.Split(":")[0];
            var pgPort = pgHostPort.Split(":")[1];

            connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};SSL Mode=Require;TrustServerCertificate=True";
        }

        // Whether the connection string came from the local development configuration file
        // or from the environment variable from Heroku, use it to set up your DbContext.
        options.UseNpgsql(connStr);
    }
);



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>{

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
    // We have to get the token from the query string since we can't get it from the header for SignalR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
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


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);




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

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

// Looks in wwwroot for static files
app.UseDefaultFiles();
app.UseStaticFiles();

// app.UseEndpoints(endpoints => { endpoints.MapControllers(); }

app.MapControllers(); // Maps urls with controllers

// Fallback is the name of the FallbackController.cs
app.MapHub<PresenceHub>("hubs/presence");

app.MapFallbackToController("Index", "Fallback");
app.MapHub<MessageHub>("hubs/message");

// app.Run();
await app.RunAsync();
