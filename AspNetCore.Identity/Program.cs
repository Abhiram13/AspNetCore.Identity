using AspNetCore.Identity;
using Abhiram.Abstractions.Logging;
using Abhiram.Extensions.DotEnv;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using AspNetCore.Identity.Features.Company.Repository;
using AspNetCore.Identity.Features.Company.Services;
using AspNetCore.Identity.Features.Jwt.Models;
using AspNetCore.Identity.Features.Jwt.Services;
using AspNetCore.Identity.Features.Role.Services;
using AspNetCore.Identity.Features.User.Services;
using AspNetCore.Identity.Policies;
using AspNetCore.Identity.Shared.Constants;
using AspNetCore.Identity.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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
builder.Services.AddScoped<CompanyRepository>();
builder.Services.AddScoped<CompanyService>();
builder.Services.AddScoped<IAuthorizationHandler, CompanyAdminRoleHandler>();
builder.Services.AddScoped<IAuthorizationHandler, SuperAdminRoleHandler>();
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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = "User",
            ValidAudience = "identity",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("NT0wxMAKSD4u5UpEvGXxcpx+iKQszd7KwRHG9bJcrNc="))
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.IS_COMPANY_ADMIN, policy =>
    {
        policy.RequireAuthenticatedUser(); // To prevent the policy check from running for anonymous users, setting explict check to require authentication before checking the requirement.
        policy.Requirements.Add(new CompanyAdminRoleRequirement("Admin"));
    });
    
    options.AddPolicy(Policies.IS_SUPER_ADMIN, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.Requirements.Add(new SuperAdminRoleRequirement());
    });
});

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

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();