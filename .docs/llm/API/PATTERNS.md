# API Test Code Patterns & Examples

This document provides reference implementations for common patterns in the API test suite.

---

## Table of Contents

1. [Test Class Patterns](#test-class-patterns)
2. [ApiHelper Patterns](#apihelper-patterns)
3. [DatabaseHelper Patterns](#databasehelper-patterns)
4. [Model Patterns](#model-patterns)
5. [Common Scenarios](#common-scenarios)

---

## Test Class Patterns

### Basic API Test

```csharp
using Xunit;
using AO.Automation.API.Client.Helpers;
using AO.Automation.API.Client.Models.Requests;
using AO.Automation.API.Client.Models.Responses;

namespace AO.Automation.API.Client.Tests.Login;

/// <summary>
/// Azure Test Case: 25057
/// User with valid credentials can login via API
/// </summary>
[Trait("Suite", "Login-25146")]
[Trait("Feature", "Login")]
[Trait("API", "ClientAPI")]
public class ValidCredentialsLogin
{
    private readonly ApiHelper _apiHelper;
    private readonly DatabaseHelper _dbHelper;
    
    public ValidCredentialsLogin()
    {
        _apiHelper = new ApiHelper();
        _dbHelper = new DatabaseHelper();
    }
    
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokenAndCreatesDbRecord()
    {
        // Arrange
        var request = new LoginRequest
        {
            ClientIdentifier = "ww7client",
            Username = "api.tc25057.login@activeops.com",
            Password = "Workware@1"
        };
        
        // Act
        var response = await _apiHelper.LoginAsync(request);
        
        // Assert - Response
        Assert.Equal(200, response.StatusCode);
        Assert.NotNull(response.Data.Token);
        Assert.NotNull(response.Data.RefreshToken);
        Assert.True(response.Data.User.Id > 0);
        
        // Assert - Database
        var dbRecord = await _dbHelper.GetLatestLoginDetail(response.Data.User.Id);
        Assert.NotNull(dbRecord);
        Assert.Equal(response.Data.RefreshToken, dbRecord.RefreshToken);
        
        var timeSinceLogin = DateTime.UtcNow - dbRecord.LoginDateTime;
        Assert.True(timeSinceLogin.TotalSeconds < 5);
    }
}
```

### Multiple Scenarios (Theory)

```csharp
/// <summary>
/// Azure Test Case: 25058
/// Invalid credentials are rejected
/// </summary>
[Trait("Suite", "Login-25146")]
[Trait("Feature", "Login")]
public class InvalidCredentialsLogin
{
    [Theory]
    [InlineData("api.tc25058.invalidpw@activeops.com", "WrongPassword@1", "invalid password")]
    [InlineData("api.tc25058.inactive@activeops.com", "Workware@1", "inactive user")]
    [InlineData("api.tc25058.noroles@activeops.com", "Workware@1", "no roles")]
    public async Task Login_WithInvalidCredentials_Returns401(
        string username, 
        string password, 
        string scenario)
    {
        // Arrange
        var request = new LoginRequest
        {
            ClientIdentifier = "ww7client",
            Username = username,
            Password = password
        };
        
        // Act
        var response = await _apiHelper.LoginAsync(request);
        
        // Assert
        Assert.Equal(401, response.StatusCode);
        Assert.NotNull(response.Error);
        
        // Assert - Database (no record created)
        var loginCount = await _dbHelper.CountRecentLogins(username, seconds: 10);
        Assert.Equal(0, loginCount);
    }
}
```

---

## ApiHelper Patterns

### Basic Implementation

```csharp
using System.Net.Http.Json;
using System.Text.Json;

namespace AO.Automation.API.Client.Helpers;

public class ApiHelper
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    
    public ApiHelper()
    {
        _baseUrl = TestConfig.Current.ApiBaseUrl;
        _httpClient = new HttpClient { BaseAddress = new Uri(_baseUrl) };
    }
    
    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/user/login", request);
        
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return new ApiResponse<LoginResponse>
            {
                StatusCode = (int)response.StatusCode,
                Data = data
            };
        }
        
        var error = await response.Content.ReadAsStringAsync();
        return new ApiResponse<LoginResponse>
        {
            StatusCode = (int)response.StatusCode,
            Error = error
        };
    }
}
```

### Generic Request Method

```csharp
public async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object body)
{
    var response = await _httpClient.PostAsJsonAsync(endpoint, body);
    
    if (response.IsSuccessStatusCode)
    {
        var data = await response.Content.ReadFromJsonAsync<T>();
        return new ApiResponse<T>
        {
            StatusCode = (int)response.StatusCode,
            Data = data
        };
    }
    
    return new ApiResponse<T>
    {
        StatusCode = (int)response.StatusCode,
        Error = await response.Content.ReadAsStringAsync()
    };
}
```

### Authenticated Requests

```csharp
public void SetAuthToken(string token)
{
    _httpClient.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", token);
}

public async Task<ApiResponse<T>> AuthenticatedGetAsync<T>(string endpoint)
{
    // Assumes SetAuthToken was called previously
    var response = await _httpClient.GetAsync(endpoint);
    // ... handle response
}
```

---

## DatabaseHelper Patterns

### Basic Implementation

```csharp
using Dapper;
using Microsoft.Data.SqlClient;

namespace AO.Automation.API.Client.Helpers;

public class DatabaseHelper
{
    private readonly string _connectionString;
    
    public DatabaseHelper()
    {
        _connectionString = TestConfig.Current.DatabaseConnectionString;
    }
    
    public async Task<T?> QuerySingleAsync<T>(string sql, object? parameters = null)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<T>(sql, parameters);
    }
    
    public async Task<List<T>> QueryAsync<T>(string sql, object? parameters = null)
    {
        using var connection = new SqlConnection(_connectionString);
        var results = await connection.QueryAsync<T>(sql, parameters);
        return results.ToList();
    }
}
```

### Common Verification Methods

```csharp
// Get latest login detail for user
public async Task<UserLoginDetailRecord?> GetLatestLoginDetail(int userId)
{
    const string sql = @"
        SELECT TOP 1 * 
        FROM [WW7Client].[dbo].[UserLoginDetail] 
        WHERE UserId = @UserId 
        ORDER BY LoginDateTime DESC";
    
    return await QuerySingleAsync<UserLoginDetailRecord>(sql, new { UserId = userId });
}

// Count recent logins
public async Task<int> CountRecentLogins(int userId, int seconds = 10)
{
    const string sql = @"
        SELECT COUNT(*) 
        FROM [WW7Client].[dbo].[UserLoginDetail] 
        WHERE UserId = @UserId 
        AND LoginDateTime >= DATEADD(SECOND, -@Seconds, GETUTCDATE())";
    
    return await QuerySingleAsync<int>(sql, new { UserId = userId, Seconds = seconds });
}

// Get user by ID
public async Task<UserRecord?> GetUserById(int userId)
{
    const string sql = "SELECT * FROM [WW7Client].[dbo].[User] WHERE Id = @UserId";
    return await QuerySingleAsync<UserRecord>(sql, new { UserId = userId });
}
```

---

## Model Patterns

### Request Models

```csharp
namespace AO.Automation.API.Client.Models.Requests;

public class LoginRequest
{
    public string ClientIdentifier { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
```

### Response Models

```csharp
namespace AO.Automation.API.Client.Models.Responses;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public UserInfo User { get; set; } = new();
}

public class UserInfo
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int StaffMemberId { get; set; }
    public string Name { get; set; } = string.Empty;
}
```

### Generic API Response Wrapper

```csharp
namespace AO.Automation.API.Client.Models;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
    public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;
}
```

### Database Record Models

```csharp
namespace AO.Automation.API.Client.Models.Database;

public class UserLoginDetailRecord
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime LoginDateTime { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiry { get; set; }
    public string SessionValidationToken { get; set; } = string.Empty;
}

public class UserRecord
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int ActivationStatusId { get; set; }
    public bool Active { get; set; }
    public string? PasswordHash { get; set; }
}
```

---

## Common Scenarios

### Scenario 1: Test POST Creates Record

```csharp
[Fact]
public async Task CreateUser_WithValidData_CreatesDbRecord()
{
    // Arrange
    var request = new CreateUserRequest { /* ... */ };
    
    // Act
    var response = await _apiHelper.CreateUserAsync(request);
    
    // Assert - Response
    Assert.Equal(201, response.StatusCode);
    Assert.True(response.Data.UserId > 0);
    
    // Assert - Database
    var dbUser = await _dbHelper.GetUserById(response.Data.UserId);
    Assert.NotNull(dbUser);
    Assert.Equal(request.Username, dbUser.UserName);
}
```

### Scenario 2: Test DELETE Removes Record

```csharp
[Fact]
public async Task DeleteUser_RemovesFromDatabase()
{
    // Arrange - Verify user exists
    var userId = 9250; // Pre-seeded test user
    var userBefore = await _dbHelper.GetUserById(userId);
    Assert.NotNull(userBefore);
    
    // Act
    var response = await _apiHelper.DeleteUserAsync(userId);
    
    // Assert - Response
    Assert.Equal(204, response.StatusCode); // No content
    
    // Assert - Database
    var userAfter = await _dbHelper.GetUserById(userId);
    Assert.Null(userAfter); // User deleted
}
```

### Scenario 3: Test PUT Updates Record

```csharp
[Fact]
public async Task UpdateUser_ChangesDbRecord()
{
    // Arrange
    var userId = 9251;
    var updateRequest = new UpdateUserRequest 
    { 
        UserId = userId,
        Email = "updated.email@example.com" 
    };
    
    // Act
    var response = await _apiHelper.UpdateUserAsync(updateRequest);
    
    // Assert - Response
    Assert.Equal(200, response.StatusCode);
    
    // Assert - Database
    var dbUser = await _dbHelper.GetUserById(userId);
    Assert.NotNull(dbUser);
    Assert.Equal("updated.email@example.com", dbUser.Email);
}
```

### Scenario 4: Test Error Response (No DB Change)

```csharp
[Fact]
public async Task Login_WithInvalidPassword_Returns401AndNoDbRecord()
{
    // Arrange
    var userId = 9201;
    var loginsBefore = await _dbHelper.CountRecentLogins(userId);
    
    // Act
    var response = await _apiHelper.LoginAsync(new LoginRequest 
    { 
        Username = "api.tc25058.invalidpw@activeops.com",
        Password = "WrongPassword"
    });
    
    // Assert - Response
    Assert.Equal(401, response.StatusCode);
    Assert.NotNull(response.Error);
    
    // Assert - Database (no new records)
    var loginsAfter = await _dbHelper.CountRecentLogins(userId);
    Assert.Equal(loginsBefore, loginsAfter);
}
```

### Scenario 5: Test Token Validation

```csharp
using System.IdentityModel.Tokens.Jwt;

[Fact]
public async Task Login_ReturnsValidJwtToken()
{
    // Arrange & Act
    var response = await _apiHelper.LoginAsync(validRequest);
    
    // Assert - Response
    Assert.Equal(200, response.StatusCode);
    
    // Parse and validate JWT
    var handler = new JwtSecurityTokenHandler();
    var token = handler.ReadJwtToken(response.Data.Token);
    
    Assert.Contains(token.Claims, c => c.Type == "username");
    Assert.Contains(token.Claims, c => c.Type == "ClientIdentifier");
    
    var tokenExpiry = token.ValidTo;
    var expectedExpiry = DateTime.UtcNow.AddMinutes(30);
    var delta = Math.Abs((tokenExpiry - expectedExpiry).TotalMinutes);
    Assert.True(delta < 2, "Token expiry should be ~30 min from now");
}
```

---

## Configuration Pattern

### TestConfig Class

```csharp
using Microsoft.Extensions.Configuration;

namespace AO.Automation.API.Client.Config;

public class TestConfig
{
    private static TestConfig? _current;
    private readonly IConfiguration _configuration;
    
    public static TestConfig Current => _current ??= new TestConfig();
    
    private TestConfig()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local"}.json", optional: true)
            .Build();
    }
    
    public string ApiBaseUrl => _configuration["Api:BaseUrl"] 
        ?? "http://localhost/api";
    
    public string DatabaseConnectionString => _configuration["Database:ConnectionString"]
        ?? "Server=localhost;Database=WW7Client;User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True";
}
```

### appsettings.json

```json
{
  "Api": {
    "BaseUrl": "http://localhost/api"
  },
  "Database": {
    "ConnectionString": "Server=localhost;Database=WW7Client;User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True"
  }
}
```

---

## Common Assertions

### Status Code Assertions

```csharp
// Success responses
Assert.Equal(200, response.StatusCode); // OK
Assert.Equal(201, response.StatusCode); // Created
Assert.Equal(204, response.StatusCode); // No Content

// Client errors
Assert.Equal(400, response.StatusCode); // Bad Request
Assert.Equal(401, response.StatusCode); // Unauthorized
Assert.Equal(403, response.StatusCode); // Forbidden
Assert.Equal(404, response.StatusCode); // Not Found
Assert.Equal(409, response.StatusCode); // Conflict

// Multiple possibilities
Assert.True(response.StatusCode == 401 || response.StatusCode == 403);
```

### Response Field Assertions

```csharp
// Null/empty checks
Assert.NotNull(response.Data);
Assert.NotNull(response.Data.Token);
Assert.NotEmpty(response.Data.Token);

// Value checks
Assert.Equal("expected", response.Data.Field);
Assert.True(response.Data.Id > 0);
Assert.Contains("keyword", response.Data.Message);

// Error responses
Assert.NotNull(response.Error);
Assert.Contains("invalid", response.Error.ToLower());
```

### Database Assertions

```csharp
// Record exists
Assert.NotNull(dbRecord);

// Field matches
Assert.Equal(expectedValue, dbRecord.Field);

// Timestamp validations
var timeDelta = DateTime.UtcNow - dbRecord.CreatedAt;
Assert.True(timeDelta.TotalSeconds < 5);

// Count checks
Assert.Equal(0, count); // No records
Assert.True(count > 0); // At least one record
```

### JWT Token Assertions

```csharp
var handler = new JwtSecurityTokenHandler();

// Valid format
Assert.True(handler.CanReadToken(tokenString));

// Parse and check claims
var token = handler.ReadJwtToken(tokenString);
Assert.Contains(token.Claims, c => c.Type == "username" && c.Value == expectedUsername);

// Expiry
var tokenExpiry = token.ValidTo;
var now = DateTime.UtcNow;
Assert.True(tokenExpiry > now, "Token should not be expired");
```

---

## Tips

### Keep Tests Simple
- One test method = one scenario
- Clear test names describe what's being tested
- Avoid complex setup logic

### Database Verification Best Practices
- Always verify for state-changing operations (POST, PUT, DELETE)
- Use specific queries (WHERE UserId = @UserId)
- Check timestamps are recent (within seconds)
- Verify relationships (foreign keys)

### Error Testing
- Test invalid inputs return appropriate errors
- Verify no database changes on errors
- Check error messages don't leak sensitive info

### Token Management
- Generate tokens via TokenHelper for speed
- Test login endpoint explicitly in auth tests
- Reuse tokens across tests when appropriate
