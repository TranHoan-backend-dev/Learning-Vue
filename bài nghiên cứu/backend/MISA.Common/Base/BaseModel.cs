using MISA.Common.Attributes;

namespace MISA.Common.Base;

public class BaseModel
{
    [ConfigColumn("created_by")] public string? CreatedBy { get; set; }

    [ConfigColumn("created_at")] public DateTime CreatedAt { get; set; }

    [ConfigColumn("modified_by")] public string? ModifiedBy { get; set; }

    [ConfigColumn("modified_at")] public DateTime ModifiedAt { get; set; }
    
    // public 
}