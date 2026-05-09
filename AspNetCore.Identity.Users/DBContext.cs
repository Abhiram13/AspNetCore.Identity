using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Identity;

// 1. Define custom classes with correct base inheritance
public class ApplicationUser : IdentityUser<int> { }

public class ApplicationRole : IdentityRole<int> { }

public class ApplicationUserRole : IdentityUserRole<int> { }

public class ApplicationUserClaim : IdentityUserClaim<int> { }

public class ApplicationUserLogin : IdentityUserLogin<int> { }

public class ApplicationRoleClaim : IdentityRoleClaim<int> { }

public class ApplicationUserToken : IdentityUserToken<int> { }

// 2. Define the DbContext using all custom classes
public class UsersDBContext : IdentityDbContext<
    ApplicationUser, 
    ApplicationRole,
    int, 
    ApplicationUserClaim, 
    ApplicationUserRole, 
    ApplicationUserLogin, 
    ApplicationRoleClaim, 
    ApplicationUserToken>
{
    public UsersDBContext(DbContextOptions<UsersDBContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 3. Rename tables using YOUR custom classes
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<ApplicationRole>().ToTable("Roles");
        builder.Entity<ApplicationUserRole>().ToTable("UserRoles");
        builder.Entity<ApplicationUserClaim>().ToTable("UserClaims");
        builder.Entity<ApplicationUserLogin>().ToTable("UserLogins");
        builder.Entity<ApplicationRoleClaim>().ToTable("RoleClaims");
        builder.Entity<ApplicationUserToken>().ToTable("UserTokens");
    }
}