using MISA.Common.Enum;

namespace MISA.BL.DTO.Request;

public class FilterRequest
{
    public string? Keyword { get; set; }
    public IEnumerable<FilterColumn>? ColumnFilters { get; set; }

    public class FilterColumn
    {
        public required string Column { get; set; }
        public required string Value { get; set; }
        public DataType DataType { get; set; }
        public FilterType FilterType { get; set; }
    }
}