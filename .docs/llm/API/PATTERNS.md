# API Test Code Patterns & Examples

**IMPORTANT:** Before writing database queries, ALWAYS check the actual schema:
- **Location:** `WW7/ww7-api/AO.WW/AO.WW.DB.Client/Tables/[TableName].sql`
- **Why:** Column names, types, and constraints must match exactly
- **Example:** UserLoginDetail has `Created` (not `LoginDateTime`), Id is `BIGINT` (not INT)

---

## Table of Contents

1. [Test Fixture Pattern](#test-fixture-pattern)
2. [Test Class Pattern](#test-class-pattern)
3. [Model Patterns](#model-patterns)
4. [Common Scenarios](#common-scenarios)
5. [OneShot Tests](#oneshot-tests)

---

## Test Fixture Pattern

**Use ApiTestFixture base class for all API test fixtures**

```csharp
using AO.Automation.API.Client.Models.Requests.Login;
using AO.Automation.API.Client.Models.Responses.Login;
using AO.Automation.API.Client.Models.Database;
using Dapper;

namespace AO.Automation.API.Client.Tests.Login;

/// <summary>
/// Fixture for ValidCredentialsLogin - runs setup ONCE for all tests in the class
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
        // ApiHelper and DbConnection are already initialized by base class
        
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
        
        // Query database (using Dapper directly on DbConnection)
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

**Benefits:**
- ✅ ApiHelper already initialized (Playwright APIRequestContext)
- ✅ DbConnection already open (shared across all tests)
- ✅ Dispose handled automatically
- ✅ Setup runs ONCE for all tests in class
- ✅ No boilerplate

---

## Test Class Pattern

**Use IClassFixture<T> to share fixture data**

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
    
    // ... more focused test methods
}
```

**Pattern Benefits:**
- ✅ One API call shared across 10+ tests
- ✅ One DB connection for all queries
- ✅ Clear test names (immediate failure identification)
- ✅ Can filter: `--filter "FullyQualifiedName~Response"` or `~Database`
- ✅ Efficient resource usage

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

### Scenario 1: Test Successful POST with Database Verification

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
    Assert.Equal(request.Username, _fixture.CreatedUser.UserName);
}
```

### Scenario 2: Test Error Response (No Database Changes)

```csharp
public class InvalidLoginFixture : ApiTestFixture
{
    public int StatusCode { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int LoginCountBefore { get; private set; }
    public int LoginCountAfter { get; private set; }
    
    protected override async Task SetupAsync()
    {
        var userId = 9201;
        
        // Count logins before
        LoginCountBefore = await DbConnection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM [dbo].[UserLoginDetail] WHERE UserId = @UserId",
            new { UserId = userId });
        
        // Make API call with invalid credentials
        var request = new LoginRequest { /* invalid */ };
        var response = await ApiHelper.PostAsync<LoginResponse>("/api/user/login", request);
        
        StatusCode = response.StatusCode;
        ErrorMessage = response.Error;
        
        // Count logins after
        LoginCountAfter = await DbConnection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM [dbo].[UserLoginDetail] WHERE UserId = @UserId",
            new { UserId = userId });
    }
}

[Fact]
public void Response_HasUnauthorizedStatus()
{
    Assert.Equal(401, _fixture.StatusCode);
}

[Fact]
public void Database_NoLoginRecordCreated()
{
    Assert.Equal(_fixture.LoginCountBefore, _fixture.LoginCountAfter);
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
- Update seeding script comment
- Mark user as consumed in test documentation
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

### No Database Prefix

**Bad:**
```sql
SELECT * FROM [WW7Client].[dbo].[User]
```

**Good:**
```sql
SELECT Id, UserName FROM [dbo].[User]
```

### Always Use Parameters

**Bad:**
```sql
$"SELECT * FROM [dbo].[User] WHERE UserName = '{username}'"
```

**Good:**
```sql
await DbConnection.QuerySingleOrDefaultAsync<T>(
    "SELECT Id, UserName FROM [dbo].[User] WHERE UserName = @Username",
    new { Username = username });
```

### Use TOP 1 for Single Results

```sql
SELECT TOP 1 Id, RefreshToken 
FROM [dbo].[UserLoginDetail] 
WHERE UserId = @UserId 
ORDER BY Created DESC
```

---

## Tips

### Check Schema First
- Always view table definition before writing queries
- Column names must match exactly (case-sensitive in some DBs)
- Check data types (BIGINT vs INT, NVARCHAR length, etc.)

### Use JsonPropertyName
- API returns camelCase JSON
- C# models use PascalCase properties
- Always add `[JsonPropertyName("fieldName")]` attributes

### Fixture Scope
- One fixture per logical test scenario
- Share expensive operations (API calls, DB queries)
- Keep test methods simple and focused
