using AspNetCore.Identity;
using Abhiram.Abstractions.Logging;
using Abhiram.Extensions.DotEnv;
using Microsoft.EntityFrameworkCore;
using System.Net;
using AspNetCore.Identity.Features.Company.Repository;
using AspNetCore.Identity.Features.Company.Services;
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
builder.Services.AddScoped<CompanyRepository>();
builder.Services.AddScoped<CompanyService>();
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
builder.Services.AddAuthentication().AddJwtBearer();

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

// public class CommunityRoleRequirement : IAuthorizationRequirement
// {
//     public string RequiredRole { get; }
//     public CommunityRoleRequirement(string role) => RequiredRole = role;
// }

// public class CommunityRoleHandler : AuthorizationHandler<CommunityRoleRequirement>
// {
//     private readonly BusinessDbContext _db;
//     private readonly IHttpContextAccessor _httpContextAccessor;
//
//     public CommunityRoleHandler(BusinessDbContext db, IHttpContextAccessor httpContextAccessor)
//     {
//         _db = db;
//         _httpContextAccessor = httpContextAccessor;
//     }
//
//     protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CommunityRoleRequirement requirement)
//     {
//         var httpContext = _httpContextAccessor.HttpContext;
//         
//         // 1. Get Community ID from the Route (e.g., /api/communities/{id}/posts)
//         if (!httpContext.Request.RouteValues.TryGetValue("id", out var routeId) || 
//             !int.TryParse(routeId?.ToString(), out int communityId))
//         {
//             return; // No ID found, let other handlers deal with it or fail
//         }
//
//         // 2. Get User ID from JWT
//         var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//         if (userId == null) return;
//
//         // 3. Check the Business Schema table
//         var userRole = await _db.CommunityUsers
//             .Where(cu => cu.CommunityId == communityId && cu.UserId == userId)
//             .Select(cu => cu.RoleId) // Assuming you used an int RoleId
//             .FirstOrDefaultAsync();
//
//         // 4. Validate against the requirement (e.g., Admin = 1, Member = 2)
//         if (userRole != 0 && userRole <= GetRoleIdValue(requirement.RequiredRole))
//         {
//             context.Succeed(requirement);
//         }
//     }
// }