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
    
    public async Task<PagingData<T>> GetAllAsync(Pageable pageable, FilterRequest request)
    {
        return await GetAllAsyncInternal<T>(pageable, request);
    }

    protected async Task<PagingData<TReturn>> GetAllAsyncInternal<TReturn>(Pageable pageable, FilterRequest request)
    {
        log.LogInformation($"{_logPrefix} Get data with pageable: {pageable}, keyword: {request.Keyword}");
        var parameter = new DynamicParameters();
        var (sql, paginationSql) = BuildQueryStringWithCondition(request, pageable.PageIndex, pageable.PageSize, ref parameter);
        
        var data = await baseDl.GetPagedDataListAsync<TReturn>(parameter, sql);
        var totalElements = await baseDl.CountTotalElements(parameter, paginationSql);

        return new PagingData<TReturn>
        {
            Data = data,
            Pageable = new Pageable
            {
                PageIndex = pageable.PageIndex,
                PageSize = pageable.PageSize,
                TotalElements = totalElements
            }
        };
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await GetByIdAsyncInternal<T>(id);
    }

    protected async Task<TReturn?> GetByIdAsyncInternal<TReturn>(Guid id)
    {
        log.LogInformation("Get by ID {Id} for entity: {Entity}", id, typeof(T).Name);
        
        var joinSql = GetJoinSql();
        if (string.IsNullOrEmpty(joinSql))
        {
            return await baseDl.GetByIdAsync<TReturn>(id);
        }

        // Nếu có Join, ta tự build query SELECT thay vì dùng SP mặc định
        var type = typeof(T);
        var tableName = type.GetTableNameOnly();
        var columns = type.GetAllColumns();
        var (_, primaryKeyTable) = type.GetPrimaryKey();

        var selectColumns = string.Join(", ", columns.Select(c => $"t1.{c}"));
        var extraColumns = GetJoinColumns();
        if (!string.IsNullOrEmpty(extraColumns))
        {
            selectColumns += ", " + extraColumns;
        }

        var sql = $"SELECT {selectColumns} FROM `{tableName}` t1 {joinSql} WHERE t1.{primaryKeyTable} = @id";
        var parameters = new DynamicParameters();
        parameters.Add("@id", id);

        var result = await baseDl.GetPagedDataListAsync<TReturn>(parameters, sql);
        return result.FirstOrDefault();
    }

    public async Task AddAsync(T model)
    {
        log.LogInformation("Add new record for entity: {Entity}", typeof(T).Name);
        await baseDl.CreateAsync(new List<T> { model });
    }

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

    public async Task DeleteAsync(List<string> ids)
    {
        log.LogInformation("Delete records {Ids} for entity: {Entity}", string.Join(", ", ids), typeof(T).Name);
        await baseDl.DeleteAsync(ids);
    }
    
    private (string, string) BuildQueryStringWithCondition(FilterRequest request, int pageIndex, decimal pageSize,
        ref DynamicParameters parameters)
    {
        var type = typeof(T);
        var tableName = type.GetTableNameOnly();
        var columns = type.GetAllColumns();

        // 1. Build select columns with alias t1
        var selectColumns = string.Join(", ", columns.Select(c => $"t1.{c}"));
        var extraColumns = GetJoinColumns();
        if (!string.IsNullOrEmpty(extraColumns))
        {
            selectColumns += ", " + extraColumns;
        }

        var query = new StringBuilder($"SELECT {selectColumns} FROM `{tableName}` t1");
        
        // 2. Add Join SQL
        var joinSql = GetJoinSql();
        if (!string.IsNullOrEmpty(joinSql))
        {
            query.Append(" " + joinSql);
        }

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
        var pagination = new StringBuilder($"SELECT COUNT(*) FROM `{tableName}` t1");
        if (!string.IsNullOrEmpty(joinSql))
        {
            pagination.Append(" " + joinSql);
        }
        pagination.Append(subQuery);

        // Them limit offset
        subQuery.AppendLine("  ORDER BY t1.created_at DESC LIMIT @limit OFFSET @offset");
        parameters.Add("@limit", pageSize);
        parameters.Add("@offset", pageIndex * pageSize);

        query.Append(subQuery);

        log.LogInformation($"{_logPrefix} Final query: {query}");
        return (query.ToString(), pagination.ToString());
    }
}