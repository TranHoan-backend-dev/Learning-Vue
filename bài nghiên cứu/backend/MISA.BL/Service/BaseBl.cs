using System.Text;
using Dapper;
using Microsoft.Extensions.Logging;
using MISA.BL.Base;
using MISA.BL.DTO.Request;
using MISA.Common.Base;
using MISA.Common.Enum;
using MISA.Common.Extension;
using MISA.Common.Model.Pageable;
using MISA.DL.Base;

namespace MISA.BL.Service;

public class BaseBl<T>(
    IBaseDl<T> baseDl,
    ILogger<T> log
) : IBaseBl<T> where T : BaseModel
{
    private readonly string _logPrefix = "[BaseBl]";
    protected virtual string GetJoinColumns() => "";
    protected virtual string GetJoinSql() => "";

    /// <summary>
    /// Lấy ra danh sách các đối tượng T kèm phân trang, filter và tìm kiếm
    /// theo keyword
    /// </summary>
    /// <param name="pageable">Thông tin phân trang</param>
    /// <param name="request">Dữ liệu bộ lọc và từ khóa tìm kiếm</param>
    /// <returns></returns>
    public virtual async Task<PagingData<T>> GetAllAsync(Pageable pageable, FilterRequest request)
    {
        log.LogInformation($"{_logPrefix} Get data with pageable: {pageable}, keyword: {request.Keyword}");
        var parameter = new DynamicParameters();
        var (sql, paginationSql) = BuildQueryStringWithCondition(
            request, pageable.PageIndex,
            (long)pageable.PageSize, ref parameter
        );

        var data = await baseDl.GetPagedDataListAsync<T>(
            parameter, sql,
            parameter, paginationSql
        );

        return new PagingData<T>
        {
            Data = data.Data,
            Pageable = new Pageable
            {
                PageIndex = pageable.PageIndex,
                PageSize = pageable.PageSize,
                TotalElements = data.Pageable
            }
        };
    }

    /// <summary>
    /// Lấy thông tin chi tiết của 1 obj
    /// </summary>
    /// <param name="id">ID của bản ghi cần tìm</param>
    /// <returns>Đối tượng T hoặc null</returns>
    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        log.LogInformation("Get by ID {Id} for entity: {Entity}", id, typeof(T).Name);

        var joinSql = GetJoinSql();
        var extraColumns = GetJoinColumns();

        // Nếu không có Join và không có cột mở rộng, sử dụng Stored Procedure mặc định
        if (string.IsNullOrEmpty(joinSql) && string.IsNullOrEmpty(extraColumns))
        {
            return await baseDl.GetByIdAsync<T>(id, false, null, null);
        }

        var (_, primaryKeyTable) = typeof(T).GetPrimaryKey();
        var sql = $"SELECT {GetSelectColumns()} FROM {GetBaseFromSql()} WHERE t1.{primaryKeyTable} = @id";

        var parameters = new DynamicParameters();
        parameters.Add("@id", id);

        return await baseDl.GetByIdAsync<T>(null, true, sql, parameters);
    }

    /// <summary>
    /// Tạo mới đối tượng
    /// </summary>
    /// <param name="model">Dữ liệu bản ghi cần thêm</param>
    public async Task AddAsync(T model)
    {
        log.LogInformation("Add new record for entity: {Entity}", typeof(T).Name);
        await baseDl.CreateAsync(model);
    }

    /// <summary>
    /// Cập nhật bản ghi có sẵn
    /// </summary>
    /// <param name="model">Dữ liệu cần cập nhật</param>
    /// <param name="id">Id của bản ghi cần cập nhật</param>
    /// <returns></returns>
    public async Task<int> UpdateAsync(T model, Guid id)
    {
        log.LogInformation("Update record {Id} for entity: {Entity}", id, typeof(T).Name);

        // Gán ID cho model trước khi update
        var (primaryKeyModel, _) = typeof(T).GetPrimaryKey();
        if (!string.IsNullOrEmpty(primaryKeyModel))
        {
            var propInfo = typeof(T).GetProperty(primaryKeyModel);
            if (propInfo != null && propInfo.CanWrite)
            {
                propInfo.SetValue(model, id);
            }
        }

        await baseDl.UpdateAsync(model);
        return 1;
    }

    /// <summary>
    /// Xóa nhiều bản ghi
    /// </summary>
    /// <param name="ids">Danh sách các id cần xóa</param>
    public async Task DeleteAsync(List<string> ids)
    {
        log.LogInformation("Delete records {Ids} for entity: {Entity}", string.Join(", ", ids), typeof(T).Name);
        await baseDl.DeleteAsync(ids);
    }

    /// <summary>
    /// Xây dựng query truy vấn dữ liệu đi kèm với các điều kiện lọc/tìm kiếm nếu có
    /// </summary>
    /// <param name="request">Yêu cầu lọc/tìm kiếm</param>
    /// <param name="pageIndex">Trang hiện tại</param>
    /// <param name="pageSize">Cỡ bảng</param>
    /// <param name="parameters">Các tham số truyền vào query</param>
    /// <returns></returns>
    private (string, string) BuildQueryStringWithCondition(
        FilterRequest request, int pageIndex,
        long pageSize, ref DynamicParameters parameters
    )
    {
        var type = typeof(T);
        var baseFromSql = GetBaseFromSql();
        var query = new StringBuilder($"SELECT {GetSelectColumns()} FROM {baseFromSql}");
        var conditions = new List<string>();
        var subQuery = new StringBuilder();

        // 1. Search by keyword
        var keyword = request.Keyword;
        var searchableColumns = type.GetSearchableColumns();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var keywordConditions = searchableColumns.Select(c => $"t1.{c} LIKE @keyword");
            var operand = string.Join($"\n        {Operand.Or} ", keywordConditions);
            conditions.Add("(\n        " + operand + "\n    )");
            parameters.Add("@keyword", $"%{keyword}%");
        }

        // xu ly bo loc
        var columnFilters = request.ColumnFilters?.ToList();
        if (columnFilters is not null && columnFilters.Any())
        {
            foreach (var item in columnFilters)
            {
                var columnName = type.GetPropertyInModel(item.Column);
                if (string.IsNullOrWhiteSpace(columnName)) continue;

                var dataType = item.DataType;
                var condition = new StringBuilder();

                if (dataType == DataType.String)
                {
                    if (item.FilterType == FilterType.Equals || item.FilterType == FilterType.NotEquals)
                    {
                        var operand = item.FilterType == FilterType.Equals ? Operand.Equal : Operand.NotEqual;
                        condition.Append($"t1.{columnName} {operand} @filter_{columnName}");
                        parameters.Add($"@filter_{columnName}", item.Value);
                    }
                    else
                    {
                        var pattern = item.FilterType switch
                        {
                            FilterType.Contains or FilterType.NotContains
                                => $"%{item.Value}%",
                            FilterType.StartWith => $"{item.Value}%",
                            FilterType.EndWith => $"%{item.Value}",
                            _ => null
                        };

                        if (pattern is not null)
                        {
                            var operand = item.FilterType == FilterType.NotContains
                                ? Operand.NotLike
                                : Operand.Like;
                            condition.Append($"t1.{columnName} {operand} @filter_{columnName}");
                            parameters.Add($"@filter_{columnName}", pattern);
                        }
                    }
                }
                else if (dataType == DataType.DateTime)
                {
                    var operand = item.FilterType switch
                    {
                        FilterType.Equals => Operand.Equal,
                        FilterType.NotEquals => Operand.NotEqual,
                        FilterType.GreaterThanOrEqual => Operand.GreaterThanOrEqual,
                        FilterType.LessThanOrEqual => Operand.LessThanOrEqual,
                        _ => null
                    };

                    if (operand is not null)
                    {
                        condition.Append($"t1.{columnName} {operand} @filter_{columnName}");
                        parameters.Add($"@filter_{columnName}", item.Value);
                    }
                }

                if (condition.Length > 0)
                {
                    conditions.Add($"({condition})");
                }
            }
        }

        // Combine conditions
        if (conditions.Count > 0)
        {
            subQuery.Append(" WHERE ");
            subQuery.Append(string.Join(Operand.And, conditions));
        }

        var pagination = new StringBuilder($"SELECT COUNT(*) FROM {baseFromSql}");
        pagination.Append(subQuery);

        // Them limit offset
        subQuery.AppendLine("  ORDER BY t1.created_at DESC LIMIT @limit OFFSET @offset");
        parameters.Add("@limit", (long)pageSize);
        parameters.Add("@offset", (long)pageIndex * (long)pageSize);

        query.Append(subQuery);

        log.LogInformation($"{_logPrefix} Final query: {query}");
        return (query.ToString(), pagination.ToString());
    }

    /// <summary>
    /// Build chuỗi SELECT các cột (bao gồm cả các cột từ Join nếu có)
    /// </summary>
    private string GetSelectColumns()
    {
        var type = typeof(T);
        var columns = type.GetAllColumns();
        var selectColumns = string.Join(", ", columns.Select(c => $"t1.{c}"));

        var extraColumns = GetJoinColumns();
        return !string.IsNullOrEmpty(extraColumns)
            ? $"{selectColumns}, {extraColumns}"
            : selectColumns;
    }

    /// <summary>
    /// Build chuỗi FROM kèm Join SQL
    /// </summary>
    private string GetBaseFromSql()
    {
        var tableName = typeof(T).GetTableNameOnly();
        var joinSql = GetJoinSql();

        return string.IsNullOrEmpty(joinSql)
            ? $"`{tableName}` t1"
            : $"`{tableName}` t1 {joinSql}";
    }
}