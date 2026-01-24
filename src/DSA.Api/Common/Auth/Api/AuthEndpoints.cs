namespace DSA.Api.Common.Auth.Api
{
    internal static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            app.MapTokenEndpoints();
        }
    }
}
