namespace MISA.Common.Enum;

public enum FilterType: int
{
    Contains = 0,
    NotContains = 1,
    StartWith = 2,
    EndWith = 3,
    Equals = 4,
    NotEquals = 5,
    GreaterThanOrEqual = 6,
    LessThanOrEqual = 7,
}