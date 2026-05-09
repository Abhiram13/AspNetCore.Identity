using AspNetCore.Identity;
using Abhiram.Abstractions.Logging;
using Abhiram.Extensions.DotEnv;
using Microsoft.EntityFrameworkCore;
using System.Net;
using AspNetCore.Identity.Features.Jwt.Models;
using AspNetCore.Identity.Features.Jwt.Services;
using AspNetCore.Identity.Features.Role.Services;
using AspNetCore.Identity.Features.User.Services;
using AspNetCore.Identity.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
DotEnvironmentVariables.Load();

builder.AddConsoleGoogleSeriLog();
builder.Services.AddRouting();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptions<PostgresConnection>().BindConfiguration("Postgres").ValidateOnStart();
builder.Services.AddOptions<JwtConfiguration>().BindConfiguration("Jwt").ValidateOnStart();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddDbContext<UsersDBContext>((provider, options) =>
{
    PostgresConnection conn = provider.GetRequiredService<IOptions<PostgresConnection>>().Value;
    options.UseNpgsql(conn.DbConnection);
});

builder.Services.AddDbContext<BusinessDbContext>((provider, options) =>
{
    PostgresConnection conn = provider.GetRequiredService<IOptions<PostgresConnection>>().Value;
    options.UseNpgsql(conn.DbConnection);
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 2;
        options.Password.RequiredUniqueChars = 0;
    })
    .AddEntityFrameworkStores<UsersDBContext>()
    .AddDefaultTokenProviders();

builder.WebHost.ConfigureKestrel((_, server) =>
{
    string portNumber = Environment.GetEnvironmentVariable("PORT") ?? "3000";
    int port = int.Parse(portNumber);
    server.Listen(IPAddress.Any, port);
});

WebApplication app = builder.Build();

using (IServiceScope? scope = app.Services.CreateScope())
{
    ILogger<Program> logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Starting DB Migration...");
        UsersDBContext context = scope.ServiceProvider.GetRequiredService<UsersDBContext>();
        context.Database.Migrate();
        logger.LogInformation("DB Migration completed.");
    }
    catch (Exception e)
    {
        logger.LogCritical(e, "An error occurred while migrating the DB.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();
app.Run();