using Microsoft.Data.SqlClient;

namespace AO.Automation.API.Client.BaseClasses;

/// <summary>
/// Base class for API test fixtures
/// Provides shared ApiHelper and database connection that are initialized once per test class
/// Derived fixtures should override SetupAsync() to perform test-specific setup (like making API calls)
/// </summary>
public abstract class ApiTestFixture : IAsyncLifetime
{
    public ApiHelper ApiHelper { get; private set; } = null!;
    public SqlConnection DbConnection { get; private set; } = null!;
    
    public async Task InitializeAsync()
    {
        // Initialize Playwright API context
        ApiHelper = new ApiHelper();
        await ApiHelper.InitializeAsync();
        
        // Open database connection
        DbConnection = new SqlConnection(ApiTestConfig.Instance.DatabaseConnectionString);
        await DbConnection.OpenAsync();
        
        // Allow derived fixture to do custom setup (e.g., make API calls, query data)
        await SetupAsync();
    }
    
    /// <summary>
    /// Override this in derived fixtures to perform test-specific setup
    /// ApiHelper and DbConnection are available at this point
    /// </summary>
    protected abstract Task SetupAsync();
    
    public async Task DisposeAsync()
    {
        if (DbConnection != null)
        {
            await DbConnection.DisposeAsync();
        }
        if (ApiHelper != null)
        {
            await ApiHelper.DisposeAsync();
        }
    }
}
