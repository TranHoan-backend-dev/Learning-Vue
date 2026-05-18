using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using MISA.Common.Attributes;
using MISA.Common.Base;
using MISA.Common.Exception;
using MISA.Common.Extension;
using MISA.Common.Procedures;
using MISA.DL.Base;
using MISA.DL.Context;
using MySqlConnector;

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
    /// <param name="parametersData">Tham số truyền vào query để lấy ra danh sách dữ liệu</param>
    /// <param name="commandData">Query dùng để lấy ra danh sách dữ liệu</param>
    /// <param name="parametersPageable">Tham số truyền vào query để lấy ra thông tin phân trang</param>
    /// <param name="commandPageable">Query dùng để truy vấn phân trang</param>
    /// <returns></returns>
    public async Task<(IEnumerable<T> Data, int Pageable)> GetPagedDataListAsync<T>(
        DynamicParameters parametersData, string commandData,
        DynamicParameters parametersPageable, string commandPageable
    )
    {
        log.LogInformation(
            "{Prefix} Execute query for {Entity}",
            Prefix,
            typeof(T).Name
        );

        await using var conn = context.GetConnection();

        var result = (await conn.QueryAsync<T>(
            commandData,
            param: parametersData
        )).ToList();

        log.LogDebug(
            "{Prefix} Returned {Count} rows",
            Prefix,
            result.Count
        );

        log.LogInformation($"{Prefix} Get total records count");
        var pageable = await conn.ExecuteScalarAsync<int>(
            commandPageable,
            param: parametersPageable
        );

        return (result, pageable);
    }

    /// <summary>
    /// Lấy thông tin chi tiết 1 bản ghi
    /// </summary>
    /// <param name="id">ID của bản ghi cần tìm</param>
    /// <param name="includeJoin">Cờ cho phép dùng procedure hay không</param>
    /// <param name="parameters">Tham số truyền vào command</param>
    /// <param name="command">Lệnh SQL dùng để truy vấn</param>
    /// <returns></returns>
    public async Task<T?> GetByIdAsync<T>(Guid? id, bool includeJoin, string? command, DynamicParameters? parameters)
    {
        log.LogInformation($"{Prefix} Get by id: {id}");
        await using var conn = context.GetConnection();
        if (!includeJoin && id is not null || parameters is null || command is null)
        {
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

        return await conn.QueryFirstOrDefaultAsync<T>(
            command,
            param: parameters
        );
    }

    /// <summary>
    /// Tạo lệnh INSERT để thêm nhiều bản ghi vào DB
    /// </summary>
    /// <param name="entity">Entity cần thêm</param>
    public async Task CreateAsync(T entity)
    {
        log.LogInformation($"{Prefix} Create");
        await using var conn = context.GetConnection();
        conn.Open();
        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            var type = typeof(T);
            var tableName = type.GetTableNameOnly();
            var columns = type.GetAllColumns();
            var columnsList = string.Join(",", columns);
            var (primaryKeyModel, primaryKeyTable) = type.GetPrimaryKey();

            if (await IsExist(type, tableName, entity, conn, transaction))
            {
                throw new ExistingException($"Bản ghi đã tồn tại trong hệ thống.");
            }

            // Audit dữ liệu
            var now = DateTime.Now;
            entity.CreatedAt = now;
            entity.CreatedBy = "Administrator";
            entity.ModifiedAt = now;
            entity.ModifiedBy = "Administrator";

            // Xử lý khóa chính. Nếu khóa chính không ở dạng UUID thì tạo mới
            if (!Guid.TryParse(entity.GetValue(primaryKeyModel)?.ToString(), out Guid keyValue) ||
                keyValue == Guid.Empty)
            {
                keyValue = Guid.NewGuid();
                // Gán lại giá trị cho model
                var propInfo = type.GetProperty(primaryKeyModel);
                if (propInfo != null && propInfo.CanWrite)
                {
                    propInfo.SetValue(entity, keyValue);
                }
            }

            // Build SQL
            var paramNames = columns.Select(c => $"@{c}"); // Tạo các param cho các cột
            var sql = $"INSERT INTO `{tableName}` ({columnsList}) VALUES ({string.Join(",", paramNames)})";

            var param = new DynamicParameters();
            foreach (var col in columns)
            {
                var value = primaryKeyTable == col ? keyValue : entity.GetValue(col);
                param.Add($"@{col}", value);
            }

            log.LogDebug($"{Prefix} Execute command: {sql}");

            var result = await conn.ExecuteAsync(sql, param, transaction);
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
        var param = new DynamicParameters();
        var variable = "@PrimaryKey";

        try
        {
            var type = typeof(T);
            var tableName = type.GetTableNameOnly();
            var (primaryKeyModel, primaryKeyTable) = type.GetPrimaryKey();
            var primaryKeyValue = entity.GetValue(primaryKeyModel);
            // truyền giá trị cho biến @PrimaryKey
            param.Add(variable, primaryKeyValue);

            // Kiem tra xem id nay co ton tai hay khong
            var query = $"SELECT COUNT(*) FROM `{tableName}` WHERE `{primaryKeyTable}` = @PrimaryKey";
            var res = await conn.QueryFirstOrDefaultAsync<int>(query, param, transaction);
            if (res == 0)
            {
                throw new NotFoundException("Ban ghi nay khong ton tai trong he thong");
            }

            // lay ra cac thuoc tinh can check trung
            if (await IsExist(type, tableName, entity, conn, transaction))
            {
                throw new ExistingException("Bản ghi đã tồn tại trong hệ thống.");
            }

            // Lấy ra cc thuộc tính không phải khóa chính
            var columns = type
                .GetAllColumns()
                .Where(c => c != primaryKeyTable && c != "created_at" && c != "created_by")
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

            foreach (var col in columns)
            {
                param.Add(
                    $"@{col}",
                    entity.GetValue(col)
                );
            }

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

    /// <summary>
    /// Cho phép xóa nhiều bản ghi
    /// </summary>
    /// <param name="ids">Danh sách các id cần xóa</param>
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

    /// <summary>
    /// Đếm toàn bộ số bản ghi của bảng 
    /// </summary>
    /// <param name="parameters">Param truyền vào query</param>
    /// <param name="command">Lệnh SQL</param>
    /// <returns></returns>
    public async Task<int> CountTotalElements(DynamicParameters parameters, string command)
    {
        log.LogInformation($"{Prefix} Get total records count");
        await using var conn = context.GetConnection();
        return await conn.ExecuteScalarAsync<int>(command, param: parameters);
    }

    private async Task<bool> IsExist(
        Type type, string tableName, T entity,
        MySqlConnection conn, MySqlTransaction transaction
    )
    {
        var duplicatedProperties = type
            .GetProperties()
            .Where(p => Attribute.IsDefined(p, typeof(CheckDuplicatedAttribute)))
            .ToList();

        // Kiểm tra bản ghi bị trùng
        if (duplicatedProperties.Any())
        {
            foreach (var property in duplicatedProperties)
            {
                var val = property.GetValue(entity)?.ToString();
                if (string.IsNullOrWhiteSpace(val)) continue;

                var duplicatedColumn = property.GetColumnName();
                var checkSql = $"SELECT COUNT(*) FROM `{tableName}` WHERE {duplicatedColumn} = @val";
                var existsCount = await conn.ExecuteScalarAsync<int>(checkSql, new { val }, transaction);

                return existsCount > 0;
            }
        }

        return false;
    }
}