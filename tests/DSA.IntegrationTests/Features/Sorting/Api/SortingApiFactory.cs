using DSA.Api.Features.Sorting.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace DSA.IntegrationTests.Features.Sorting.Api
{
    public class SortingApiFactory:WebApplicationFactory<Program>
    {
        public ISortAlgorithm SortAlgorithmMock { get; private set; } 

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // find the existing registration and remove it
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ISortAlgorithm));

                if (descriptor != null) { 
                    services.Remove(descriptor);
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
