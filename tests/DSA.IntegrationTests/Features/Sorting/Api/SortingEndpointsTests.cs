using DSA.Api.Features.Sorting.Dto;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;

namespace DSA.IntegrationTests.Features.Sorting.Api
{
    public class SortingEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public SortingEndpointsTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task GetAlgorithms_ShouldReturnListOfAlgorithms()
        {
            // Act
            var response = await _client.GetAsync("/api/sort/");
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
            var dataToSort = new int[] { 5, 3, 8, 1, 2 };
            var algorithm = "bubblesort";
            // Act
            var response = await _client.PostAsJsonAsync($"/api/sort/{algorithm}", dataToSort);
            // Assert
            response.EnsureSuccessStatusCode();
            var sortResult = await response.Content.ReadFromJsonAsync<SortResult>();
            Assert.NotNull(sortResult);
            Assert.Equal(new int[] { 1, 2, 3, 5, 8 }, sortResult.SortedData);
            Assert.Equal("bubble sort", sortResult.Algorithm.ToLower());
            Assert.True(sortResult.Iterations > 0);
        }

        [Fact]
        public async Task RunSortAlgorithm_InvalidAlgorithm_ShouldReturnNotFound()
        {
            // Arrange
            var dataToSort = new int[] { 5, 3, 8, 1, 2 };
            var algorithm = "invalidsort";
            // Act
            var response = await _client.PostAsJsonAsync($"/api/sort/{algorithm}", dataToSort);
            // Assert
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.NotFound);
        }
    }
}