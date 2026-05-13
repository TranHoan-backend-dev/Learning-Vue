using System.ComponentModel.DataAnnotations;
using MISA.Common.Attributes;
using MISA.Common.Base;

namespace MISA.Common.Model;

/// <summary>
/// Danh mục hệ thống
/// </summary>
[ConfigTable("pa_salary_composition_system")]
public class SalaryCompositionSystem : BaseModel
{
    /// <summary>
    /// Khóa chính của bảng.<br/>
    /// Lưu dưới dạng UUID
    /// </summary>
    [Key] [ConfigColumn("salary_component_system_id")] 
    public Guid SalaryComponentSystemId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Tên danh mục hệ thống.<br/>
    /// Not null
    /// </summary>
    [ConfigColumn("salary_component_system_name")] 
    [Required] 
    [MaxLength(255)] 
    public required string SalaryComponentSystemName { get; set; }

    /// <summary>
    /// Mô tả
    /// </summary>
    [ConfigColumn("description")] 
    public string? Description { get; set; }
}
