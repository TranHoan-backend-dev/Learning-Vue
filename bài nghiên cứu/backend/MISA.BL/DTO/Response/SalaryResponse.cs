using MISA.Common.Model;
using Attribute = MISA.Common.Model.Attribute;
using ValueType = MISA.Common.Model.ValueType;

namespace MISA.BL.DTO.Response;

public class SalaryResponse
{
    public Guid ComponentId { get; set; }
    public required string ComponentCode { get; set; }
    public required string ComponentName { get; set; }
    public required string AppliedUnit { get; set; }
    public required ComponentType ComponentType { get; set; }
    public required Attribute Attribute { get; set; }
    public required ValueType ValueType { get; set; }
    public string? Value { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}