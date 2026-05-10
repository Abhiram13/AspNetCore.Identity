using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Identity.Policies;

public class SuperAdminRoleRequirement : IAuthorizationRequirement
{
    public string RequiredRole = "SuperAdmin";
}

public class SuperAdminRoleHandler : AuthorizationHandler<SuperAdminRoleRequirement>
{
    private readonly UsersDBContext _dbContext;

    public SuperAdminRoleHandler(UsersDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SuperAdminRoleRequirement requirement)
    {
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            Console.WriteLine("User is not logged in");
            context.Fail();
            return;
        }
        
        string? role = context.User.FindFirst(ClaimTypes.Role)?.Value;
        string? userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        bool exists = await _dbContext.UserRoles
            .Join(
                _dbContext.Roles,
                userRole => userRole.RoleId,
                role => role.Id,
                (userRole, role) => new { userRole, role })
            .AnyAsync(x => x.userRole.UserId == int.Parse(userId!) && x.role.Name == "SuperAdmin");
        
        Console.WriteLine("Exists? = {0}", exists);
        if (!exists)
        {
            context.Fail();
            return;
        }

        if (role == requirement.RequiredRole)
        {
             context.Succeed(requirement);
        }
    }
}