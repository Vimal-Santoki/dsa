using DSA.Api.Features.Sorting.Algorithms;
using DSA.Api.Features.Sorting.Interfaces;

namespace DSA.Api.Features.Sorting.Extensions
{
    internal static class SortingExtensions
    {
        public static void AddSorting(this IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<ISortAlgorithm, BubbleSort>();
        }
    }
}
