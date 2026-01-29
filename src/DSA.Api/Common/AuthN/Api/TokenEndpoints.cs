using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DSA.Api.Common.AuthN.Dto;
using DSA.Api.Common.AuthN.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DSA.Api.Common.AuthN.Api
{
    internal static class TokenEndpoints
    {
        public static void MapTokenEndpoints(this WebApplication app)
        {
            app.MapPost("/connect/token", GetToken)
                .WithTags("Authentication")
                .AllowAnonymous()
                .Produces<TokenResponse>(200)
                .Produces(401);
        }

        static async Task<IResult> GetToken(
            [FromBody] Dto.LoginRequest request, 
            [FromServices] IIdentityService identityService,
            [FromServices] ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("DSA.Api.Common.AuthN.Api.TokenEndpoints");
            
            var result = await identityService.LoginAsync(request);

            if (result is null)
            {
                logger.LogDebug("Token request denied for user: {Username}", request.Username);
                return Results.Unauthorized();
            }

            return Results.Ok(result);
        }
    }
}
