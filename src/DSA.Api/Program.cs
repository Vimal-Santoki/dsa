using DSA.Api.Features.Sorting.Api;
using DSA.Api.Features.Sorting.Extensions;

// .NET 9 provides out of the box class and main method support. The below code is equivalent to having a Main method.

var builder = WebApplication.CreateBuilder(args);
builder.AddSortingFeature();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        // IMPORTANT: The file name is 'v1.json', not 'swagger.json'
        options.SwaggerEndpoint("/openapi/v1.json", "DSA Learning API");
    });
}

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapSortingEndpoints();

app.Run();

public partial class Program { } // For integration testing purposes