using System.Net;
using System.Net.Http.Json;
using DSA.Api.Features.Sorting.Dto;
using DSA.IntegrationTests.Common.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;

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

        //[Fact]
        //public async Task RunSortAlgorithm_WhenOverloaded_ShouldRejectExcessRequests()
        //{
        //    // Arrange
        //    // We need enough load to trigger the Bulkhead (Limit=2, Queue=4 => Capacity=6)
        //    // Sending 10 requests should cause ~4 failures.
        //    var parallelCount = 7;
        //    var endpoint = "/api/sort/bubblesort";
        //    var tasks = new List<Task<HttpResponseMessage>>();
        //    var largeInput = Enumerable.Range(1, 70000).Reverse().ToArray();
        //    // Act
        //    for (var i = 0; i < parallelCount; i++)
        //    {
        //        // We fire them all "at once" without awaiting immediately
        //        tasks.Add(_client.PostAsJsonAsync(endpoint, largeInput));
        //    }

        //    var responses = await Task.WhenAll(tasks);

        //    // Assert
        //    var successCount = responses.Count(r => r.IsSuccessStatusCode);
        //    var rejectedCount = responses.Count(r => r.StatusCode == HttpStatusCode.ServiceUnavailable || r.StatusCode == HttpStatusCode.TooManyRequests);

        //    // We expect at least 2 successes (Bulkhead limit) and at least 1 rejection. But exact counts may vary.
        //    Assert.True(successCount >= 2, $"Expected at least 2 successful responses, got {successCount}");
        //    Assert.True(rejectedCount >= 1, $"Expected at least 1 rejected responses, got {rejectedCount}");
        //}

        public async Task InitializeAsync() => await _client.AuthenticateAsync();

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
