# API Test Code Patterns & Examples

**IMPORTANT:** Before writing database queries, ALWAYS check the actual schema:
- **Location:** `WW7/ww7-api/AO.WW/AO.WW.DB.Client/Tables/[TableName].sql`
- **Why:** Column names, types, and constraints must match exactly
- **Example:** UserLoginDetail has `Created` (not `LoginDateTime`), Id is `BIGINT` (not INT)

---

## Choosing the Right Pattern

### Pattern 1: Fixture with Multiple Focused Tests

**When to use:**
- Testing one API operation with many assertions
- Want to reuse expensive setup (API call, DB queries)
- Need clear isolation of what failed

**Example:** TC25057 - Valid login has 12 different validations (token format, claims, DB state, etc.)

### Pattern 2: Theory with InlineData

**When to use:**
- Same test logic with different inputs
- Testing multiple error scenarios
- Negative testing (invalid inputs)

**Example:** TC25058 - Invalid login with 3 scenarios (wrong password, inactive user, no roles)

### Pattern 3: Simple Single Test

**When to use:**
- Simple API call with few assertions
- Quick smoke tests
- One-off scenarios

**Example:** Simple GET endpoint validation

---

## Pattern 1: Fixture with Multiple Focused Tests

**Complete implementation of TC25057 showing fixture pattern**

### Step 1: Create Fixture Class

```csharp
using AO.Automation.API.Client.Models.Requests.Login;
using AO.Automation.API.Client.Models.Responses.Login;
using AO.Automation.API.Client.Models.Database;
using Dapper;

namespace AO.Automation.API.Client.Tests.Login;

/// <summary>
/// Fixture runs setup ONCE for all tests in ValidCredentialsLogin class
/// </summary>
public class ValidCredentialsLoginFixture : ApiTestFixture
{
    // Expose data needed by tests
    public int LoginStatusCode { get; private set; }
    public LoginResponse? LoginResponse { get; private set; }
    public UserRecord? UserRecord { get; private set; }
    public UserLoginDetailRecord? LoginDetailRecord { get; private set; }
    
    protected override async Task SetupAsync()
    {
        // ApiHelper and DbConnection already initialized by base class
        
        // Make API call
        var request = new LoginRequest
        {
            ClientIdentifier = "ww7client",
            Username = "api.tc25057.login@activeops.com",
            Password = "Workware@1"
        };
        
        var response = await ApiHelper.PostAsync<LoginResponse>("/api/user/login", request);
        LoginStatusCode = response.StatusCode;
        LoginResponse = response.Data;
        
        // Query database using Dapper directly on DbConnection
        UserRecord = await DbConnection.QuerySingleOrDefaultAsync<UserRecord>(
            "SELECT Id, UserName, StaffMemberId FROM [dbo].[User] WHERE UserName = @Username",
            new { Username = request.Username });
        
        if (UserRecord != null)
        {
            LoginDetailRecord = await DbConnection.QuerySingleOrDefaultAsync<UserLoginDetailRecord>(
                "SELECT TOP 1 Id, UserId, RefreshToken, RefreshTokenExpiry, Created FROM [dbo].[UserLoginDetail] WHERE UserId = @UserId ORDER BY Created DESC",
                new { UserId = UserRecord.Id });
        }
    }
}
```

### Step 2: Create Test Class

```csharp
/// <summary>
/// Azure Test Case: 25057
/// User with valid credentials can successfully login via API
/// </summary>
[Trait("Suite", "Login-25146")]
[Trait("Feature", "Login")]
[Trait("API", "ClientAPI")]
public class ValidCredentialsLogin : IClassFixture<ValidCredentialsLoginFixture>
{
    private readonly ValidCredentialsLoginFixture _fixture;
    
    public ValidCredentialsLogin(ValidCredentialsLoginFixture fixture)
    {
        _fixture = fixture;
    }
    
    // Response validation tests
    
    [Fact]
    public void Response_HasSuccessStatusCode()
    {
        Assert.Equal(200, _fixture.LoginStatusCode);
    }
    
    [Fact]
    public void Response_ContainsToken()
    {
        Assert.NotNull(_fixture.LoginResponse);
        Assert.NotNull(_fixture.LoginResponse.Token);
        Assert.NotEmpty(_fixture.LoginResponse.Token);
    }
    
    [Fact]
    public void Database_LoginRecordCreated()
    {
        Assert.NotNull(_fixture.LoginDetailRecord);
        Assert.True(_fixture.LoginDetailRecord.Id > 0);
    }
    
    [Fact]
    public void Database_RefreshTokenMatchesResponse()
    {
        Assert.NotNull(_fixture.LoginResponse);
        Assert.NotNull(_fixture.LoginDetailRecord);
        Assert.Equal(_fixture.LoginResponse.RefreshToken, _fixture.LoginDetailRecord.RefreshToken);
    }
    
    // ... 8 more focused test methods
}
```

