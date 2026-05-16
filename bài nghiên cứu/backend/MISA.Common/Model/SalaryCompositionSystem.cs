using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    /// Mã thành phần hệ thống
    /// </summary>
    [ConfigColumn("salary_component_code")]
    [ConfigSearchable]
    public string? ComponentCode { get; set; }

    /// <summary>
    /// Tên danh mục hệ thống.<br/>
    /// Not null
    /// </summary>
    [ConfigColumn("salary_component_system_name")] 
    [ConfigSearchable]
    [Required] 
    [MaxLength(255)] 
    public required string SalaryComponentSystemName { get; set; }

    /// <summary>
    /// Tính chất (0: Khác, 1: Thu nhập, 2: Khấu trừ)
    /// </summary>
    [ConfigColumn("attribute")]
    public int Attribute { get; set; } = 0;

    /// <summary>
    /// Kiểu giá trị
    /// </summary>
    [ConfigColumn("value_type")]
    public int ValueType { get; set; } = 0;

    /// <summary>
    /// Giá trị mặc định
    /// </summary>
    [ConfigColumn("value")]
    public string Value { get; set; } = "-";

    /// <summary>
    /// Mô tả
    /// </summary>
    [ConfigColumn("description")] 
    public string? Description { get; set; }
}
