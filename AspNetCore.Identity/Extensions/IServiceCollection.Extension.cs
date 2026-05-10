using AspNetCore.Identity.Features.Companies.Repository;
using AspNetCore.Identity.Features.Companies.Services;
using AspNetCore.Identity.Features.Jwt.Models;
using AspNetCore.Identity.Features.Jwt.Services;
using AspNetCore.Identity.Features.Role.Services;
using AspNetCore.Identity.Features.User.Services;
using AspNetCore.Identity.Policies;
using AspNetCore.Identity.Shared.Configurations;
using AspNetCore.Identity.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AspNetCore.Identity.Extensions;

public static class IServiceCollectionExtensions
{
    extension(IServiceCollection collection)
    {
        public IServiceCollection AddServices()
        {
            collection
                .AddLifeTimeServices()
                .AddOptionConfigurations()
                .AddDbContext()
                .AddIdentityServices()
                .AddJwtAuthentication()
                .AddAuthorizationConfiguration()
                .AddOtherServices();
            
            return collection;
        }

        private IServiceCollection AddLifeTimeServices()
        {
            collection
                .AddScoped<RoleService>()
                .AddScoped<JwtService>()
                .AddScoped<UserService>()
                .AddScoped<CompanyRepository>()
                .AddScoped<CompanyService>()
                .AddScoped<IAuthorizationHandler, CompanyAdminRoleHandler>()
                .AddScoped<IAuthorizationHandler, SuperAdminRoleHandler>();
            
            return collection;
        }

        private IServiceCollection AddOptionConfigurations()
        {
            collection.AddOptions<PostgresConnection>().BindConfiguration("Postgres").ValidateOnStart();
            collection.AddOptions<JwtConfiguration>().BindConfiguration("Jwt").ValidateOnStart();
            
            return collection;
        }

        private IServiceCollection AddDbContext()
        {
            collection.AddDbContext<UsersDBContext>((provider, options) =>
            {
                PostgresConnection conn = provider.GetRequiredService<IOptions<PostgresConnection>>().Value;
                options.UseNpgsql(conn.DbConnection);
            });

            collection.AddDbContext<BusinessDbContext>((provider, options) =>
            {
                PostgresConnection conn = provider.GetRequiredService<IOptions<PostgresConnection>>().Value;
                options.UseNpgsql(conn.DbConnection);
            });
            
            return collection;
        }

        private IServiceCollection AddIdentityServices()
        {
            Action<IdentityOptions> identityOptions = options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 2;
                options.Password.RequiredUniqueChars = 0;
            };
            
            collection.AddIdentity<ApplicationUser, ApplicationRole>(identityOptions)
                .AddEntityFrameworkStores<UsersDBContext>()
                .AddDefaultTokenProviders();
            
            return collection;
        }

        private IServiceCollection AddJwtAuthentication()
        {
            collection
                .ConfigureOptions<JwtConfiguration>()
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();
            
            return collection;
        }

        private IServiceCollection AddAuthorizationConfiguration()
        {
            collection
                .ConfigureOptions<AuthorisationOptionsSetup>()
                .AddAuthorization();
            
            return collection;
        }

        private IServiceCollection AddOtherServices()
        {
            collection
                .AddRouting()
                .ConfigureHttpJsonOptions(option => option.SerializerOptions.PropertyNamingPolicy = null) // TIP: This one for HttpContext.Response.WriteAsJsonAsync()
                .AddEndpointsApiExplorer()
                .AddSwaggerGen()
                .AddControllers()
                .AddJsonOptions(option => option.JsonSerializerOptions.PropertyNamingPolicy = null);
            
            return collection;
        }
    }
}