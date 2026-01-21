using NSubstitute;
using System.Net.Http.Json;

namespace DSA.IntegrationTests.Features.Sorting.Api
{
    public class SortingErrorTests : IClassFixture<SortingApiFactory>
    {
        readonly SortingApiFactory _factory;
        readonly HttpClient _client;

        public SortingErrorTests(SortingApiFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task RunSortAlgorithm_Should_Return_500_When_Algorithm_Crashes()
        {
            // Arrange
            var dataToSort = new int[] { 5, 3, 8, 1, 2 };
            var algorithm = "MS"; // MockSort code
            // Setup the mock to throw an exception when Sort is called
            _factory.SortAlgorithmMock
                .When(a => a.Sort(Arg.Any<int[]>()))
                .Do(x => { throw new Exception("Database global failure"); });
            // Act
            var response = await _client.PostAsJsonAsync($"/api/sort/{algorithm}", dataToSort);
            // Assert
            Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
        }
    }
}
