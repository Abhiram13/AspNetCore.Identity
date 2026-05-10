using AspNetCore.Identity;
using Abhiram.Abstractions.Logging;
using Abhiram.Extensions.DotEnv;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using System.Text.Json;
using AspNetCore.Identity.Features.Companies.Repository;
using AspNetCore.Identity.Features.Companies.Services;
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
builder.Services.AddControllers().AddJsonOptions(option => option.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.ConfigureHttpJsonOptions(option => option.SerializerOptions.PropertyNamingPolicy = null); // TIP: This one for HttpContext.Response.WriteAsJsonAsync()
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
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                ApiResponse response = new ApiResponse
                {
                    Message = "You are not authorized. Token may be missing or invalid.",
                    StatusCode = HttpStatusCode.Unauthorized,
                };

                await context.Response.WriteAsJsonAsync(response);
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                ApiResponse response = new ApiResponse
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Message = "Access Denied: You do not have permission to perform this action."
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        };
    });
builder.Services.AddAuthorization(options =>
{
    // TIP: To prevent the policy check from running for anonymous users, setting explicit check to require authentication before checking the requirement.
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    
    options.AddPolicy(Policies.IS_COMPANY_ADMIN, policy =>
    {
        policy.Requirements.Add(new CompanyAdminRoleRequirement("Admin"));
    });
    
    options.AddPolicy(Policies.IS_SUPER_ADMIN, policy =>
    {
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