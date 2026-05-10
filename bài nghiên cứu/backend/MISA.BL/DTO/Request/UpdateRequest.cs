using MISA.Common.Model;
using ValueType = MISA.Common.Model.ValueType;

namespace MISA.BL.DTO.Request;

public class UpdateRequest
{
    public required string ComponentCode { get; set; }
    public required string ComponentName { get; set; }
    public required string AppliedUnit { get; set; }
    public required ComponentType ComponentType { get; set; }
    public required MISA.Common.Model.Attribute Attribute { get; set; }
    public required ValueType ValueType { get; set; }
    public string? Value { get; set; }
}