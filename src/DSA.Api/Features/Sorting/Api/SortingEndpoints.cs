using DSA.Api.Common.AuthZ.Dto;
using DSA.Api.Common.AuthZ.Extensions;
using DSA.Api.Features.Sorting.Dto;
using DSA.Api.Features.Sorting.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using DSA.Api.Common.Iam.Constants;

namespace DSA.Api.Features.Sorting.Api
{
    internal static class SortingEndpoints
    {
        const string listPermission = AppPermissions.Sorting.List;
        const string executePermission = AppPermissions.Sorting.Execute;
        public static void MapSortingEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/sort")
                .WithTags("Sorting Algorithms");


            group.MapGet("/", GetAlgorithms)
                .RequirePermission(listPermission, "Sorting"); // authenticated user x can list all sorting algorithms if they have permission

            // algorithm post
            group.MapPost("/{algorithm}", RunSortAlgorithm)
                .RequirePermission(executePermission, RouteParam.From("algorithm")); // authenticated user x can execute sorting algorithm y if they have permission
        }

        public static Results<Ok<SortResult>, NotFound<string>, BadRequest<string>> RunSortAlgorithm(
            [FromRoute] string algorithm, 
            [FromBody] int[] data,
            [FromServices] IEnumerable<ISortAlgorithm> sortAlgorithms)
        {
            try
            {
                if (data == null)
                {
                    return TypedResults.BadRequest("Data array cannot be null.");
                }

                var selectedAlgorithm = sortAlgorithms.FirstOrDefault(a =>
                    a.Code.Equals(algorithm, StringComparison.OrdinalIgnoreCase));

                if (selectedAlgorithm == null)
                {
                    return TypedResults.NotFound($"Sorting algorithm '{algorithm}' not found.");
                }

                var iterations = selectedAlgorithm.Sort(data);
                return TypedResults.Ok(new SortResult
                {
                    Algorithm = selectedAlgorithm.Name,
                    Iterations = iterations,
                    SortedData = data
                });
            }
            catch (ArgumentException ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        public static Ok<IEnumerable<AlgorithmInfo>> GetAlgorithms([FromServices] IEnumerable<ISortAlgorithm> sortAlgorithms)
        {
            var response = sortAlgorithms.Select(a =>
            new AlgorithmInfo
            {
                Code = a.Code,
                DisplayName = a.Name,
                Category = a.Category
            });
            return TypedResults.Ok(response);
        }
    }
}
