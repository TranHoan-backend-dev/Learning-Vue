using MISA.Common.Enum;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace MISA.BL.DTO.Request;

public abstract class FilterRequest
{
    public string? Keyword { get; set; }
    public IEnumerable<FilterColumn>? ColumnFilters { get; set; }

    public abstract class FilterColumn
    {
        public required string Column { get; set; }
        public required string Value { get; set; }
        public DataType DataType { get; set; }
        public FilterType FilterType { get; set; }
    }
}