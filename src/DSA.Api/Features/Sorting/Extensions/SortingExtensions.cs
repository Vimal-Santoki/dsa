using DSA.Api.Common.Extensions;
using DSA.Api.Features.Sorting.Algorithms;
using DSA.Api.Features.Sorting.Algorithms.Decorators;
using DSA.Api.Features.Sorting.Interfaces;

namespace DSA.Api.Features.Sorting.Extensions
{
    internal static class SortingExtensions
    {
        public static void AddSorting(this IHostApplicationBuilder builder)
        {
            builder.Services.AddDecorate<ISortAlgorithm, BubbleSort, ObservabilitySortDecorator>();
            builder.Services.AddDecorate<ISortAlgorithm, QuickSort, ObservabilitySortDecorator>();
        }
    }
}

