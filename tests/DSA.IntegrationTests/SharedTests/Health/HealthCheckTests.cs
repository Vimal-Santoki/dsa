using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DSA.IntegrationTests.SharedTests.Health
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1515:Consider usage of internal types", Justification = "xUnit requirement")]
    public sealed class HealthCheckTests : IClassFixture<WebApplicationFactory<Program>>
    {
        readonly HttpClient _client;

        public HealthCheckTests(WebApplicationFactory<Program> factory)
        {
            _client = factory?.CreateClient() ?? throw new ArgumentNullException(nameof(factory));
        }

        [Theory]
        [InlineData("/health/live")]
        [InlineData("/health/ready")]
        public async Task Health_Endpoint_Should_Return_Healthy(string endpoint) {
            // Act
            var response = await _client.GetAsync(new Uri(endpoint, UriKind.Relative));

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Healthy", content, StringComparison.OrdinalIgnoreCase);
}
    }
}
