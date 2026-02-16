using System.Net.Http.Headers;
using System.Net.Http.Json;
using DSA.E2E.Extensions;
using Xunit;

namespace DSA.E2E.Features.Sorting
{
    public sealed class SortingE2ETests : IDisposable, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private static readonly int[] _testArray = [5, 3, 8, 1, 2];
        private static readonly int[] _expectedSortedArray = [1, 2, 3, 5, 8];

        public SortingE2ETests()
        {
            var baseUrl = Environment.GetEnvironmentVariable("DSA_API_BASE_URL") ?? "http://localhost:5000";
            _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        [Fact]
        [Trait("Category", "Smoke")]
        public async Task RunSortAlorithms_ShouldReturn200OKInDeployedEnv()
        {
            try
            {
                // CA2234: Use Uri instead of string for HttpClient
                var response = await _client.GetAsync(new Uri("/api/sort/", UriKind.Relative));

                Assert.True(response.IsSuccessStatusCode,
                    $"Critical smoke test failed! Could not reach api at {_client.BaseAddress}. Returned {response.StatusCode}");
            }
            catch (HttpRequestException ex) // CA1031: Catch specific exception
            {
                Assert.Fail($"Critical smoke test failed! Could not reach api at {_client.BaseAddress}. Exception: {ex.Message}");
            }
        }

        [Fact]
        public async Task RunSortAlorithms_Should_Return_Sorted_Data()
        {
            // Act
            // CA1861: Use static readonly field for array argument
            var response = await _client.PostAsJsonAsync("/api/sort/bubblesort", _testArray);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<E2ESortResult>();

            Assert.NotNull(result);
            Assert.Equal(_expectedSortedArray, result.SortedData);
            Assert.Equal("Bubble Sort", result.Algorithm);
        }

        [Fact]
        public async Task RunSortAlorithms_Should_Hide_StackTraces_On_Error()
        {
            var response = await _client.PostAsJsonAsync($"/api/sort/invalidsort", _testArray);
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.NotFound);

            var errorContent = await response.Content.ReadAsStringAsync();

            // CA1307: Specify StringComparison
            Assert.DoesNotContain("System.Exception", errorContent, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("at ", errorContent, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("DSA.", errorContent, StringComparison.OrdinalIgnoreCase);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        public async Task InitializeAsync() => await _client.AuthenticateAsync();

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
