using Dapper;
using Microsoft.Data.SqlClient;

namespace AO.Automation.API.Client.Helpers;

/// <summary>
/// Database access helper using Dapper for raw SQL queries
/// </summary>
public class DatabaseHelper
{
    private readonly string _connectionString;
    
    public DatabaseHelper()
    {
        _connectionString = ApiTestConfig.Instance.DatabaseConnectionString;
    }
    
    /// <summary>
    /// Execute a query and return a single result or null
    /// </summary>
    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<T>(sql, parameters);
    }
    
    /// <summary>
    /// Execute a query and return a list of results
    /// </summary>
    public async Task<List<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        using var connection = new SqlConnection(_connectionString);
        var results = await connection.QueryAsync<T>(sql, parameters);
        return results.ToList();
    }
    
    /// <summary>
    /// Execute a query and return a scalar value (e.g., COUNT)
    /// Returns default(T) if result is null
    /// </summary>
    public async Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null)
    {
        using var connection = new SqlConnection(_connectionString);
        var result = await connection.ExecuteScalarAsync<T>(sql, parameters);
        return result ?? default(T)!;
    }
    
    // Login-specific convenience methods
    
    /// <summary>
    /// Get the most recent login detail for a user
    /// </summary>
    public async Task<Models.Database.UserLoginDetailRecord?> GetLatestLoginDetailAsync(int userId)
    {
        const string sql = @"
            SELECT TOP 1 * 
            FROM [WW7Client].[dbo].[UserLoginDetail] 
            WHERE UserId = @UserId 
            ORDER BY LoginDateTime DESC";
        
        return await QuerySingleOrDefaultAsync<Models.Database.UserLoginDetailRecord>(sql, new { UserId = userId });
    }
    
    /// <summary>
    /// Count recent login attempts for a user
    /// </summary>
    public async Task<int> CountRecentLoginsAsync(int userId, int seconds = 10)
    {
        const string sql = @"
            SELECT COUNT(*) 
            FROM [WW7Client].[dbo].[UserLoginDetail] 
            WHERE UserId = @UserId 
            AND LoginDateTime >= DATEADD(SECOND, -@Seconds, GETUTCDATE())";
        
        return await ExecuteScalarAsync<int>(sql, new { UserId = userId, Seconds = seconds });
    }
}
