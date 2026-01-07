using System.IdentityModel.Tokens.Jwt;
using AO.Automation.API.Client.Models.Requests.Login;
using AO.Automation.API.Client.Models.Requests.TokenRefresh;
using AO.Automation.API.Client.Models.Responses.Login;
using AO.Automation.API.Client.Models.Responses.TokenRefresh;
using AO.Automation.API.Client.Models.Database;
using Dapper;

namespace AO.Automation.API.Client.Tests.Login25146;

/// <summary>
/// Fixture for TokenRefresh - runs setup ONCE for all tests
/// </summary>
public class TokenRefreshFixture : ApiTestFixture
{
    public LoginResponse? InitialLoginResponse { get; private set; }
    public UserLoginDetailRecord? InitialLoginRecord { get; private set; }
    public string ExpiredAccessToken { get; private set; } = string.Empty;
    public int RefreshStatusCode { get; private set; }
    public TokenRefreshResponse? RefreshResponse { get; private set; }
    public UserLoginDetailRecord? UpdatedLoginRecord { get; private set; }
    public int UserId { get; private set; }
    
    protected override async Task SetupAsync()
    {
        // Step 1: Login to get valid refresh token
        var loginRequest = new LoginRequest
        {
            ClientIdentifier = "ww7client",
            Username = "api.tc25060.tokenrefresh@activeops.com",
            Password = "Workware@1"
        };
        
        var loginResponse = await ApiHelper.PostAsync<LoginResponse>("/api/user/login", loginRequest);
        InitialLoginResponse = loginResponse.Data;
        
        // Extract SessionValidationToken from real token to reuse in expired token
        var tokenHandler = new JwtSecurityTokenHandler();
        var realToken = tokenHandler.ReadJwtToken(InitialLoginResponse!.Token);
        var realSessionToken = realToken.Claims.First(c => c.Type == "SessionValidationToken").Value;
        
        // Get user ID
        var userRecord = await DbConnection.QuerySingleOrDefaultAsync<UserRecord>(
            "SELECT Id, UserName, StaffMemberId FROM [dbo].[User] WHERE UserName = @Username",
            new { Username = loginRequest.Username });
        
        UserId = userRecord!.Id;
        
        // Get initial login record
        InitialLoginRecord = await DbConnection.QuerySingleOrDefaultAsync<UserLoginDetailRecord>(
            "SELECT TOP 1 Id, UserId, RefreshToken, RefreshTokenExpiry, Created FROM [dbo].[UserLoginDetail] WHERE UserId = @UserId ORDER BY Created DESC",
            new { UserId });
        
        // Step 2: Generate expired access token (expired 5 minutes ago)
        var tokenHelper = ApiTestConfig.Instance.GetTokenHelper();
        ExpiredAccessToken = tokenHelper.GenerateAccessToken(
            username: loginRequest.Username,
            staffMemberId: userRecord.StaffMemberId,
            clientIdentifier: "ww7client",
            sessionValidationToken: realSessionToken,  // Reuse from real login
            expiryMinutes: -5  // Expired 5 minutes ago
        );
        
        // Step 3: Call refresh endpoint with expired token + valid refresh token
        var refreshRequest = new RefreshTokenRequest
        {
            Token = ExpiredAccessToken,
            RefreshToken = InitialLoginResponse.RefreshToken
        };
        
        var refreshResponse = await ApiHelper.PutAsync<TokenRefreshResponse>("/api/user/login", refreshRequest);
        RefreshStatusCode = refreshResponse.StatusCode;
        RefreshResponse = refreshResponse.Data;
        
        // Step 4: Get updated login record from database
        UpdatedLoginRecord = await DbConnection.QuerySingleOrDefaultAsync<UserLoginDetailRecord>(
            "SELECT TOP 1 Id, UserId, RefreshToken, RefreshTokenExpiry, Created FROM [dbo].[UserLoginDetail] WHERE UserId = @UserId ORDER BY Created DESC",
            new { UserId });
    }
}

/// <summary>
/// Azure Test Case: 25060
/// Token refresh allows getting new access token with expired token + valid refresh token
/// </summary>
[Trait("Suite", "Login-25146")]
[Trait("Feature", "Login")]
[Trait("API", "ClientAPI")]
public class TokenRefresh : IClassFixture<TokenRefreshFixture>
{
    private readonly TokenRefreshFixture _fixture;
    
