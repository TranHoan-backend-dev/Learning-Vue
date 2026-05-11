namespace MISA.BL.DTO.Response;

public class SalaryCompositionResponse
{
    public Guid SalaryComponentId { get; set; }
    public required string SalaryComponentCode { get; set; }
    public required string SalaryComponentName { get; set; }
    public Guid? AppliedUnitId { get; set; }
    public required Guid SalaryComponentSystemId { get; set; }
    public int Attribute { get; set; }
    public int? ValueType { get; set; }
    public string? Value { get; set; }
    public int Status { get; set; }
    public string? Source { get; set; }
    public string? AppliedUnitName { get; set; }
    public string? SalaryComponentSystemName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}
