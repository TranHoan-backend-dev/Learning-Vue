using MISA.Common.Resources;

namespace MISA.Common.Attributes;

/***
 * Cau hinh ten bang cho class
 */
[AttributeUsage(AttributeTargets.Class)]
public class ConfigTableAttribute(string tableName) : Attribute
{
    public string TableName { get; set; } = tableName;
}

/**
 * Cau hinh ten cot cho thuoc tinh
 */
[AttributeUsage(AttributeTargets.Property)]
public class ConfigColumnAttribute(string columnName) : Attribute
{
    public string ColumnName { get; set; } = columnName;
}

/**
 * Cau hinh cac thuoc tinh co the tim kiem duoc
 */
[AttributeUsage(AttributeTargets.Property)]
public class ConfigSearchableAttribute : Attribute;

/// <summary>
/// Attribute kiểm tra bản ghi bị trùng lặp
/// </summary>
/// <param name="errorMessageKey"></param>
[AttributeUsage(AttributeTargets.Property)]
public class CheckDuplicatedAttribute(string errorMessageKey) : Attribute
{
    public string ErrorMessage => ResourcesVN.ResourceManager.GetString(errorMessageKey) ?? errorMessageKey;
}