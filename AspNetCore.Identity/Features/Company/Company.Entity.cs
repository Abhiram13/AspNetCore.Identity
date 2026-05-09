using System.ComponentModel.DataAnnotations.Schema;
using AspNetCore.Identity.Shared.Entities;

namespace AspNetCore.Identity.Features.Company.Entities;

[Table("companies")]
public sealed class Company : BaseEntity
{
    [Column("name")]
    public required string Name { get; init; }
}

[Table("company_users")]
public sealed class CompanyUser
{
    [Column("company_id")]
    public required int CompanyId { get; set; }
    
    [Column("user_id")]
    public required int UserId { get; set; }
    
    [Column("role_id")]
    public required int RoleId { get; set; }
    
    [ForeignKey(nameof(CompanyId))]
    public Community? CompanyF { get; set; }
}