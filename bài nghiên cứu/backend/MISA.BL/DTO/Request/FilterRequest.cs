using MISA.Common.Enum;

namespace MISA.BL.DTO.Request;

public class FilterRequest
{
    /// <summary>
    /// Từ khóa 
    /// </summary>
    public string? Keyword { get; set; }
    public IEnumerable<FilterColumn>? ColumnFilters { get; set; }
    
    /// <summary>
    /// Bộ lọc theo cột 
    /// </summary>

    public class FilterColumn
    {
        /// <summary>
        /// Tên cột
        /// </summary>
        public required string Column { get; set; }
        
        /// <summary>
        /// Giá trị filter
        /// </summary>
        public required string Value { get; set; }
        
        /// <summary>
        /// Kiểu dữ liệu của filter (String, Datetime,...)
        /// </summary>
        public DataType DataType { get; set; }
        
        /// <summary>
        /// Loại filter (Contains hay Equals,...)
        /// </summary>
        public FilterType FilterType { get; set; }
    }
}