namespace MISA.Common.Enum;

/// <summary>
/// Kiểu giá trị của thành phần lương
/// </summary>
public enum SalaryValueType
{
    /// <summary>
    /// Số
    /// </summary>
    Number = 0,

    /// <summary>
    /// Tiền tệ
    /// </summary>
    Currency = 1,

    /// <summary>
    /// Chữ (Văn bản)
    /// </summary>
    Text = 2,

    /// <summary>
    /// Ngày tháng
    /// </summary>
    Date = 3,

    /// <summary>
    /// Phần trăm
    /// </summary>
    Percentage = 4
}
