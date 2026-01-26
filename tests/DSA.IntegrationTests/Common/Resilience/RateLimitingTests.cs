using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DSA.Api.Common.AuthN.Dto;
using DSA.Api.Common.Resilience.Dto;
using DSA.IntegrationTests.Common.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace DSA.IntegrationTests.SharedTests.Resilience
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1515:Consider usage of internal types", Justification = "xUnit requirement")]
    public sealed class RateLimitingTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public RateLimitingTests(WebApplicationFactory<Program> factory)
        {
            ArgumentNullException.ThrowIfNull(factory);

            _factory = factory.WithWebHostBuilder(builder =>
            {
                // 1. Fast settings for test
                builder.UseSetting($"{RateLimitingSettings.SectionName}:WindowInSeconds", "2");

                // 2. Inject middleware that allows us to set the IP per request via a header
                //    This simulates the "ForwardedHeaders" behavior in a test environment
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IStartupFilter, FakeIpStartupFilter>();
                });
            });
        }

        [Fact]
        public async Task Rate_Limiting_Should_Allow_Under_Limit()
        {
            
            var client = _factory.CreateClient();
            await client.AuthenticateAsync();
            var endpoint = new Uri("/api/sort", UriKind.Relative);
            
            // Unique IP
            client.DefaultRequestHeaders.Add("X-Test-IP", "192.168.1.50");
            
            for(var i=0; i<50; i++) 
            {
                 using var res = await client.GetAsync(endpoint);
                 res.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task Rate_Limiting_Should_Enforce_Limits()
        {
            var client = _factory.CreateClient();
            await client.AuthenticateAsync();
            var endpoint = new Uri("/api/sort", UriKind.Relative);
            
            // Unique IP
            client.DefaultRequestHeaders.Add("X-Test-IP", "192.168.1.51");

            // Exhaust
            for(var i=0; i<100; i++) 
            {
                 using var res = await client.GetAsync(endpoint);
            }

            // Verify Block
            using var blocked = await client.GetAsync(endpoint);
            Assert.Equal(HttpStatusCode.TooManyRequests, blocked.StatusCode);
        }

        [Fact]
        public async Task Rate_Limiting_Should_Reset_After_Interval()
        {
            var client = _factory.CreateClient();
            await client.AuthenticateAsync();
            var endpoint = new Uri("/api/sort", UriKind.Relative);
            client.DefaultRequestHeaders.Add("X-Test-IP", "192.168.1.52");

            // Exhaust
            for(var i=0; i<100; i++) 
            {
                 await client.GetAsync(endpoint);
            }

            // Wait (Window is 2s)
            await Task.Delay(TimeSpan.FromSeconds(3));

            // Should be fresh
            using var fresh = await client.GetAsync(endpoint);
            Assert.Equal(HttpStatusCode.OK, fresh.StatusCode);
        }

         [Fact]
        public async Task Health_Endpoints_Should_Bypass_Rate_Limit()
        {
            var client = _factory.CreateClient();
            await client.AuthenticateAsync();
            var endpoint = new Uri("/health/ready", UriKind.Relative);
            client.DefaultRequestHeaders.Add("X-Test-IP", "192.168.1.53");
            
            for (var i = 0; i < 120; i++) 
            {
                using var response = await client.GetAsync(endpoint);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1515:Do not catch general exception types", Justification = "Test code")]
    // --- Test Helpers ---
    public class FakeIpStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseMiddleware<FakeIpMiddleware>();
                next(app);
            };
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1515:Do not catch general exception types", Justification = "Test code")]
    public  class FakeIpMiddleware
    {
        private readonly RequestDelegate _next;
        public FakeIpMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Request.Headers.TryGetValue("X-Test-IP", out var ipVal) && 
                IPAddress.TryParse(ipVal.ToString(), out var ip))
            {
                context.Connection.RemoteIpAddress = ip;
            }
            await _next(context);
        }
    }
}
