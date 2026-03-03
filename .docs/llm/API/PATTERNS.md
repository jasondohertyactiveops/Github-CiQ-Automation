# API Code Patterns

Reference implementations for common patterns.

**IMPORTANT:** Before writing database queries, always check actual schema at `WW7/ww7-api/AO.WW/AO.WW.DB.Client/Tables/[TableName].sql`

---

## Pattern 1: Fixture with Multiple Focused Tests

One API call in fixture, many focused assertions. Use when testing one operation with many validations.

```csharp
public class ValidCredentialsLoginFixture : ApiTestFixture
{
    public int LoginStatusCode { get; private set; }
    public LoginResponse? LoginResponse { get; private set; }
    public UserLoginDetailRecord? LoginDetailRecord { get; private set; }

    protected override async Task SetupAsync()
    {
        var request = new LoginRequest
        {
            ClientIdentifier = "ww7client",
            Username = "api.tc25057.login@activeops.com",
            Password = "Workware@1"
        };

        var response = await ApiHelper.PostAsync<LoginResponse>("/api/user/login", request);
        LoginStatusCode = response.StatusCode;
        LoginResponse = response.Data;

        // Database verification via Dapper on shared DbConnection
        LoginDetailRecord = await DbConnection.QuerySingleOrDefaultAsync<UserLoginDetailRecord>(
            "SELECT TOP 1 Id, UserId, RefreshToken, RefreshTokenExpiry, Created FROM [dbo].[UserLoginDetail] WHERE UserId = @UserId ORDER BY Created DESC",
            new { UserId = 9200 });
    }
}

[AzureTestSuite(25146)] // Login
[AzureTestCase(25057)]
[AzureTestPlan("Smoke")]
[AzureTestPlan("Regression")]
public class ValidCredentialsLogin : IClassFixture<ValidCredentialsLoginFixture>
{
    private readonly ValidCredentialsLoginFixture _fixture;
    public ValidCredentialsLogin(ValidCredentialsLoginFixture fixture) { _fixture = fixture; }

    [Fact]
    [AzureTestStep(25057, 1)]
    public void Response_HasSuccessStatusCode() => Assert.Equal(200, _fixture.LoginStatusCode);

    [Fact]
    [AzureTestStep(25057, 2)]
    public void Response_ContainsToken()
    {
        Assert.NotNull(_fixture.LoginResponse);
        Assert.NotEmpty(_fixture.LoginResponse.Token);
    }

    [Fact]
    [AzureTestStep(25057, 9)]
    public void Database_LoginRecordCreated()
    {
        Assert.NotNull(_fixture.LoginDetailRecord);
        Assert.True(_fixture.LoginDetailRecord.Id > 0);
    }
}
```

---

## Pattern 2: Theory with InlineData

Same test logic with different inputs. Use for negative testing and multiple error scenarios.

```csharp
public class InvalidCredentialsLoginFixture : ApiTestFixture
{
    protected override Task SetupAsync() => Task.CompletedTask;
}

[AzureTestSuite(25146)] // Login
[AzureTestCase(25058)]
[AzureTestPlan("Smoke")]
[AzureTestPlan("Regression")]
public class InvalidCredentialsLogin : IClassFixture<InvalidCredentialsLoginFixture>
{
    private readonly InvalidCredentialsLoginFixture _fixture;
    public InvalidCredentialsLogin(InvalidCredentialsLoginFixture fixture) { _fixture = fixture; }

    [Theory]
    [AzureTestStep(25058, 4)]
    [InlineData("api.tc25058.invalidpw@activeops.com", "WrongPassword@1", 9201)]
    [InlineData("api.tc25058.inactive@activeops.com", "Workware@1", 9202)]
    [InlineData("api.tc25058.noroles@activeops.com", "Workware@1", 9203)]
    public async Task InvalidLogin_Returns401AndNoDbRecord(string username, string password, int userId)
    {
        var countBefore = await _fixture.DbConnection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM [dbo].[UserLoginDetail] WHERE UserId = @UserId",
            new { UserId = userId });

        var request = new LoginRequest { ClientIdentifier = "ww7client", Username = username, Password = password };
        var response = await _fixture.ApiHelper.PostAsync<LoginResponse>("/api/user/login", request);

        Assert.Equal(401, response.StatusCode);
        Assert.False(response.IsSuccess);

        var countAfter = await _fixture.DbConnection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM [dbo].[UserLoginDetail] WHERE UserId = @UserId",
            new { UserId = userId });
        Assert.Equal(countBefore, countAfter);
    }
}
```

---

## Model Patterns

```csharp
// Request: Models/Requests/Login/LoginRequest.cs
public class LoginRequest
{
    public string ClientIdentifier { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// Response: Models/Responses/Login/LoginResponse.cs (always use JsonPropertyName)
public class LoginResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
}

// Database: Models/Database/UserLoginDetailRecord.cs
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

## Database Query Rules

```sql
-- ✅ Specific columns, parameterised, [dbo] schema
SELECT Id, UserName FROM [dbo].[User] WHERE UserName = @Username

-- ✅ TOP 1 with ORDER BY for most recent
SELECT TOP 1 Id, RefreshToken, Created FROM [dbo].[UserLoginDetail]
WHERE UserId = @UserId ORDER BY Created DESC

-- ❌ No SELECT *, no database prefix, no string interpolation
SELECT * FROM [WW7Client].[dbo].[User] WHERE UserName = '{username}'
```

---

## Timing Assertions

Use database timestamps as source of truth, not `DateTime.UtcNow`:

```csharp
// ✅ Compare expiry to DB timestamp (no clock drift)
var minutesDifference = (tokenExpiry - loginRecord.Created).TotalMinutes;
Assert.InRange(minutesDifference, 28, 32);

// ❌ Comparing to UtcNow drifts during test execution
var minutes = (tokenExpiry - DateTime.UtcNow).TotalMinutes;
```

---

## Pattern Decision Guide

| Scenario | Pattern |
|----------|---------|
| Many assertions on one API call | Fixture + multiple Facts |
| Same validation, different inputs | Theory + InlineData |
| Simple endpoint check | Single Fact in fixture |
| Expensive setup (login + DB queries) | Fixture (runs once) |
| Error scenarios (3+ cases) | Theory |
