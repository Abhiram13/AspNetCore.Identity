using System.ComponentModel.DataAnnotations.Schema;
using AspNetCore.Identity.Shared.Constants;
using AspNetCore.Identity.Shared.Entities;

namespace AspNetCore.Identity.Features.Company.Entities;

[Table(DatabaseTables.Community.TABLE_NAME)]
public sealed class Community : BaseEntity
{
    [Column(DatabaseTables.Community.NAME)]
    public required string Name { get; set; }
    
    [Column(DatabaseTables.Community.PARENT_COMPANY_ID)]
    public int ParentCompanyId { get; set; }
    
    [ForeignKey(nameof(ParentCompanyId))]
    public Company? ParentCompanyF { get; set; }
}

[Table(DatabaseTables.CommunityUsers.TABLE_NAME)]
public sealed class CommunityUser
{
    [Column(DatabaseTables.CommunityUsers.COMMUNITY_ID)]
    public required int CommunityId { get; set; }
    
    [Column(DatabaseTables.CommunityUsers.USER_ID)]
    public required int UserId { get; set; }
    
    [Column(DatabaseTables.CommunityUsers.ROLE_ID)]
    public required int RoleId { get; set; }
    
    [ForeignKey(nameof(CommunityId))]
    public Community? CommunityF { get; set; }
}