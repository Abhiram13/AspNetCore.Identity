using System.ComponentModel.DataAnnotations.Schema;
using AspNetCore.Identity.Shared.Entities;

namespace AspNetCore.Identity.Features.Company.Entities;

[Table("communities")]
public sealed class Community : BaseEntity
{
    [Column("name")]
    public required string Name { get; set; }
    
    [Column("parent_company_id")]
    public int ParentCompanyId { get; set; }
    
    [ForeignKey(nameof(ParentCompanyId))]
    public Company? ParentCompanyF { get; set; }
}

[Table("community_users")]
public sealed class CommunityUser
{
    [Column("community_id")]
    public required int CommunityId { get; set; }
    
    [Column("user_id")]
    public required int UserId { get; set; }
    
    [Column("role_id")]
    public required int RoleId { get; set; }
    
    [ForeignKey(nameof(CommunityId))]
    public Community? CommunityF { get; set; }
}