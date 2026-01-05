using System.IdentityModel.Tokens.Jwt;
using AO.Automation.API.Client.Models.Requests.Login;
using AO.Automation.API.Client.Models.Responses.Login;

namespace AO.Automation.API.Client.Tests.Login;

/// <summary>
/// Azure Test Case: 25057
/// User with valid credentials can successfully login via API
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
            Username = "api.tc25057.login@activeops.com", // API test user 9200
            Password = "Workware@1"
        };
        
        // Act
        var response = await _apiHelper.PostAsync<LoginResponse>("/api/user/login", request);
        
        // Assert - Response
        Assert.Equal(200, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.NotNull(response.Data.Token);
        Assert.NotEmpty(response.Data.Token);
        Assert.NotNull(response.Data.RefreshToken);
        Assert.NotEmpty(response.Data.RefreshToken);
        
        // Verify JWT token format (3 parts separated by dots)
        var tokenParts = response.Data.Token.Split('.');
        Assert.Equal(3, tokenParts.Length);
        
        // Parse and validate JWT
        var tokenHandler = new JwtSecurityTokenHandler();
        Assert.True(tokenHandler.CanReadToken(response.Data.Token), "Token should be valid JWT format");
        
        var jwtToken = tokenHandler.ReadJwtToken(response.Data.Token);
        Assert.Contains(jwtToken.Claims, c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        Assert.Contains(jwtToken.Claims, c => c.Type == "ClientIdentifier" && c.Value == "ww7client");
        
        // Verify token expiry is reasonable (should be ~30 minutes from now)
        var tokenExpiry = jwtToken.ValidTo;
        var expectedExpiry = DateTime.UtcNow.AddMinutes(30);
        var expiryDelta = Math.Abs((tokenExpiry - expectedExpiry).TotalMinutes);
        Assert.True(expiryDelta < 5, $"Token expiry should be ~30 min from now, but delta is {expiryDelta} minutes");
        
        // Verify RefreshToken exists and is not empty (format varies by implementation)
        Assert.NotNull(response.Data.RefreshToken);
        Assert.NotEmpty(response.Data.RefreshToken);
        
        // Verify user details
        Assert.Equal(request.Username, response.Data.Username);
        Assert.NotEmpty(response.Data.FirstName);
        Assert.NotEmpty(response.Data.LastName);
        Assert.NotEmpty(response.Data.Location);
        
        // Assert - Database
        // Query by username to get user ID (response doesn't include user ID)
        var userRecord = await _dbHelper.QuerySingleOrDefaultAsync<Models.Database.UserRecord>(
            "SELECT * FROM [dbo].[User] WHERE UserName = @Username",
            new { Username = request.Username });
        
        Assert.NotNull(userRecord);
        
        var dbRecord = await _dbHelper.GetLatestLoginDetailAsync(userRecord.Id);
        Assert.NotNull(dbRecord);
        
        // Verify login timestamp is recent
        var timeSinceLogin = DateTime.UtcNow - dbRecord.Created;
        Assert.True(timeSinceLogin.TotalSeconds < 10, 
            $"Login should have been recorded in last 10 seconds, but was {timeSinceLogin.TotalSeconds}s ago");
        
        // Verify RefreshToken matches
        Assert.Equal(response.Data.RefreshToken, dbRecord.RefreshToken);
        
        // Verify RefreshTokenExpiry is reasonable (typically 20-120 minutes)
        var refreshExpiry = dbRecord.RefreshTokenExpiry;
        var timeSinceNow = refreshExpiry - DateTime.UtcNow;
        Assert.True(timeSinceNow.TotalMinutes > 20, 
            $"RefreshToken should expire in the future (>20 min), but expires in {timeSinceNow.TotalMinutes} minutes");
        Assert.True(timeSinceNow.TotalMinutes < 120, 
            $"RefreshToken expiry should be reasonable (<120 min), but expires in {timeSinceNow.TotalMinutes} minutes");
    }
}
