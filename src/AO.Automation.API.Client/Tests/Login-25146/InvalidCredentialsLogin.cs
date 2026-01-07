using AO.Automation.API.Client.Models.Requests.Login;
using AO.Automation.API.Client.Models.Responses.Login;
using Dapper;

namespace AO.Automation.API.Client.Tests.Login25146;

/// <summary>
/// Fixture for InvalidCredentialsLogin - provides shared ApiHelper and DbConnection
/// </summary>
public class InvalidCredentialsLoginFixture : ApiTestFixture
{
    protected override Task SetupAsync()
    {
        // Just initialize resources, Theory will make the API calls
        return Task.CompletedTask;
    }
}

/// <summary>
/// Azure Test Case: 25058
/// Invalid credentials are rejected by API
/// </summary>
[Trait("Suite", "Login-25146")]
[Trait("Feature", "Login")]
[Trait("API", "ClientAPI")]
public class InvalidCredentialsLogin : IClassFixture<InvalidCredentialsLoginFixture>
{
    private readonly InvalidCredentialsLoginFixture _fixture;
    
    public InvalidCredentialsLogin(InvalidCredentialsLoginFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Theory]
    [InlineData("api.tc25058.invalidpw@activeops.com", "WrongPassword@1", 9201)]
    [InlineData("api.tc25058.inactive@activeops.com", "Workware@1", 9202)]
    [InlineData("api.tc25058.noroles@activeops.com", "Workware@1", 9203)]
    public async Task InvalidLogin_Returns401AndNoDbRecord(
        string username, 
        string password, 
        int userId)
    {
        // Count login records before
        var loginCountBefore = await _fixture.DbConnection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM [dbo].[UserLoginDetail] WHERE UserId = @UserId",
            new { UserId = userId });
        
        // Act - Make API call with invalid credentials
        var request = new LoginRequest
        {
            ClientIdentifier = "ww7client",
            Username = username,
            Password = password
        };
        
        var response = await _fixture.ApiHelper.PostAsync<LoginResponse>("/api/user/login", request);
        
        // Assert - Response shows error
        Assert.Equal(401, response.StatusCode);
        Assert.NotNull(response.Error);
        Assert.False(response.IsSuccess);
        
        // Assert - Database unchanged (no login record created)
        var loginCountAfter = await _fixture.DbConnection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM [dbo].[UserLoginDetail] WHERE UserId = @UserId",
            new { UserId = userId });
        
        Assert.Equal(loginCountBefore, loginCountAfter);
    }
}
