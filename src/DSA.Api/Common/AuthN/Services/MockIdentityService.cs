using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DSA.Api.Common.AuthN.Dto;
using DSA.Api.Common.AuthN.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DSA.Api.Common.AuthN.Services
{
    internal class MockIdentityService : IIdentityService
    {
        readonly IOptions<JwtSettings> _jwtSettings;
        readonly ILogger<MockIdentityService> _logger;

        public MockIdentityService(IOptions<JwtSettings> jwtSettings, ILogger<MockIdentityService> logger)
        {
            _jwtSettings = jwtSettings;
            _logger = logger;
        }
        public Task<TokenResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            // Mock user validation. We will replace this with real user creds from db later.
            var claimSub = "";
            var isValid = false;

            if (request.Username == "admin" && request.Password == "password")
            {
                isValid = true;
                claimSub = "user-123";
            }
            else if (request.Username == "guest" && request.Password == "guest")
            {
                isValid = true;
                claimSub = "user-456";
            }

            if (!isValid)
            {
                _logger.LogWarning("Login failed for user: {Username}. Invalid credentials.", request.Username);
                return Task.FromResult<TokenResponse?>(null);
            }

            var settings = _jwtSettings.Value;

            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sub, claimSub),
                new Claim(JwtRegisteredClaimNames.Name, request.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(settings.ExpiryMinutes),
                signingCredentials: creds);


            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var tokenResponse= new TokenResponse(tokenString, settings.ExpiryMinutes * 60);

            return Task.FromResult<TokenResponse?>(tokenResponse);
        }
    }
}
