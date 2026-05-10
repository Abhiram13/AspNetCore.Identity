using Microsoft.AspNetCore.Authorization;

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
    
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CompanyAdminRoleRequirement requirement)
    {
        throw new NotImplementedException();
    }
}