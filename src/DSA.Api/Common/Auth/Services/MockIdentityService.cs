using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DSA.Api.Common.Auth.Dto;
using DSA.Api.Common.Auth.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DSA.Api.Common.Auth.Services
{
    internal class MockIdentityService : IIdentityService
    {
        readonly IOptions<JwtSettings> _jwtSettings;
        public MockIdentityService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }
        public Task<TokenResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            // Mock user validation. We will replace this with real user creds from db later.
            var userName = "admin";
            var password = "password";
            var claimSub = "user-123"; // This would typically be the user's unique ID from the database.

            if (request.Username != userName || request.Password != password)
            {
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
