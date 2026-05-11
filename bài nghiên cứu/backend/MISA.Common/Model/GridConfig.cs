using System.ComponentModel.DataAnnotations;
using MISA.Common.Attributes;
using MISA.Common.Base;

namespace MISA.Common.Model;

[ConfigTable("pa_grid_config")]
public class GridConfig : BaseModel
{
    [Key] [ConfigColumn("grid_config_id")] 
    public Guid GridConfigId { get; set; } = Guid.NewGuid();

    [ConfigColumn("grid_id")] 
    [Required] 
    [MaxLength(100)] 
    public required string GridId { get; set; }

    [ConfigColumn("column_id")] 
    [Required] 
    [MaxLength(100)] 
    public required string ColumnId { get; set; }

    [ConfigColumn("column_name")] 
    [MaxLength(255)] 
    public string? ColumnName { get; set; }

    [ConfigColumn("is_visible")] 
    public int IsVisible { get; set; } = 1;

    [ConfigColumn("column_order")] 
    public int ColumnOrder { get; set; } = 0;

    [ConfigColumn("width")] 
    public int Width { get; set; } = 150;

    [ConfigColumn("is_pinned")] 
    public int IsPinned { get; set; } = 0;

    [ConfigColumn("pin_side")] 
    [MaxLength(10)] 
    public string PinSide { get; set; } = "left";
}
