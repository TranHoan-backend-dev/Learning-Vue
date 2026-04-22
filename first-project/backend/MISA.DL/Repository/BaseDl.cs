using System.Data;
using System.Reflection;
using System.Text;
using Dapper;
using Microsoft.Extensions.Logging;
using MISA.Common.Attributes;
using MISA.Common.Base;
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
    private const string LogPrefix = "[BaseDl]";

    public async Task<IEnumerable<T>?> GetAllAsync(BaseModel model)
    {
        log.LogInformation("{prefix} Get all models", LogPrefix);
        var storedProcedure = string.Format(ProcedureNames.GetAll, model.GetType().Name);
        using var conn = context.GetConnection();
        var res = await conn.QueryAsync<T>(
            storedProcedure,
            commandType: CommandType.StoredProcedure
        );
        log.LogDebug("Response: {res}", res);
        return res;
    }

    public async Task<IEnumerable<T>?> GetPagedAsync(DynamicParameters parameters, string command)
    {
        log.LogInformation("{prefix} Get paginated models list", LogPrefix);
        using var conn = context.GetConnection();
        var res = await conn.QueryAsync<T>(command, param: parameters);
        log.LogDebug("Response: {res}", res);
        return res;
    }


    public async Task<T?> GetByIdAsync(Guid id)
    {
        log.LogInformation("{prefix} Get model details by id: {id}", LogPrefix, id.ToString());
        var storedProcedure = string.Format(ProcedureNames.GetDetails, typeof(T).Name);
        using var conn = context.GetConnection();

        var param = new DynamicParameters();
        param.Add("p_Id", id);

        var res = await conn.QueryFirstOrDefaultAsync<T>(
            storedProcedure,
            param,
            commandType: CommandType.StoredProcedure
        );
        log.LogDebug("{prefix} Response: {res}", LogPrefix, res);
        return res;
    }

    public async Task CreateAsync(T entity)
    {
        log.LogInformation($"{LogPrefix} Create {entity.GetType().Name}");

        if (await CheckExisting(entity))
        {
            throw new ArgumentException("Duplicate entity");
        }

        var type = entity.GetType();
        var tableName = type.GetTableNameOnly();

        entity.CreatedAt = DateTime.Now;
        entity.CreatedBy = Guid.NewGuid().ToString();
        entity.ModifiedAt = DateTime.Now;
        entity.ModifiedBy = Guid.NewGuid().ToString();

        var param = new DynamicParameters();
        var sql = new StringBuilder();

        var primaryKey = type.GetPrimaryKey().keyTable;
        var columns = type.GetAllColumns();

        var columnsList = string.Join(",", columns);
        sql.Append($"INSERT INTO `{tableName}` ({columnsList}) VALUES");

        var isFirst = true;
        // neu parse fail hoac keyValue bi empty thi tao key moi
        if (!Guid.TryParse(entity.GetValue(primaryKey) + "", out Guid keyValue) || keyValue == Guid.Empty)
        {
            keyValue = Guid.NewGuid();
        }

        var parameterNames = columns.Select(col => $"@{col}_0");
        var s = string.Join(",", parameterNames);

        log.LogDebug("{prefix} Append sql command: {s}", LogPrefix, s);
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
            var value = primaryKey == col ? keyValue : entity.GetValue(col);
            param.Add($"@{col}_0", value);
        }

        log.LogDebug("{prefix} Build command: {command}", LogPrefix, sql);

        var result = await ExecuteCommandText(sql.ToString(), param);
        log.LogDebug("{prefix} Output: {res}", LogPrefix, result);
    }

    public async Task UpdateAsync(T entity, Guid id)
    {
        log.LogInformation("{prefix} Update model details by id: {id}", LogPrefix, id.ToString());
        var type = entity.GetType();
        var primaryKey = type.GetPrimaryKey().keyTable;
        var command = $"SELECT COUNT(*) FROM `{type.Name}` WHERE `{primaryKey}` = @p_Id";
        var param = new DynamicParameters();
        param.Add("@p_Id", id);
        using var conn = context.GetConnection();
        var res = await conn.ExecuteScalarAsync<long>(command, param);
        if (res == 0)
        {
            throw new ArgumentException("Entity not found");
        }
        
        entity.ModifiedBy = Guid.NewGuid().ToString();
        entity.ModifiedAt = DateTime.Now;

        var storedProcedure = string.Format(ProcedureNames.Update, typeof(T).Name);
        param = new DynamicParameters(entity);
        param.Add("p_Id", id);
        var response = await ExecuteCommandText(storedProcedure, param);
        log.LogDebug("{prefix} Response: {res}", LogPrefix, response);
    }

    public Task DeleteAsync(T entity, Guid id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Kiem tra xem 1 obj da ton tai hay chua
    /// </summary>
    /// <param name="entity">Doi tuong can kiem tra</param>
    /// <returns>bool</returns>
    public async Task<bool> CheckExisting(T entity)
    {
        // log.LogInformation("{prefix} Check duplicate", _LogPrefix);
        // var storedProcedure = string.Format(ProcedureNames.CheckExisting, typeof(T).Name);
        //
        // var param = new DynamicParameters();
        // param.Add($"p_{propName}", value);
        //
        // var res = await ExecuteCommandText(storedProcedure, param);
        // log.LogDebug("{prefix} Response: {res}", _LogPrefix, res);
        // return (int)res > 0;
        var type = entity.GetType();
        var tableName = type.GetTableNameOnly();
        var properties = type.GetProperties()
            .Where(p => p.GetCustomAttribute<CheckDuplicateAttribute>(true) is not null);
        var command = new StringBuilder($"SELECT COUNT(*) FROM `{tableName}` WHERE ");
        var propertyInfos = properties.ToList();
        command.Append(string.Join(" OR ", propertyInfos.Select(p => $"`{p.Name}` = @{p.Name}")));

        var parameters = new DynamicParameters();
        foreach (var property in propertyInfos)
        {
            parameters.Add(property.Name, property.GetValue(entity));
        }

        using var conn = context.GetConnection();
        var res = await conn.ExecuteScalarAsync<long>(command.ToString(), parameters);
        Console.WriteLine(res > 0);
        return res > 0;
    }

    /// <summary>
    /// Build query chi danh cho lenh CUD
    /// </summary>
    /// <param name="commandText"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public async Task<object> ExecuteCommandText(string commandText, DynamicParameters parameters)
    {
        log.LogInformation("{prefix} Build query: {command}", LogPrefix, commandText);
        using var conn = context.GetConnection();
        var commandType = commandText.Contains(' ') ? CommandType.Text : CommandType.StoredProcedure;
        return await conn.ExecuteAsync(
            commandText,
            parameters,
            commandType: commandType
        );
    }

    public async Task<int> CountTotalElements()
    {
        log.LogInformation($"{LogPrefix} Count total elements");
        using var conn = context.GetConnection();

        var type = typeof(T);
        var query = $"SELECT COUNT(*) FROM `{type.Name}`";
        return await conn.ExecuteScalarAsync<int>(query);
    }
}