    public TokenRefresh(TokenRefreshFixture fixture)
    {
        _fixture = fixture;
    }
    
    // Response validation tests
    
    [Fact]
    public void Response_HasSuccessStatusCode()
    {
        Assert.Equal(200, _fixture.RefreshStatusCode);
    }
    
    [Fact]
    public void Response_ContainsNewToken()
    {
        Assert.NotNull(_fixture.RefreshResponse);
        Assert.NotNull(_fixture.RefreshResponse.Token);
        Assert.NotEmpty(_fixture.RefreshResponse.Token);
    }
    
    [Fact]
    public void Response_ContainsNewRefreshToken()
    {
        Assert.NotNull(_fixture.RefreshResponse);
        Assert.NotNull(_fixture.RefreshResponse.RefreshToken);
        Assert.NotEmpty(_fixture.RefreshResponse.RefreshToken);
    }
    
    [Fact]
    public void Response_NewTokenIsDifferentFromExpiredToken()
    {
        Assert.NotNull(_fixture.RefreshResponse);
        Assert.NotEqual(_fixture.ExpiredAccessToken, _fixture.RefreshResponse.Token);
    }
    
    [Fact]
    public void Response_NewRefreshTokenIsDifferentFromOldOne()
    {
        Assert.NotNull(_fixture.RefreshResponse);
        Assert.NotNull(_fixture.InitialLoginResponse);
        Assert.NotEqual(_fixture.InitialLoginResponse.RefreshToken, _fixture.RefreshResponse.RefreshToken);
    }
    
    [Fact]
    public void Response_NewTokenIsValidJwt()
    {
        Assert.NotNull(_fixture.RefreshResponse);
        
        var tokenParts = _fixture.RefreshResponse.Token.Split('.');
        Assert.Equal(3, tokenParts.Length);
        
        var tokenHandler = new JwtSecurityTokenHandler();
        Assert.True(tokenHandler.CanReadToken(_fixture.RefreshResponse.Token), "New token should be valid JWT");
    }
    
    [Fact]
    public void Response_NewTokenExpiryIsReasonable()
    {
        Assert.NotNull(_fixture.RefreshResponse);
        Assert.NotNull(_fixture.UpdatedLoginRecord);
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(_fixture.RefreshResponse.Token);
        
        // New token should expire ~30 minutes from when refresh was called
        var tokenExpiry = jwtToken.ValidTo;
        var minutesDifference = (tokenExpiry - _fixture.UpdatedLoginRecord.Created).TotalMinutes;
        
        Assert.InRange(minutesDifference, 28, 32); // Should be ~30 minutes
    }
    
    // Database validation tests
    
    [Fact]
    public void Database_NewLoginRecordCreatedOrUpdated()
    {
        Assert.NotNull(_fixture.UpdatedLoginRecord);
        Assert.True(_fixture.UpdatedLoginRecord.Id > 0);
    }
    
    [Fact]
    public void Database_NewRefreshTokenMatchesResponse()
    {
        Assert.NotNull(_fixture.RefreshResponse);
        Assert.NotNull(_fixture.UpdatedLoginRecord);
        Assert.Equal(_fixture.RefreshResponse.RefreshToken, _fixture.UpdatedLoginRecord.RefreshToken);
    }
    
    [Fact]
    public void Database_NewRefreshTokenIsDifferentFromOldOne()
    {
        Assert.NotNull(_fixture.InitialLoginRecord);
        Assert.NotNull(_fixture.UpdatedLoginRecord);
        Assert.NotEqual(_fixture.InitialLoginRecord.RefreshToken, _fixture.UpdatedLoginRecord.RefreshToken);
    }
    
    [Fact]
    public void Database_NewRefreshTokenExpiryIsReasonable()
    {
        Assert.NotNull(_fixture.UpdatedLoginRecord);
        
        // New refresh token should expire ~90 minutes from when it was created
        var minutesDifference = (_fixture.UpdatedLoginRecord.RefreshTokenExpiry - _fixture.UpdatedLoginRecord.Created).TotalMinutes;
        
        Assert.InRange(minutesDifference, 88, 92); // Should be ~90 minutes
    }
}
