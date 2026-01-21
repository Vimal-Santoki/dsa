using DSA.Api.Features.Sorting.Algorithms;
using DSA.Api.Features.Sorting.Interfaces;

namespace DSA.Api.Features.Sorting.Extensions
{
    internal static class SortingExtensions
    {
        public static WebApplicationBuilder AddSortingFeature(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<ISortAlgorithm, BubbleSort>();
            return builder;
        }
    }
}
