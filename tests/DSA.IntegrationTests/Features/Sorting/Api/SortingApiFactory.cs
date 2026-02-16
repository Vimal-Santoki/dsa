using DSA.Api.Features.Sorting.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace DSA.IntegrationTests.Features.Sorting.Api
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1515:Consider usage of internal types", Justification = "xUnit Fixture must be public")]
    public sealed class SortingApiFactory : WebApplicationFactory<Program>
    {
        internal ISortAlgorithm SortAlgorithmMock { get; private set; } = default!;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.ConfigureServices(services =>
            {
                // find the existing registrations
                var descriptors = services.Where(
                    d => d.ServiceType == typeof(ISortAlgorithm))
                    .ToList();

                // remove all of them
                foreach (var d in descriptors)
                {
                    services.Remove(d);
                }

                // create and register the mock
                SortAlgorithmMock = Substitute.For<ISortAlgorithm>();

                // setup mock properties that endpoint expects.
                SortAlgorithmMock.Name.Returns("MockSort");
                SortAlgorithmMock.Code.Returns("MS");

                services.AddSingleton(SortAlgorithmMock);
            });
        }
    }
}
