using AspNetCore.Identity.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AspNetCore.Identity.Shared.Configurations;

public sealed class AuthorisationOptionsSetup : IConfigureOptions<AuthorizationOptions>
{
    public void Configure(AuthorizationOptions options)
    {
        // TIP: To prevent the policy check from running for anonymous users, setting explicit check to require authentication before checking the requirement.
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    
        options.AddPolicy(Constants.Policies.IS_COMPANY_ADMIN, policy =>
        {
            policy.Requirements.Add(new CompanyAdminRoleRequirement("Admin"));
        });
    
        options.AddPolicy(Constants.Policies.IS_SUPER_ADMIN, policy =>
        {
            policy.Requirements.Add(new SuperAdminRoleRequirement());
        });
    }
}