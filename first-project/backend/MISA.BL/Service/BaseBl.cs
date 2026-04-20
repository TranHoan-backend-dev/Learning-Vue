using System.Text;
using Dapper;
using Microsoft.Extensions.Logging;
using MISA.BL.Base;
using MISA.BL.DTO.Request;
using MISA.Common.Base;
using MISA.Common.Enum;
using MISA.Common.Extension;
using MISA.Common.Model;
using MISA.DL.Base;

namespace MISA.BL.Service;

public sealed class BaseBl<T>(
    IBaseDl<T> baseDl,
    ILogger<BaseBl<T>> log
) : IBaseBl<T> where T : BaseModel
{
    private readonly string _logPrefix = "[BaseBl]";

    public async Task<IEnumerable<T>?> GetAllAsync(Pageable pageable, FilterRequest request)
    {
        log.LogInformation($"{_logPrefix} Get data with pageable: {pageable}, keyword: {request.Keyword}");
        var parameter = new DynamicParameters();
        var sql = BuildQueryStringWithCondition(request, pageable.PageIndex, pageable.PageSize, ref parameter);
        return await baseDl.GetPagedAsync(parameter, sql);
    }

    public Task<T?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(T model, Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<T>> SaveDataAsync(List<T> models)
    {
        log.LogInformation("{prefix} Save data", _logPrefix);
        if (models.Count > 0)
        {
            await BeforeSave(models);

            var rowInsert = models.Where(m =>
                    m.State == AppEnum.ModelState.Insert || m.State == AppEnum.ModelState.Update)
                .ToList();

            var param = new DynamicParameters();
            var command = BuildQueryStringInsertAndUpdate(rowInsert, ref param);
            log.LogDebug("{prefix} Build command: {command}", _logPrefix, command);

            var result = await baseDl.ExecuteCommandText(command, param);
            log.LogDebug("{prefix} Output: {res}", _logPrefix, result);
            return models;
        }

        return [];
    }

    public Task<int> CountTotalElements()
    {
        return baseDl.CountTotalElements();
    }

    #region Build query

    private String BuildQueryStringInsertAndUpdate(List<T> models, ref DynamicParameters parameters)
    {
        var sql = new StringBuilder();
        var type = models[0].GetType();

        var tableName = type.GetTableNameOnly();
        var primaryKey = type.GetPrimaryKey().keyTable;
        var columns = type.GetAllColumns();

        var columnsList = string.Join(",", columns);
        sql.Append($"INSERT INTO `{tableName}` ({columnsList}) VALUES");

        var isFirst = true;
        int count = default;
        foreach (var model in models)
        {
            // neu parse fail hoac keyValue bi empty thi tao key moi
            if (!Guid.TryParse(model.GetValue(primaryKey) + "", out Guid keyValue) || keyValue == Guid.Empty)
            {
                keyValue = Guid.NewGuid();
            }

            var parameterNames = columns.Select(col => $"@{col}_{count}");
            var s = string.Join(",", parameterNames);

            log.LogDebug("{prefix} Append sql command: {s}", _logPrefix, s);
            if (isFirst)
            {
                sql.Append($"({s})");
                isFirst = false;
            }
            else
            {
                sql.Append($", ({s})");
            }

            foreach (var col in columns)
            {
                var value = primaryKey == col ? keyValue : model.GetValue(col);
                parameters.Add($"@{col}_{count}", value);
                // Console.WriteLine($"Col: {col}");
                // Console.WriteLine($"Value: {value}");
                if (primaryKey == col)
                {
                    Console.WriteLine(value);
                }
            }

            count++;
        }

        log.LogInformation("{prefix} Append update command", _logPrefix);
        var columnUpdate = columns
            .FindAll(c => c != primaryKey)
            .Select(c => $"`{c}` = VALUES(`{c}`)");
        sql.AppendLine($"ON DUPLICATE KEY UPDATE {string.Join(", ", columnUpdate)}");
        return sql.ToString();
    }

    private string BuildQueryStringWithCondition(FilterRequest request, int pageIndex, decimal pageSize,
        ref DynamicParameters parameters)
    {
        var type = typeof(T);
        var tableName = type.GetTableNameOnly();
        var columns = type.GetAllColumns();

        var query = new StringBuilder($"SELECT {string.Join(", ", columns)} FROM `{tableName}`");
        var conditions = new List<string>();

        // 1. Search by keyword
        var keyword = request.Keyword;
        var searchableColumns = type.GetSearchableColumns();
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var keywordConditions = searchableColumns.Select(c => $"{c} LIKE @keyword");
            var operand = string.Join($"\n        {AppEnum.Operand.Or} ", keywordConditions);
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

                if (dataType == AppEnum.DataType.String)
                {
                    var pattern = item.FilterType switch
                    {
                        AppEnum.FilterType.Contains or AppEnum.FilterType.NotContains
                            => $"%{item.Value}%",
                        AppEnum.FilterType.StartWith => $"{item.Value}%",
                        AppEnum.FilterType.EndWith => $"%{item.Value}",
                        _ => null
                    };

                    if (pattern is not null)
                    {
                        var operand = item.FilterType == AppEnum.FilterType.NotContains
                            ? AppEnum.Operand.NotLike
                            : AppEnum.Operand.Like;
                        condition.Append($"{columnName} {operand} @filter_{columnName}");
                        parameters.Add($"@filter_{columnName}", pattern);
                    }
                }
                else if (dataType == AppEnum.DataType.DateTime)
                {
                    var operand = item.FilterType switch
                    {
                        AppEnum.FilterType.Equals => AppEnum.Operand.Equal,
                        AppEnum.FilterType.NotEquals => AppEnum.Operand.NotEqual,
                        AppEnum.FilterType.GreaterThanOrEqual => AppEnum.Operand.GreaterThanOrEqual,
                        AppEnum.FilterType.LessThanOrEqual => AppEnum.Operand.LessThanOrEqual,
                        _ => null
                    };

                    if (operand is not null)
                    {
                        condition.Append($"{columnName} {operand} @filter_{columnName}");
                        parameters.Add($"@filter_{columnName}", item.Value);
                    }
                }

                if (condition.Length > 0)
                {
                    conditions.Add($"({condition})");
                }
            }
        }

        // 3. Combine conditions
        if (conditions.Count > 0)
        {
            query.Append(" WHERE ");
            query.Append(string.Join(AppEnum.Operand.And, conditions));
        }

        // Them limit offset
        query.AppendLine(" LIMIT @limit OFFSET @offset");
        parameters.Add("@limit", pageSize);
        parameters.Add("@offset", pageIndex * pageSize);

        log.LogInformation($"{_logPrefix} Final query: {query}");
        return query.ToString();
    }

    #endregion

    private Task BeforeSave(List<T> models)
    {
        foreach (var item in models)
        {
            item.CreatedBy = "Admin";
            item.CreatedAt = DateTime.Now;
            item.ModifiedBy = "Admin";
            item.ModifiedAt = DateTime.Now;
        }

        return Task.CompletedTask;
    }
}