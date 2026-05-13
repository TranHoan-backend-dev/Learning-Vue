using System.ComponentModel.DataAnnotations;
using MISA.Common.Attributes;
using MISA.Common.Base;

namespace MISA.Common.Model;

/// <summary>
/// Bảng cấu hình bảng cho front-end
/// </summary>
[ConfigTable("pa_grid_config")]
public class GridConfig : BaseModel
{
    /// <summary>
    /// Khóa chính của bảng.<br/>
    /// Lưu dưới dạng UUID
    /// </summary>
    [Key] [ConfigColumn("grid_config_id")] 
    public Guid GridConfigId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Mã của bảng.<br/>
    /// Not null
    /// </summary>
    [ConfigColumn("grid_id")]
    [Required]
    [MaxLength(100)]
    public required string GridId { get; set; }

    /// <summary>
    /// Id của cột.<br/>
    /// Not null
    /// </summary>
    [ConfigColumn("column_id")]
    [Required]
    [MaxLength(100)]
    public required string ColumnId { get; set; }

    /// <summary>
    /// Tên cột.<br/>
    /// </summary>
    [ConfigColumn("column_name")]
    [MaxLength(255)]
    public string? ColumnName { get; set; }

    /// <summary>
    /// Trạng thái hiển thị hoặc không
    /// </summary>
    [ConfigColumn("is_visible")]
    [Required]
    public int IsVisible { get; set; } = 1;

    [ConfigColumn("column_order")] public int ColumnOrder { get; set; } = 0;

    /// <summary>
    /// Độ rộng của cột
    /// </summary>
    [ConfigColumn("width")] public int Width { get; set; } = 150;

    /// <summary>
    /// Trạng thái ghim cột
    /// </summary>
    [ConfigColumn("is_pinned")] public int IsPinned { get; set; } = 0;

    /// <summary>
    /// Vị trí ghim cột trái hoặc phải
    /// </summary>
    [ConfigColumn("pin_side")]
    [MaxLength(10)]
    public string PinSide { get; set; } = "left";
}