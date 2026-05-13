using System.ComponentModel.DataAnnotations;
using MISA.Common.Attributes;
using MISA.Common.Base;

namespace MISA.Common.Model;

/// <summary>
/// Bảng đơn vị áp dụng
/// </summary>
[ConfigTable("pa_organization")]
public class Organization : BaseModel
{
    /// <summary>
    /// Khóa chính của bảng.<br/>
    /// Id của đơn vị áp dụng.<br/>
    /// Lưu dưới dạng UUID
    /// </summary>
    [Key]
    [ConfigColumn("organization_id")]
    public Guid OrganizationId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Mã đơn vị áp dụng.<br/>
    /// Not null
    /// </summary>
    [ConfigColumn("organization_code")]
    [Required]
    [MaxLength(50)]
    public required string OrganizationCode { get; set; }

    /// <summary>
    /// Tên đơn vị áp dụng.<br/>
    /// Not null
    /// </summary>
    [ConfigColumn("organization_name")]
    [Required]
    [MaxLength(255)]
    public required string OrganizationName { get; set; }

    /// <summary>
    /// Khóa ngoại của đơn vị áp dụng (cha)
    /// </summary>
    [ConfigColumn("parent_id")]
    public Guid? ParentId { get; set; }
}