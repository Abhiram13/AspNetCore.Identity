using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Identity.Policies;

public class CompanyAdminRoleRequirement : IAuthorizationRequirement
{
    public string RequiredRole { get; }

    public CompanyAdminRoleRequirement(string requiredRole)
    {
        RequiredRole = requiredRole;
    }
}

public class CompanyAdminRoleHandler : AuthorizationHandler<CompanyAdminRoleRequirement>
{
    private readonly BusinessDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CompanyAdminRoleHandler(BusinessDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CompanyAdminRoleRequirement requirement)
    {
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            Console.WriteLine("User is not logged in");
            context.Fail();
            return;
        }
        
        HttpContext httpContext = _httpContextAccessor.HttpContext!;
        int companyId = 0;

        if (!httpContext.Request.RouteValues.TryGetValue("companyId", out object? routeId) || !int.TryParse(routeId?.ToString(), out companyId))
        {
             context.Fail();
             return;
        }
        
        Console.WriteLine("RouteID = {0} and CompanyId = {1}", routeId, companyId);
        
        string? userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine("UserID = {0}", userId);
        if (userId == null)
        {
            context.Fail();
            return;
        }
        
        int userRole = await _dbContext.CompanyUsers
             .Where(cu => cu.CompanyId == companyId && cu.UserId == int.Parse(userId!))
             .Select(cu => cu.RoleId)
             .FirstOrDefaultAsync();
        
        Console.WriteLine("UserRole = {0}", userRole);

         // 4. Validate against the requirement (e.g., Admin = 1, Member = 2)
         if (userRole == 1)
         {
             context.Succeed(requirement);
         }
    }
}