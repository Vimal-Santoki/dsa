using System.Net.Http.Json;
using Xunit;
using static DSA.E2E.Features.Sorting.dto;

namespace DSA.E2E.Features.Sorting
{
    public class SortingE2ETests
    {
        readonly HttpClient _client;
        public SortingE2ETests()
        {
            var baseUrl = Environment.GetEnvironmentVariable("DSA_API_BASE_URL") ?? "http://localhost:5000";
            _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
        }

        [Fact]
        [Trait("Category", "Smoke")]
        public async Task RunSortAlorithms_Should_Return_200_OK_In_Deployed_Env()
        {
            try
            {
                var response = await _client.GetAsync("/api/sort/");

                Assert.True(response.IsSuccessStatusCode, 
                    $"Critical smoke test failed! Count not reach api at {_client.BaseAddress}. Retruned {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Assert.Fail($"Critical smoke test failed! Count not reach api at {_client.BaseAddress}. Exception: {ex.Message}");
            }
        }

        [Fact]
        public async Task RunSortAlorithms_Should_Return_Sorted_Data()
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/sort/bubblesort", new int[] { 5, 3, 8, 1, 2 });
            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<E2ESortResult>();
            Assert.NotNull(result);
            Assert.Equal(new int[] { 1, 2, 3, 5, 8 }, (int[])result.SortedData);
            Assert.Equal("Bubble Sort", (string)result.Algorithm);
            Assert.True((int)result.Iterations > 0);
        }

        [Fact]
        public async Task RunSortAlorithms_Should_Hide_StackTraces_On_Error()
        {
            var response = await _client.PostAsJsonAsync($"/api/sort/invalidsort", new int[] { 5, 3, 8, 1, 2 });
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.NotFound);
            var errorContent = await response.Content.ReadAsStringAsync();
            Assert.DoesNotContain("System.Exception", errorContent); // Simple check to ensure stack trace is not included
            Assert.DoesNotContain("at ", errorContent); // Another simple check for stack trace lines
            Assert.DoesNotContain("DSA.", errorContent); // Ensure no internal namespaces are leaked
        }
    }
}