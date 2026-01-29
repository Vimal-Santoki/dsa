using System.Runtime.CompilerServices;
using DSA.Api.Features.Sorting.Api;
using DSA.Api.Features.Sorting.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using DSA.Api.Common.Health.Extensions;
using DSA.Api.Common.Health.Api;
using DSA.Api.Common;
using DSA.Api.Common.Resilience.Extensions;
using DSA.Api.Common.AuthN.Extensions;
using DSA.Api.Common.AuthN.Api;
using DSA.Api.Common.Iam.Extensions;
using DSA.Api.Common.AuthZ.Extensions;
using DSA.Api.Common.Observability.Extensions;
using DSA.Api.Common.Observability.Middleware;

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
builder.AddAuthN();
builder.AddAuthZ();
builder.AddIam();
builder.AddHealthChecks();
builder.AddObservability();
builder.AddSorting();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseForwardedHeaders();
app.UseExceptionHandler();
app.UseRateLimiting();
app.UseAuthentication();
app.UseMiddleware<UserContextLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();

    app.UseSwaggerUI(options =>
    {
        // IMPORTANT: The file name is 'v1.json', not 'swagger.json'
        options.SwaggerEndpoint("/openapi/v1.json", "DSA Learning API");
    });
}

app.UseAuthorization();

app.MapHealthCheckEndpoints();
app.MapGet("/", () => Results.Redirect("/swagger")).AllowAnonymous();

app.MapAuthNEndpoints();
app.MapSortingEndpoints();

app.Run();

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1515:Consider usage of internal types", Justification = "Public for Integration Tests")]
public partial class Program { } // Make Program public for Integration Tests
