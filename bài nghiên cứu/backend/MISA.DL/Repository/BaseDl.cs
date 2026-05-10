using System.Data;
using System.Reflection;
using System.Text;
using Dapper;
using Microsoft.Extensions.Logging;
using MISA.Common.Attributes;
using MISA.Common.Base;
using MISA.Common.Enum;
using MISA.Common.Extension;
using MISA.Common.Procedures;
using MISA.DL.Base;
using MISA.DL.Context;

namespace MISA.DL.Repository;

public class BaseDl<T>(
    DbContext context,
    ILogger<BaseDl<T>> log
) : IBaseDl<T> where T : BaseModel
{
    private const string Prefix = "[BaseDL]";

    /// <summary>
    /// Lấy ra danh sách phân trang các bản ghi có kiểu dữ liệu generic 
    /// </summary>
    /// <param name="parameters">Tham số truyền vào query</param>
    /// <param name="command">Query</param>
    /// <returns></returns>
    public async Task<IEnumerable<T>> GetPagedDataListAsync(DynamicParameters parameters, string command)
    {
        log.LogInformation(
            "{Prefix} Execute query for {Entity}",
            Prefix,
            typeof(T).Name
        );

        await using var conn = context.GetConnection();

        var result = (await conn.QueryAsync<T>(
            command,
            param: parameters,
            commandType: CommandType.Text
        )).ToList();

        log.LogDebug(
            "{Prefix} Returned {Count} rows",
            Prefix,
            result.Count
        );

        return result;
    }

    /// <summary>
    /// Lấy thông tin chi tiết 1 bản ghi
    /// </summary>
    /// <param name="id">ID của bản ghi cần tìm</param>
    /// <returns></returns>
    public async Task<T?> GetByIdAsync(Guid id)
    {
        log.LogInformation($"{Prefix} Get by id: {id}");
        await using var conn = context.GetConnection();
        var storedProcedure = string.Format(ProcedureNames.GetDetails, typeof(T).GetTableNameOnly());
        var res = await conn.QueryFirstOrDefaultAsync<T>(
            storedProcedure,
            param: new { Id = id },
            commandType: CommandType.StoredProcedure
        );
        log.LogDebug(
            "{Prefix} Found entity: {Found}",
            Prefix,
            res is not null
        );
        return res;
    }

    /// <summary>
    /// Tạo lệnh INSERT để thêm nhiều bản ghi vào DB
    /// </summary>
    /// <param name="entities">Danh sách các entity cần thêm</param>
    public async Task CreateAsync(IEnumerable<T> entities)
    {
        log.LogInformation($"{Prefix} Create");

        await using var conn = context.GetConnection();

        conn.Open();

        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            var list = entities.ToList();

            if (!list.Any())
            {
                return;
            }

            var type = typeof(T);

            var tableName = type.GetTableNameOnly();

            var columns = type.GetAllColumns();

            var columnsList = string.Join(",", columns);

            var (primaryKeyModel, primaryKeyTable) = type.GetPrimaryKey();

            var sql = new StringBuilder();

            sql.Append($"INSERT INTO `{tableName}` ({columnsList}) VALUES ");

            var param = new DynamicParameters();

            var values = new List<string>();


            // =====================================================
            // FIX N+1 QUERY
            // =====================================================

            var duplicatedProperties = type
                .GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(CheckDuplicatedAttribute)))
                .ToList();

            HashSet<string> duplicatedValues = [];

            var hasDuplicateCheck = duplicatedProperties.Any();
            PropertyInfo? duplicatedProperty = null;

            if (hasDuplicateCheck)
            {
                duplicatedProperty = duplicatedProperties.First();
            }

            if (hasDuplicateCheck)
            {
                // Hiện tại xử lý property duplicate đầu tiên
                // Có thể mở rộng multi-column unique sau

                var duplicatedColumn = duplicatedProperty!.GetColumnName();

                var existingQuery =
                    $"SELECT {duplicatedColumn} FROM `{tableName}`";

                var existingValues = await conn.QueryAsync<string>(
                    existingQuery,
                    transaction: transaction
                );

                duplicatedValues = existingValues.ToHashSet();
            }


            // =====================================================
            // BUILD BULK INSERT
            // =====================================================

            var now = DateTime.Now;

            int count = 0;

            foreach (var entity in list)
            {
                // =====================================================
                // CHECK DUPLICATE IN MEMORY
                // =====================================================

                if (hasDuplicateCheck)
                {
                    var duplicatedValue =
                        duplicatedProperty!.GetValue(entity)?.ToString();

                    if (!string.IsNullOrWhiteSpace(duplicatedValue))
                    {
                        if (!duplicatedValues.Add(duplicatedValue))
                        {
                            continue;
                        }
                    }
                }


                // =====================================================
                // AUDIT FIELD
                // =====================================================

                entity.CreatedAt = now;
                entity.CreatedBy = "Administrator";

                entity.ModifiedAt = now;
                entity.ModifiedBy = "Administrator";


                // =====================================================
                // PRIMARY KEY
                // =====================================================

                if (!Guid.TryParse(
                        entity.GetValue(primaryKeyModel)?.ToString(),
                        out Guid keyValue
                    ) || keyValue == Guid.Empty)
                {
                    keyValue = Guid.NewGuid();
                }


                // =====================================================
                // BUILD PARAMETERS
                // =====================================================

                var parameterNames = new List<string>();

                foreach (var col in columns)
                {
                    var paramName = $"@{col}_{count}";

                    parameterNames.Add(paramName);

                    var value = primaryKeyTable == col
                        ? keyValue
                        : entity.GetValue(col);

                    param.Add(paramName, value);
                }

                values.Add($"({string.Join(",", parameterNames)})");

                count++;
            }


            // =====================================================
            // NOTHING TO INSERT
            // =====================================================

            if (!values.Any())
            {
                log.LogDebug($"{Prefix} No valid entity to insert");
                return;
            }


            // =====================================================
            // EXECUTE
            // =====================================================

            sql.Append(string.Join(",", values));

            log.LogDebug($"{Prefix} Build command: {sql}");

            var result = await conn.ExecuteAsync(
                sql.ToString(),
                param,
                transaction
            );

            await transaction.CommitAsync();

            log.LogDebug($"{Prefix} Output: {result}");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            log.LogError($"{Prefix} Exception: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Cập nhật bản ghi 
    /// </summary>
    /// <param name="entity"></param>
    public async Task UpdateAsync(T entity)
    {
        log.LogInformation(
            "{Prefix} Update entity {Entity}",
            Prefix,
            typeof(T).Name
        );

        await using var conn = context.GetConnection();
        await conn.OpenAsync();
        await using var transaction = await conn.BeginTransactionAsync();
        var variable = "@PrimaryKey";

        try
        {
            var type = typeof(T);
            var tableName = type.GetTableNameOnly();
            var (primaryKeyModel, primaryKeyTable) = type.GetPrimaryKey();

            var columns = type
                .GetAllColumns()
                .Where(c => c != primaryKeyTable)
                .ToList();

            entity.ModifiedAt = DateTime.Now;
            entity.ModifiedBy = "Administrator";

            var setStatements = columns
                .Select(c => $"{c} = @{c}");
            var sql = $"""
                       UPDATE `{tableName}`
                       SET {string.Join(",", setStatements)}
                       WHERE {primaryKeyTable} = {variable}
                       """;
            var param = new DynamicParameters();

            foreach (var col in columns)
            {
                param.Add(
                    $"@{col}",
                    entity.GetValue(col)
                );
            }

            var primaryKeyValue = entity.GetValue(primaryKeyModel);
            // truyền giá trị cho biến @PrimaryKey
            param.Add(variable, primaryKeyValue);

            log.LogDebug(
                "{Prefix} Build command: {Sql}",
                Prefix,
                sql
            );

            var result = await conn.ExecuteAsync(
                sql,
                param,
                transaction
            );

            await transaction.CommitAsync();

            log.LogDebug(
                "{Prefix} Updated rows: {Result}",
                Prefix,
                result
            );
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            log.LogError(
                ex,
                "{Prefix} Exception while updating",
                Prefix
            );

            throw;
        }
    }

    public async Task DeleteAsync(List<string> ids)
    {
        log.LogInformation(
            "{Prefix} Delete entities {Entity}",
            Prefix,
            typeof(T).Name
        );

        if (!ids.Any())
        {
            return;
        }

        await using var conn = context.GetConnection();

        await conn.OpenAsync();

        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            var type = typeof(T);

            var tableName = type.GetTableNameOnly();

            var (_, primaryKeyTable) = type.GetPrimaryKey();

            var sql = $"""
                       DELETE FROM `{tableName}`
                       WHERE {primaryKeyTable} IN @Ids
                       """;

            var result = await conn.ExecuteAsync(
                sql,
                new
                {
                    Ids = ids
                },
                transaction
            );

            await transaction.CommitAsync();

            log.LogDebug(
                "{Prefix} Deleted rows: {Result}",
                Prefix,
                result
            );
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            log.LogError(
                ex,
                "{Prefix} Exception while deleting",
                Prefix
            );

            throw;
        }
    }

    // public async Task<object> ExecuteCommandText(string commandText, DynamicParameters parameters)
    // {
    //     throw new NotImplementedException();
    // }
}