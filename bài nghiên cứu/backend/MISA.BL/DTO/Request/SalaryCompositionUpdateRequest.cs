namespace MISA.BL.DTO.Request;

public class SalaryCompositionUpdateRequest
{
    public required string SalaryComponentCode { get; set; }
    public required string SalaryComponentName { get; set; }
    public Guid? AppliedUnitId { get; set; }
    public required Guid SalaryComponentSystemId { get; set; }
    public int Attribute { get; set; }
    public int? ValueType { get; set; }
    public string? Value { get; set; }
    public int Status { get; set; }
    public string? Source { get; set; }
}
