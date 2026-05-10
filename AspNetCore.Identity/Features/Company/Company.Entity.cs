using System.ComponentModel.DataAnnotations.Schema;
using AspNetCore.Identity.Shared.Entities;

namespace AspNetCore.Identity.Features.Company.Entities;

[Table("companies")]
public class Company : BaseEntity
{
    [Column("name")]
    public string Name { get; private init; } = string.Empty;
    
    private Company() { }

    public static Company Create(string companyName)
    {
        Company company = new Company { Name = companyName };
        company.SetModifiedAt();
        
        return company; 
    }
}

[Table("company_users")]
public sealed class CompanyUser
{
    [Column("company_id")]
    public int CompanyId { get; private set; }
    
    [Column("user_id")]
    public int UserId { get; private set; }
    
    [Column("role_id")]
    public int RoleId { get; private set; }
    
    [ForeignKey(nameof(CompanyId))]
    public Community? CompanyF { get; set; }
    
    private CompanyUser() { }

    public static CompanyUser Create(int companyId, int userId, int roleId)
    {
        CompanyUser companyUser = new CompanyUser
        {
            CompanyId = companyId,
            RoleId = roleId,
            UserId = userId
        };
        
        return companyUser;
    }
}