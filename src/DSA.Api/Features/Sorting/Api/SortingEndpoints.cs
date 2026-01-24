using DSA.Api.Features.Sorting.Algorithms;
using DSA.Api.Features.Sorting.Dto;
using DSA.Api.Features.Sorting.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DSA.Api.Features.Sorting.Api
{
    internal static class SortingEndpoints
    {
        public static void MapSortingEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/sort")
                .WithTags("Sorting Algorithms");


            group.MapGet("/", GetAlgorithms)
                .Produces<IEnumerable<AlgorithmInfo>>(200);

            // algorithm post
            group.MapPost("/{algorithm}", RunSortAlgorithm)
                .Produces<SortResult>(200)
                .Produces(404);
        }

       public static IResult RunSortAlgorithm([FromRoute] string algorithm, [FromBody] int[] data, [FromServices] IEnumerable<ISortAlgorithm> sortAlgorithms)
        {
            try
            {
                if (data == null)
                {
                    return Results.BadRequest("Data array cannot be null.");
                }

                var selectedAlgorithm = sortAlgorithms.FirstOrDefault(a =>
                    a.Code.Equals(algorithm, StringComparison.OrdinalIgnoreCase));

                if (selectedAlgorithm == null)
                {
                    return Results.NotFound($"Sorting algorithm '{algorithm}' not found.");
                }

                var iterations = selectedAlgorithm.Sort(data);
                return Results.Ok(new SortResult
                {
                    Algorithm = selectedAlgorithm.Name,
                    Iterations = iterations,
                    SortedData = data
                });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        public static IResult GetAlgorithms([FromServices] IEnumerable<ISortAlgorithm> sortAlgorithms)
        {
            var response = sortAlgorithms.Select(a =>
            new AlgorithmInfo
            {
                Code = a.Code,
                DisplayName = a.Name,
                Category = a.Category
            });
            return Results.Ok(response);
        }
    }
}
