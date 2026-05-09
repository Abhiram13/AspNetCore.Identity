using AspNetCore.Identity.Features.Company.Entities;
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


public class BusinessDbContext : DbContext
{
    public BusinessDbContext(DbContextOptions<BusinessDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Business");
        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("companies"); // create table if not exist
            entity.HasKey(e => e.Id); // setting id as primary key
        });
        modelBuilder.Entity<CompanyUser>(entity =>
        {
            entity.ToTable("company_users"); // create table if not exist
            entity.HasKey(e => new { e.CompanyId, e.UserId }); // Composite Key remains (CompanyId + UserId). This prevents a user from having multiple entries in the same company.
            entity.Property(e => e.RoleId).HasColumnName("role_id").IsRequired();
            entity.HasOne(d => d.CompanyF).WithMany().HasForeignKey(d => d.CompanyId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<CommunityUser>(entity =>
        {
            entity.ToTable("community_users"); // create table if not exist
            entity.HasKey(e => new { e.CommunityId, e.UserId }); // Composite Key remains (CommunityId + UserId). This prevents a user from having multiple entries in the same company.
            entity.Property(e => e.RoleId).HasColumnName("role_id").IsRequired();
            entity.HasOne(d => d.CommunityF).WithMany().HasForeignKey(d => d.CommunityId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<Community>(entity =>
        {
            entity.ToTable("communities");
            entity.HasKey(e => e.Id);
            entity.HasOne(d => d.ParentCompanyF).WithMany().HasForeignKey(d => d.ParentCompanyId).OnDelete(DeleteBehavior.Restrict); 
        });
    }
}