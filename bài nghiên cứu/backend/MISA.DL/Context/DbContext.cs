using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace MISA.DL.Context;

public class DbContext(IConfiguration config)
{
    private readonly string _connString = config.GetConnectionString("MISA_AMIS_Conn")!;

    public IDbConnection GetConnection()
    {
        return new MySqlConnection(_connString);
    }
}