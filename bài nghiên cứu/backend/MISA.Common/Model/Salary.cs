using System.ComponentModel.DataAnnotations;
using MISA.Common.Attributes;
using MISA.Common.Base;
using MISA.Common.Resources;

namespace MISA.Common.Model;

[ConfigTable("salary")]
public class Salary : BaseModel
{
    [Key] [ConfigColumn("component_id")] public Guid ComponentId { get; set; } = Guid.NewGuid();

    /**
     * Mã thành phần
     */

    [ConfigColumn("component_code")]
    [ConfigSearchable]
    [CheckDuplicated(nameof(ResourcesVN.DuplicatedComponentCode))]
    public required string ComponentCode { get; set; }

    /**
     * Tên thành phần
     */
    [ConfigColumn("component_name")]
    [ConfigSearchable]
    public required string ComponentName { get; set; }

    /**
     * Đơn vị áp dụng
     */
    [ConfigColumn("applied_unit")]
    public required string AppliedUnit { get; set; }

    /**
     * Loại thành phần
     */
    [ConfigColumn("component_type")]
    public required ComponentType ComponentType { get; set; }

    /**
     * Tính chất
     */
    [ConfigColumn("attribute")]
    public required Attribute Attribute { get; set; }

    /**
     * Kiểu giá trị
     */
    [ConfigColumn("value_type")]
    public required ValueType ValueType { get; set; }

    /**
     * Giá trị
     */
    [ConfigColumn("value")]
    public string? Value { get; set; }
}

public enum ComponentType
{
    EmployeeInfo, // Thong tin nhan vien
    Other, // Khac
    Salary, // Luong
    Revenue, // Doanh so
    InsuranceOrUnion, // Bao hiem - Cong doan 
    Attendance, // Cham cong
}

public enum Attribute
{
    Other, // Khac
    Income, // Thu nhap
    Deduction, // Khau tru
}

public enum ValueType
{
    Digit, // So
    Currency, // Tien te
    Text, // Chu
    Day, // Ngay
}