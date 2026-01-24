using System.Net.Http.Headers;
using System.Net.Http.Json;
using DSA.Api.Common.Auth.Dto;
using DSA.Api.Features.Sorting.Dto;
using DSA.IntegrationTests.Common.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DSA.IntegrationTests.Features.Sorting.Api
{
    public sealed class SortingEndpointsTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private static readonly int[] _testData = [5, 3, 8, 1, 2];
        private static readonly int[] _expectedData = [1, 2, 3, 5, 8];

        public SortingEndpointsTests(WebApplicationFactory<Program> factory)
        {
            ArgumentNullException.ThrowIfNull(factory);
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task GetAlgorithms_ShouldReturnListOfAlgorithms()
        {
            // Act
            var response = await _client.GetAsync(new Uri("/api/sort/", UriKind.Relative));
            // Assert
            response.EnsureSuccessStatusCode();
            var algorithms = await response.Content.ReadFromJsonAsync<IEnumerable<AlgorithmInfo>>();
            Assert.NotNull(algorithms);
            Assert.Contains(algorithms, a => a.Code == "BubbleSort" && a.DisplayName == "Bubble Sort");
        }

        [Fact]
        public async Task RunSortAlgorithm_ShouldReturnSortedData()
        {
            // Arrange
            var algorithm = "bubblesort";
            // Act
            var response = await _client.PostAsJsonAsync($"/api/sort/{algorithm}", _testData);
            // Assert
            response.EnsureSuccessStatusCode();
            var sortResult = await response.Content.ReadFromJsonAsync<SortResult>();
            Assert.NotNull(sortResult);
            Assert.Equal(_expectedData, sortResult.SortedData);
            Assert.Equal("bubble sort", sortResult.Algorithm, ignoreCase: true);
            Assert.True(sortResult.Iterations > 0);
        }

        [Fact]
        public async Task RunSortAlgorithm_InvalidAlgorithm_ShouldReturnNotFound()
        {
            // Arrange
            var algorithm = "invalidsort";
            // Act
            var response = await _client.PostAsJsonAsync($"/api/sort/{algorithm}", _testData);
            // Assert
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.NotFound);
        }

        public async Task InitializeAsync() => await _client.AuthenticateAsync();

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