### Pattern 1 Benefits

- ✅ One API call shared across 12 tests
- ✅ One DB connection for all queries  
- ✅ Clear test names (know exactly what failed)
- ✅ Can filter tests: `--filter "FullyQualifiedName~Response"`
- ✅ Efficient resource usage (critical for 100+ test classes)

---

## Pattern 2: Theory with InlineData

**Use for testing same validation logic with different inputs**

```csharp
using AO.Automation.API.Client.Models.Requests.Login;
using AO.Automation.API.Client.Models.Responses.Login;
using Microsoft.Data.SqlClient;
using Dapper;

namespace AO.Automation.API.Client.Tests.Login;

/// <summary>
/// Azure Test Case: 25058
/// Invalid credentials are rejected by API
/// </summary>
[Trait("Suite", "Login-25146")]
[Trait("Feature", "Login")]
[Trait("API", "ClientAPI")]
public class InvalidCredentialsLogin
{
    [Theory]
    [InlineData("api.tc25058.invalidpw@activeops.com", "WrongPassword@1", 9201, "invalid password")]
    [InlineData("api.tc25058.inactive@activeops.com", "Workware@1", 9202, "inactive user")]
    [InlineData("api.tc25058.noroles@activeops.com", "Workware@1", 9203, "no roles")]
    public async Task InvalidLogin_Returns401AndNoDbRecord(
        string username, 
        string password, 
        int userId,
        string scenario)
    {
        // Arrange - Initialize resources
        var apiHelper = new ApiHelper();
        await apiHelper.InitializeAsync();
        
        using var dbConnection = new SqlConnection(ApiTestConfig.Instance.DatabaseConnectionString);
        await dbConnection.OpenAsync();
        
        // Count login records before
        var loginCountBefore = await dbConnection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM [dbo].[UserLoginDetail] WHERE UserId = @UserId",
            new { UserId = userId });
        
        // Act - Make API call with invalid credentials
        var request = new LoginRequest
        {
            ClientIdentifier = "ww7client",
            Username = username,
            Password = password
        };
        
        var response = await apiHelper.PostAsync<LoginResponse>("/api/user/login", request);
        
        // Assert - Response shows error
        Assert.Equal(401, response.StatusCode);
        Assert.NotNull(response.Error);
        
        // Assert - Database unchanged (no login record created)
        var loginCountAfter = await dbConnection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM [dbo].[UserLoginDetail] WHERE UserId = @UserId",
            new { UserId = userId });
        
        Assert.Equal(loginCountBefore, loginCountAfter);
        
        // Cleanup
        await apiHelper.DisposeAsync();
    }
}
```

### Pattern 2 Benefits

- ✅ One test method handles multiple scenarios
- ✅ Test output clearly shows which scenario failed
- ✅ Less code than separate tests or fixtures
- ✅ Perfect for negative testing and edge cases

---

## Model Patterns

### Request Models

**Feature-based organization:** `Models/Requests/Login/`

```csharp
namespace AO.Automation.API.Client.Models.Requests.Login;

public class LoginRequest
{
    public string ClientIdentifier { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
```

### Response Models

**Feature-based organization:** `Models/Responses/Login/`

**IMPORTANT:** Use JsonPropertyName to map camelCase JSON to PascalCase C#

```csharp
using System.Text.Json.Serialization;

namespace AO.Automation.API.Client.Models.Responses.Login;

public class LoginResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
    
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
    
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
    
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; } = string.Empty;
    
    [JsonPropertyName("lastName")]
    public string LastName { get; set; } = string.Empty;
    
    // ... other fields
}
```

### Database Record Models

```csharp
namespace AO.Automation.API.Client.Models.Database;

public class UserLoginDetailRecord
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiry { get; set; }
    public DateTime Created { get; set; }
}
```

---

## Common Scenarios

### Test POST Creates Database Record

```csharp
public class CreateUserFixture : ApiTestFixture
{
    public ApiResponse<CreateUserResponse>? Response { get; private set; }
    public UserRecord? CreatedUser { get; private set; }
    
    protected override async Task SetupAsync()
    {
        var request = new CreateUserRequest { /* ... */ };
        Response = await ApiHelper.PostAsync<CreateUserResponse>("/api/user", request);
        
        if (Response.IsSuccess)
        {
            CreatedUser = await DbConnection.QuerySingleOrDefaultAsync<UserRecord>(
                "SELECT Id, UserName, Email FROM [dbo].[User] WHERE Id = @UserId",
                new { UserId = Response.Data.UserId });
        }
    }
}

[Fact]
public void Response_HasCreatedStatusCode()
{
    Assert.Equal(201, _fixture.Response!.StatusCode);
}

[Fact]
public void Database_UserRecordExists()
{
    Assert.NotNull(_fixture.CreatedUser);
}
```

