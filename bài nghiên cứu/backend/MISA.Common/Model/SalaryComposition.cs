using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MISA.Common.Attributes;
using MISA.Common.Base;
using MISA.Common.Enum;
using MISA.Common.Resources;

namespace MISA.Common.Model;

/// <summary>
/// Bảng thành phần lương
/// </summary>
[ConfigTable("pa_salary_composition")]
public class SalaryComposition : BaseModel
{
    /// <summary>
    /// Khóa chính của bảng. Lưu dưới dạng UUID
    /// </summary>
    [Key]
    [ConfigColumn("salary_component_id")]
    public Guid SalaryComponentId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Mã thành phần.<br/>
    /// Unique, not null, có thể tìm kiếm
    /// </summary>
    [ConfigColumn("salary_component_code")]
    [ConfigSearchable]
    [Required]
    [MaxLength(255)]
    [CheckDuplicated(nameof(ResourcesVN.DuplicatedComponentCode))]
    public required string SalaryComponentCode { get; set; }

    /// <summary>
    /// Tên thành phần lương.<br/>
    /// Có thể tìm kiếm, not null
    /// </summary>
    [ConfigColumn("salary_component_name")]
    [ConfigSearchable]
    [Required]
    [MaxLength(255)]
    public required string SalaryComponentName { get; set; }

    /// <summary>
    /// Id của đơn vị áp dụng (Organization).<br/>
    /// Lưu dưới dạng UUID
    /// </summary>
    [ConfigColumn("applied_unit_id")]
    public Guid? AppliedUnitId { get; set; }

    /// <summary>
    /// Id của danh mục hệ thống (SalaryCompositionSystem).<br/>
    /// Lưu dưới dạng UUID
    /// </summary>
    [ConfigColumn("salary_component_system_id")]
    [Required]
    public Guid SalaryComponentSystemId { get; set; }

    /// <summary>
    /// Tính chất lương (0: Khác, 1: Thu nhập, 2: Khấu trừ)
    /// </summary>
    [ConfigColumn("attribute")]
    [Required]
    public SalaryAttribute Attribute { get; set; }

    /// <summary>
    /// Kiểu giá trị (0: Số, 1: Tiền tệ, 2: Chữ, 3: Ngày, 4: Phần trăm)
    /// </summary>
    [ConfigColumn("value_type")]
    public SalaryValueType? ValueType { get; set; }

    /// <summary>
    /// Giá trị.<br/>
    /// Lưu dưới dạng công thức (VD: =SUM(...))
    /// </summary>
    [ConfigColumn("value")]
    [MaxLength(255)]
    public string? Value { get; set; }

    /// <summary>
    /// Trạng thái (1 là đang theo dõi, 0 là ngừng theo dõi)
    /// </summary>
    [ConfigColumn("status")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// Nguồn tạo thành phần lương
    /// </summary>
    [ConfigColumn("source")]
    [MaxLength(50)]
    public string Source { get; set; } = "Tự thêm";

    /// <summary>
    /// Tên đơn vị áp dụng.<br/>
    /// Thuộc tính này khng lưu vào DB
    /// </summary>
    [NotMapped]
    public string? AppliedUnitName { get; set; }

    /// <summary>
    /// Tên danh mục hệ thống.<br/>
    /// Thuộc tính này khng lưu vào DB
    /// </summary>
    [NotMapped]
    public string? SalaryComponentSystemName { get; set; }
}