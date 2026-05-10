using System.Text;
using Dapper;
using Microsoft.Extensions.Logging;
using MISA.BL.Base;
using MISA.BL.DTO.Request;
using MISA.Common.Base;
using MISA.Common.Extension;
using MISA.Common.Model.Pageable;
using MISA.DL.Repository;

namespace MISA.BL.Service;

public class BaseBl<T>(
    BaseDl<T> baseDl,
    ILogger<T> logger
) : IBaseBl<T> where T : BaseModel
{
    public async Task<PagingData<T>> GetAllAsync(Pageable pageable, FilterRequest request)
    {
        logger.LogInformation("Getting all records for entity: {Entity}", typeof(T).Name);
        
        var tableName = typeof(T).GetTableNameOnly();
        var command = new StringBuilder($"SELECT * FROM `{tableName}` WHERE 1=1");
        var parameters = new DynamicParameters();

        // Xử lý filter từ FilterRequest DTO
        if (request != null)
        {
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var searchableColumns = typeof(T).GetSearchableColumns();
                if (searchableColumns.Count > 0)
                {
                    command.Append(" AND (");
                    var keywordConditions = searchableColumns.Select(col => $"{col} LIKE @Keyword");
                    command.Append(string.Join(" OR ", keywordConditions));
                    command.Append(")");
                    parameters.Add("@Keyword", $"%{request.Keyword}%");
                }
            }

            if (request.ColumnFilters != null && request.ColumnFilters.Any())
            {
                var index = 0;
                foreach (var filter in request.ColumnFilters)
                {
                    var paramName = $"@FilterVal{index}";
                    command.Append($" AND {filter.Column} = {paramName}");
                    parameters.Add(paramName, filter.Value);
                    index++;
                }
            }
        }

        // Xử lý sắp xếp từ Pageable DTO
        if (!string.IsNullOrWhiteSpace(pageable.Sort))
        {
            command.Append($" ORDER BY {pageable.Sort}");
        }
        else
        {
            var (_, primaryKeyTable) = typeof(T).GetPrimaryKey();
            if (!string.IsNullOrEmpty(primaryKeyTable))
            {
                command.Append($" ORDER BY {primaryKeyTable} DESC");
            }
        }

        // Xử lý phân trang
        if (pageable.PageSize > 0)
        {
            command.Append(" LIMIT @Limit OFFSET @Offset");
            parameters.Add("@Limit", pageable.PageSize);
            parameters.Add("@Offset", pageable.PageIndex * pageable.PageSize);
        }

        var data = await baseDl.GetPagedDataListAsync(parameters, command.ToString());

        return new PagingData<T>
        {
            Data = data,
            Pageable = new Pageable
            {
                PageIndex = pageable.PageIndex,
                PageSize = pageable.PageSize,
                Sort = pageable.Sort,
                TotalElements = data.Count() // Lưu ý: Số lượng này chỉ là số lượng của trang hiện tại. Để lấy tổng số phần tử cần 1 câu query count riêng biệt.
            }
        };
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        logger.LogInformation("Get by ID {Id} for entity: {Entity}", id, typeof(T).Name);
        return await baseDl.GetByIdAsync(id);
    }

    public async Task AddAsync(T model)
    {
        logger.LogInformation("Add new record for entity: {Entity}", typeof(T).Name);
        await baseDl.CreateAsync(new List<T> { model });
    }

    public async Task<int> UpdateAsync(T model, Guid id)
    {
        logger.LogInformation("Update record {Id} for entity: {Entity}", id, typeof(T).Name);
        
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
        logger.LogInformation("Delete records {Ids} for entity: {Entity}", string.Join(", ", ids), typeof(T).Name);
        await baseDl.DeleteAsync(ids);
    }
}