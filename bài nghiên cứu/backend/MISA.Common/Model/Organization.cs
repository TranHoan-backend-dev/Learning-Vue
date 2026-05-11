using System.ComponentModel.DataAnnotations;
using MISA.Common.Attributes;
using MISA.Common.Base;

namespace MISA.Common.Model;

[ConfigTable("pa_organization")]
public class Organization : BaseModel
{
    [Key] [ConfigColumn("organization_id")] public Guid OrganizationId { get; set; } = Guid.NewGuid();

    [ConfigColumn("organization_code")] 
    [Required] 
    [MaxLength(50)] 
    public required string OrganizationCode { get; set; }

    [ConfigColumn("organization_name")] 
    [Required] 
    [MaxLength(255)] 
    public required string OrganizationName { get; set; }

    [ConfigColumn("parent_id")] 
    public Guid? ParentId { get; set; }
}
