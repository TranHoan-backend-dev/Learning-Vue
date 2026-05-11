using System.ComponentModel.DataAnnotations;
using MISA.Common.Attributes;
using MISA.Common.Base;
using MISA.Common.Resources;

namespace MISA.Common.Model;

[ConfigTable("pa_salary_composition")]
public class SalaryComposition : BaseModel
{
    [Key] [ConfigColumn("salary_component_id")] public Guid SalaryComponentId { get; set; } = Guid.NewGuid();

    /**
     * Mã thành phần
     */
    [ConfigColumn("salary_component_code")]
    [ConfigSearchable]
    [Required]
    [MaxLength(255)]
    [CheckDuplicated(nameof(ResourcesVN.DuplicatedComponentCode))]
    public required string SalaryComponentCode { get; set; }

    /**
     * Tên thành phần
     */
    [ConfigColumn("salary_component_name")]
    [ConfigSearchable]
    [Required]
    [MaxLength(255)]
    public required string SalaryComponentName { get; set; }

    /**
     * Đơn vị áp dụng
     */
    [ConfigColumn("applied_unit_id")]
    public Guid? AppliedUnitId { get; set; }

    /**
     * Danh mục hệ thống
     */
    [ConfigColumn("salary_component_system_id")]
    [Required]
    public Guid SalaryComponentSystemId { get; set; }

    /**
     * Tính chất
     */
    [ConfigColumn("attribute")]
    [Required]
    public int Attribute { get; set; }

    /**
     * Kiểu giá trị
     */
    [ConfigColumn("value_type")]
    public int? ValueType { get; set; }

    /**
     * Giá trị
     */
    [ConfigColumn("value")]
    [MaxLength(255)]
    public string? Value { get; set; }

    /**
     * Trạng thái
     */
    [ConfigColumn("status")]
    public int Status { get; set; } = 1;

    /**
     * Nguồn tạo
     */
    [ConfigColumn("source")]
    [MaxLength(50)]
    public string Source { get; set; } = "Tự thêm";
}
