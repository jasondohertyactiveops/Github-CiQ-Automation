using System.IdentityModel.Tokens.Jwt;
using AO.Automation.API.Client.Models.Requests.Login;
using AO.Automation.API.Client.Models.Responses.Login;
using AO.Automation.API.Client.Models.Database;
using Dapper;

namespace AO.Automation.API.Client.Tests.Login;

/// <summary>
/// Fixture for ValidCredentialsLogin - runs setup ONCE for all tests
/// </summary>
public class ValidCredentialsLoginFixture : ApiTestFixture
{
    public int LoginStatusCode { get; private set; }
    public LoginResponse? LoginResponse { get; private set; }
    public UserRecord? UserRecord { get; private set; }
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
    public void Response_ContainsRefreshToken()
    {
        Assert.NotNull(_fixture.LoginResponse);
        Assert.NotNull(_fixture.LoginResponse.RefreshToken);
        Assert.NotEmpty(_fixture.LoginResponse.RefreshToken);
    }
    
    [Fact]
    public void Response_TokenIsValidJwt()
    {
        Assert.NotNull(_fixture.LoginResponse);
        
        var tokenParts = _fixture.LoginResponse.Token.Split('.');
        Assert.Equal(3, tokenParts.Length);
        
        var tokenHandler = new JwtSecurityTokenHandler();
        Assert.True(tokenHandler.CanReadToken(_fixture.LoginResponse.Token), "Token should be valid JWT format");
    }
    
    [Fact]
    public void Response_TokenContainsExpectedClaims()
    {
        Assert.NotNull(_fixture.LoginResponse);
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(_fixture.LoginResponse.Token);
        
        Assert.Contains(jwtToken.Claims, c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        Assert.Contains(jwtToken.Claims, c => c.Type == "ClientIdentifier" && c.Value == "ww7client");
    }
    
    [Fact]
    public void Response_TokenExpiryIsReasonable()
    {
        Assert.NotNull(_fixture.LoginResponse);
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(_fixture.LoginResponse.Token);
        
        var tokenExpiry = jwtToken.ValidTo;
        var expectedExpiry = DateTime.UtcNow.AddMinutes(30);
        var expiryDelta = Math.Abs((tokenExpiry - expectedExpiry).TotalMinutes);
        Assert.True(expiryDelta < 5, $"Token expiry should be ~30 min from now, but delta is {expiryDelta} minutes");
    }
    
    [Fact]
    public void Response_ContainsUserDetails()
    {
        Assert.NotNull(_fixture.LoginResponse);
        Assert.Equal("api.tc25057.login@activeops.com", _fixture.LoginResponse.Username);
        Assert.NotEmpty(_fixture.LoginResponse.FirstName);
        Assert.NotEmpty(_fixture.LoginResponse.LastName);
        Assert.NotEmpty(_fixture.LoginResponse.Location);
    }
    
    // Database validation tests
    
    [Fact]
    public void Database_UserRecordExists()
    {
        Assert.NotNull(_fixture.UserRecord);
        Assert.True(_fixture.UserRecord.Id > 0);
        Assert.Equal("api.tc25057.login@activeops.com", _fixture.UserRecord.UserName);
    }
    
    [Fact]
    public void Database_LoginRecordCreated()
    {
        Assert.NotNull(_fixture.LoginDetailRecord);
        Assert.True(_fixture.LoginDetailRecord.Id > 0);
        Assert.Equal(_fixture.UserRecord!.Id, _fixture.LoginDetailRecord.UserId);
    }
    
    [Fact]
    public void Database_LoginTimestampIsRecent()
    {
        Assert.NotNull(_fixture.LoginDetailRecord);
        
        var timeSinceLogin = DateTime.UtcNow - _fixture.LoginDetailRecord.Created;
        Assert.True(timeSinceLogin.TotalSeconds < 30, 
            $"Login should have been recorded in last 30 seconds, but was {timeSinceLogin.TotalSeconds}s ago");
    }
    
    [Fact]
    public void Database_RefreshTokenMatchesResponse()
    {
        Assert.NotNull(_fixture.LoginResponse);
        Assert.NotNull(_fixture.LoginDetailRecord);
        Assert.Equal(_fixture.LoginResponse.RefreshToken, _fixture.LoginDetailRecord.RefreshToken);
    }
    
    [Fact]
    public void Database_RefreshTokenExpiryIsReasonable()
    {
        Assert.NotNull(_fixture.LoginDetailRecord);
        
        var refreshExpiry = _fixture.LoginDetailRecord.RefreshTokenExpiry;
        var timeSinceNow = refreshExpiry - DateTime.UtcNow;
        Assert.True(timeSinceNow.TotalMinutes > 20, 
            $"RefreshToken should expire in the future (>20 min), but expires in {timeSinceNow.TotalMinutes} minutes");
        Assert.True(timeSinceNow.TotalMinutes < 120, 
            $"RefreshToken expiry should be reasonable (<120 min), but expires in {timeSinceNow.TotalMinutes} minutes");
    }
}
