using System.Net.Http.Headers;
using System.Net.Http.Json;
using DSA.Api.Common.Auth.Dto;
using DSA.IntegrationTests.Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace DSA.IntegrationTests.Features.Sorting.Api
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1515:Consider usage of internal types", Justification = "xUnit requirement")]
    public sealed class SortingErrorTests : IClassFixture<SortingApiFactory>, IAsyncLifetime
    {
        readonly SortingApiFactory _factory;
        readonly HttpClient _client;

        public SortingErrorTests(SortingApiFactory factory)
        {
            ArgumentNullException.ThrowIfNull(factory);
            _factory = factory;
            _client = _factory.CreateClient();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        public async Task InitializeAsync() => await _client.AuthenticateAsync();

        [Fact]
        public async Task RunSortAlgorithm_Should_Return_500_When_Algorithm_Crashes()
        {
            // Arrange
            var dataToSort = new int[] { 5, 3, 8, 1, 2 };
            var algorithm = "MS"; // MockSort code
            // Setup the mock to throw an exception when Sort is called
            _factory.SortAlgorithmMock
                .When(a => a.Sort(Arg.Any<int[]>()))
                .Do(x => { throw new InvalidOperationException("Database global failure"); });
            // Act
            var response = await _client.PostAsJsonAsync($"/api/sort/{algorithm}", dataToSort);
            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);

            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            Assert.NotNull(problemDetails);
            Assert.Equal("An unexpected error occurred.", problemDetails.Title);
            Assert.Equal(500, problemDetails.Status);
            Assert.Equal("Database global failure", problemDetails.Detail);
        }
    }
}