### Test Error Response (No Database Changes)

```csharp
[Theory]
[InlineData("invalid-input-1")]
[InlineData("invalid-input-2")]
public async Task InvalidRequest_Returns400AndNoDbChange(string invalidInput)
{
    var apiHelper = new ApiHelper();
    await apiHelper.InitializeAsync();
    
    // Count records before
    // Make API call
    // Assert error response
    // Assert record count unchanged
    
    await apiHelper.DisposeAsync();
}
```

---

## OneShot Tests

### What Makes a Test OneShot?

**OneShot tests modify user state permanently:**
- Activate accounts (can't activate twice)
- Change passwords (old password no longer works)
- Lock accounts (failed logins increment lockout counter)
- Delete entities (can't delete twice)

### Marking OneShot Tests

```csharp
[Trait("Suite", "Login-25146")]
[Trait("Feature", "Login")]
[Trait("API", "ClientAPI")]
[Trait("Category", "OneShot")]  // ← Mark as OneShot
public class AccountActivationApi : IClassFixture<AccountActivationApiFixture>
{
    // Test activates user 9205 - can only run once per database reset
}
```

### Running Tests

**Skip OneShot during development:**
```powershell
dotnet test --filter "Category!=OneShot"
```

**Run everything (requires fresh database after):**
```powershell
dotnet test
```

**Database Reset:**
```powershell
cd D:\ActiveOpsGit\WW7\misc\Docker\local-environment
.\recreate-databases.ps1
```

### OneShot User Management

**Document which user consumed:**
- Update seeding script comment with test name
- Mark user as consumed in SEEDING-REFERENCE.md
- Example: User 9205 consumed by TC25059 activation test

---

## Database Query Best Practices

### Use Specific Columns

**Bad:**
```sql
SELECT * FROM [dbo].[UserLoginDetail]
```

**Good:**
```sql
SELECT Id, UserId, RefreshToken, RefreshTokenExpiry, Created 
FROM [dbo].[UserLoginDetail]
```

**Why:** Less data transfer, clearer intent, fails fast if column doesn't exist

### No Database Prefix

**Bad:**
```sql
SELECT * FROM [WW7Client].[dbo].[User]
```

**Good:**
```sql
SELECT Id, UserName FROM [dbo].[User]
```

**Why:** Connection string already specifies database

### Always Use Parameters

**Bad:**
```sql
$"SELECT * FROM [dbo].[User] WHERE UserName = '{username}'"  // SQL injection risk!
```

**Good:**
```sql
await DbConnection.QuerySingleOrDefaultAsync<UserRecord>(
    "SELECT Id, UserName FROM [dbo].[User] WHERE UserName = @Username",
    new { Username = username });
```

### Use TOP 1 for Single Results

**When querying for most recent:**
```sql
SELECT TOP 1 Id, RefreshToken, Created 
FROM [dbo].[UserLoginDetail] 
WHERE UserId = @UserId 
ORDER BY Created DESC
```

**Why:** Prevents "Sequence contains more than one element" errors

---

## Tips

### Check Schema First
- Always view `Tables/[TableName].sql` before writing queries
- Column names must match exactly
- Check data types (BIGINT vs INT, NVARCHAR length)
- Note nullable columns

### Use JsonPropertyName
- API returns camelCase JSON (`token`, `refreshToken`)
- C# models use PascalCase (`Token`, `RefreshToken`)
- Always add `[JsonPropertyName("fieldName")]` attributes

### Fixture vs Theory Decision

**Use Fixture (Pattern 1) when:**
- Many different assertions on same API call
- Expensive setup you want to reuse
- Testing complex response structure

**Use Theory (Pattern 2) when:**
- Same assertions, different inputs
- Error scenarios with similar validation
- Less than 5-6 assertions total

### Connection Management

**Pattern 1 (Fixture):**
- Base class manages connection
- Use `DbConnection` property
- Shared across all test methods

**Pattern 2 (Theory):**
- Create connection per test run
- Use `using var dbConnection = new SqlConnection(...)`
- Dispose after test

---

## Quick Reference

| Need to... | Code... |
|------------|---------|
| Make POST request | `await ApiHelper.PostAsync<TResponse>(endpoint, body)` |
| Query single record | `await DbConnection.QuerySingleOrDefaultAsync<T>(sql, params)` |
| Query multiple | `await DbConnection.QueryAsync<T>(sql, params)` |
| Count records | `await DbConnection.ExecuteScalarAsync<int>(sql, params)` |
| Check response | `Assert.Equal(200, response.StatusCode)` |
| Check DB record | `Assert.NotNull(record)` |

---

## Need More Examples?

- Check actual test files in `Tests/Login/` for working examples
- TC25057 (ValidCredentialsLogin.cs) - Pattern 1 example
- Check Swagger for API contracts
- Check `Tables/` folder for database schemas
