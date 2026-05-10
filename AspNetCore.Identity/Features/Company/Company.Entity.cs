using System.ComponentModel.DataAnnotations.Schema;
using AspNetCore.Identity.Shared.Constants;
using AspNetCore.Identity.Shared.Entities;

namespace AspNetCore.Identity.Features.Company.Entities;

[Table(DatabaseTables.Company.TABLE_NAME)]
public class Company : BaseEntity
{
    [Column(DatabaseTables.Company.NAME)]
    public string Name { get; private init; } = string.Empty;
    
    private Company() { }

    public static Company Create(string companyName)
    {
        Company company = new Company { Name = companyName };
        company.SetModifiedAt();
        
        return company; 
    }
}

[Table(DatabaseTables.CompanyUsers.TABLE_NAME)]
public sealed class CompanyUser
{
    [Column(DatabaseTables.CompanyUsers.COMPANY_ID)]
    public int CompanyId { get; private set; }
    
    [Column(DatabaseTables.CompanyUsers.USER_ID)]
    public int UserId { get; private set; }
    
    [Column(DatabaseTables.CompanyUsers.ROLE_ID)]
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