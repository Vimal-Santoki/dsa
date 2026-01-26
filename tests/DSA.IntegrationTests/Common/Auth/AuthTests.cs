using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Text;
using DSA.Api.Common.AuthN.Dto;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DSA.IntegrationTests.Common.Auth
{
    public class AuthTests: IClassFixture<WebApplicationFactory<Program>>
    {
        readonly HttpClient _client;
        public AuthTests(WebApplicationFactory<Program> factory)
        {
            ArgumentNullException.ThrowIfNull(factory);
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetToken_Should_Return_Unauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest("testuser","wrongpassword");
            var requestUri = "/connect/token"; // Replace with actual protected endpoint
            // Act
            var response = await _client.PostAsJsonAsync(requestUri, loginRequest);
            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetToken_Should_Return_Valid_Jwt_Token_If_Authenticated()
        {
            // Arrange
            var loginRequest = new LoginRequest("admin","password");
            var requestUri = "/connect/token"; // Replace with actual protected endpoint
            // Act
            var response = await _client.PostAsJsonAsync(requestUri, loginRequest);
            // Assert
            response.EnsureSuccessStatusCode();
            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            Assert.NotNull(tokenResponse);
            Assert.False(string.IsNullOrEmpty(tokenResponse.AccessToken));

            var handler = new JwtSecurityTokenHandler();
            Assert.True(handler.CanReadToken(tokenResponse.AccessToken), "Token should be a valid JWT format");

            var jwtToken = handler.ReadJwtToken(tokenResponse.AccessToken);
            Assert.Equal("dsa-api", jwtToken.Issuer);
        }
    }
}
