using System.Net;
using System.Net.Http.Json;
using DSA.Api.Features.Sorting.Dto;
using DSA.IntegrationTests.Common.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DSA.IntegrationTests.Common.AuthZ
{
    public class AuthZTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private static readonly int[] _testArray = [1, 2, 3];
        private static readonly int[] _reverseArray = [3, 2, 1];

        public AuthZTests(WebApplicationFactory<Program> factory)
        {
            ArgumentNullException.ThrowIfNull(factory);
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task AccessProtectedEndpoint_WithoutToken_ShouldReturnUnauthorized()
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/sort/bubblesort", _testArray);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AccessProtectedEndpoint_WithAdminUser_ShouldReturnOk()
        {
            // Arrange
            await _client.AuthenticateAsync("admin", "password");

            // Act
            var response = await _client.PostAsJsonAsync("/api/sort/bubblesort", _reverseArray);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AccessProtectedListEndpoint_WithGuestUser_ShouldReturnOk()
        {
            // Arrange
            await _client.AuthenticateAsync("guest", "guest");

            // Act
            // Guest has Sorting:List permission (default read-only policy in MockPermissionService)
            var response = await _client.GetAsync(new Uri("/api/sort", UriKind.Relative));

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AccessProtectedExecuteEndpoint_WithGuestUser_ShouldReturnForbidden()
        {
            // Arrange
            await _client.AuthenticateAsync("guest", "guest");

            // Act
            // Guest has Sorting:Execute DENY (default read-only policy in MockPermissionService)
            var response = await _client.PostAsJsonAsync("/api/sort/bubblesort", _reverseArray);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
