using System.Runtime.CompilerServices;
using DSA.Api.Features.Sorting.Api;
using DSA.Api.Features.Sorting.Extensions;
using DSA.Api.Shared;
using DSA.Api.Shared.Health;
using DSA.Api.Resilience;
using Microsoft.AspNetCore.HttpOverrides;

[assembly: InternalsVisibleTo("DSA.UnitTests")]
[assembly: InternalsVisibleTo("DSA.IntegrationTests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] // For NSubstitute

// .NET 9 provides out of the box class and main method support. The below code is equivalent to having a Main method.

var builder = WebApplication.CreateBuilder(args);

// Add global global exception handler and problem details middleware
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.AddRateLimiting();
builder.AddAppHealthChecks();
builder.AddSortingFeature();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseForwardedHeaders();
app.UseExceptionHandler();
app.UseRateLimiting();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        // IMPORTANT: The file name is 'v1.json', not 'swagger.json'
        options.SwaggerEndpoint("/openapi/v1.json", "DSA Learning API");
    });
}

app.MapAppHealthChecks();
app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapSortingEndpoints();

app.Run();

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1515:Consider usage of internal types", Justification = "Public for Integration Tests")]
public partial class Program { } // Make Program public for Integration Tests
