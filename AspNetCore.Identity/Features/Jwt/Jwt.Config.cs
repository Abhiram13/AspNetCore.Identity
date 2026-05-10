using System.Net;
using System.Text;
using AspNetCore.Identity.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCore.Identity.Features.Jwt.Configuration;

public sealed class JwtOptionsSetup : IConfigureNamedOptions<JwtBearerOptions>
{
    public void Configure(JwtBearerOptions options)
    {
        Configure(Options.DefaultName, options);
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme) return;
        
        options.IncludeErrorDetails = true;
        options.TokenValidationParameters = GetTokenValidationParameters();
        options.Events = GetJwtBearerEvents();
    }

    private TokenValidationParameters GetTokenValidationParameters()
    {
        TokenValidationParameters parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = "User",
            ValidAudience = "identity",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("NT0wxMAKSD4u5UpEvGXxcpx+iKQszd7KwRHG9bJcrNc="))
        };
        
        return parameters;
    }

    private JwtBearerEvents GetJwtBearerEvents()
    {
        Func<JwtBearerChallengeContext, Task> OnChallangeContext = async context =>
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
        };

        Func<ForbiddenContext, Task> OnForbiddenContext = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            ApiResponse response = new ApiResponse
            {
                StatusCode = HttpStatusCode.Forbidden,
                Message = "Access Denied: You do not have permission to perform this action."
            };

            await context.Response.WriteAsJsonAsync(response);
        };
        
        JwtBearerEvents events = new JwtBearerEvents { OnChallenge = OnChallangeContext, OnForbidden = OnForbiddenContext };
        
        return events;
    }
}