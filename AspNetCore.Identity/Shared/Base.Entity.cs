using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AspNetCore.Identity.Shared.Entities;

public abstract class BaseEntity
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    /// <remarks>
    /// This property is immutable after initialization (<c>init</c>).
    /// </remarks>
    [Key]
    [Column("id")]
    public int Id { get; init; }

    /// <summary>
    /// Gets or sets the date when the entity was first created.
    /// </summary>
    /// <remarks>
    /// This property is immutable after initialization (<c>init</c>).
    /// </remarks>
    [Required]
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Gets or sets the date when the entity was last modified.
    /// </summary>
    [Required]
    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; private set; }
    
    protected void SetModifiedAt()
    {
        DateTimeOffset now =  DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }
    
    protected void SetUpdatedAt()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;
        UpdatedAt = now;
    }
}